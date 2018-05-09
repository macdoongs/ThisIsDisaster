using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NetworkComponents;

public class PlayerBehaviour : UnitBehaviourBase
{
    
    private void Start()
    {

        _prevPos = Controller.GetPosition();
    }

    private void Update()
    {
        try
        {
            CalcRemotePosition();
            CalcLocalPosition();
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
    }

    public virtual bool IsLocal() {
        return false;
    }

    public override void Initialize()
    {
        
    }

}

public class SplineData {
    public const int SEND_ITERVAL = 10;

    private List<CharacterCoordinates> _points = new List<CharacterCoordinates>();

    public void CalcSpline(List<CharacterCoordinates> dPoints, int cullingNum) {
        int pointCount = dPoints.Count;
        _points.Clear();

        Vector3 ps = Vector3.zero;
        Vector3 pe = Vector3.zero;
        Vector3 vs = Vector3.zero;
        Vector3 ve = Vector3.zero;

        if (pointCount == 1)
        {
            ps = dPoints[pointCount - 1].GetVector();
            pe = ps;

        }
        else if (pointCount == 2)
        {
            ps = dPoints[pointCount - 2].GetVector();
            pe = dPoints[pointCount - 1].GetVector();

            vs = ps - pe;
            ve = vs;
        }
        else if (pointCount == 3)
        {
            ps = dPoints[pointCount - 2].GetVector();
            pe = dPoints[pointCount - 1].GetVector();
            vs = (dPoints[pointCount - 2].GetVector() - dPoints[pointCount - 3].GetVector());

            Vector3 v0 = (dPoints[pointCount - 2].GetVector() - dPoints[pointCount - 3].GetVector());
            Vector3 v1 = (dPoints[pointCount - 1].GetVector() - dPoints[pointCount - 2].GetVector());

            Vector3 v2 = v1 + (v1 - v0);


            ve = v2;
        }
        else if (pointCount >= 4)
        {
            pe = dPoints[pointCount - 2].GetVector();
            pe = dPoints[pointCount - 1].GetVector();

            vs = (dPoints[pointCount - 2].GetVector() - dPoints[pointCount - 3].GetVector());

            Vector3 v0 = (dPoints[pointCount - 3].GetVector() - dPoints[pointCount - 4].GetVector());
            Vector3 v1 = (dPoints[pointCount - 2].GetVector() - dPoints[pointCount - 3].GetVector());
            Vector3 v2 = (dPoints[pointCount - 1].GetVector() - dPoints[pointCount - 2].GetVector());

            Vector3 dv0 = v1 - v0;
            Vector3 dv1 = v2 - v1;
            Vector3 dv2 = dv1 + (dv1 - dv0);
            Vector3 v3 = v2 + dv2;

            ve = v3;
        }

        SimpleSpline.Curve spline = new SimpleSpline.Curve();
        spline.AppendCV(ps, vs * 0.5f);
        spline.AppendCV(pe, ve * 0.5f);
        SimpleSpline.Tracer tracer = new SimpleSpline.Tracer();

        tracer.Attach(spline);
        float totalDist = spline.CalcTotalDistance();
        for (int i = 0; i < SEND_ITERVAL; i++) {
            float rate = (float)i / (float)SEND_ITERVAL;
            tracer.ProceedByDistance(totalDist * rate);
            _points.Add(CharacterCoordinates.SetFromVector(tracer.GetCurrent().position));
        }
    }

    public int GetPlotNum() {
        return _points.Count;
    }

    public void GetPoint(int index, out CharacterCoordinates coord) {
        coord = _points[index];
    }
    
}

namespace SimpleSpline {
    public class ControlVertex {
        public int index;
        public Vector3 position;
        public Vector3 tangent;
        public float tension;
    }

    public class Curve {
        public List<ControlVertex> cvs = new List<ControlVertex>();

        public static ControlVertex Lerp(ControlVertex cv0, ControlVertex cv1, float[] posK, float[] tanK) {
            ControlVertex dest = new ControlVertex()
            {
                position
                = cv0.position * posK[0]
                + cv1.position * posK[1]
                + cv0.tangent * cv0.tension * posK[2]
                + cv1.tangent * cv1.tension * posK[3],
                tangent
                = cv0.position * tanK[0]
                + cv1.position * tanK[1]
                + cv0.tangent * cv0.tension * tanK[2]
                + cv1.tangent * cv1.tension * tanK[3]
            };

            return dest;
        }

        public static void CalcKonst(float[] destPosK, float[] destTanK, float t) {

            destPosK[0] = 2.0f * t * t * t - 3.0f * t * t + 1.0f;
            destPosK[1] = -2.0f * t * t * t + 3.0f * t * t;
            destPosK[2] = t * t * t - 2.0f * t * t + t;
            destPosK[3] = t * t * t - t * t;

            destTanK[0] = 6.0f * t * t - 6.0f * t;
            destTanK[1] = -6.0f * t * t + 6.0f * t;
            destTanK[2] = 3.0f * t * t - 4.0f * t + 1.0f;
            destTanK[3] = 3.0f * t * t - 2.0f * t;
        }

        public void AppendCV(Vector3 pos, Vector3 slope) {
            ControlVertex cv = new ControlVertex() {
                index = cvs.Count,
                position = pos,
                tension = slope.magnitude,
                tangent = slope.normalized
            };

            cvs.Add(cv);
        }

        public float GetEnd() {
            float end = (float)(cvs.Count - 1);

            return end;
        }

        public ControlVertex CalcVertex(float t) {
            ControlVertex cv = null;
            int segmentIndex = Mathf.FloorToInt(t);
            if (segmentIndex < 0)
            {
                cv = cvs[0];
            }
            else if (cvs.Count - 1 <= segmentIndex)
            {
                cv = cvs[cvs.Count - 1];
            }
            else {
                float[] posK = new float[4];
                float[] tanK = new float[4];

                ControlVertex cv0 = cvs[segmentIndex];
                ControlVertex cv1 = cvs[segmentIndex + 1];
                float lp = t - (float)segmentIndex;

                CalcKonst(posK, tanK, lp);
                cv = Lerp(cv0, cv1, posK, tanK);

            }

            return cv;
        }

        public float CalcTotalDistance() {
            float dist = 0f;

            ControlVertex cvPrev = CalcVertex(0f);
            float t = 0f;
            float dt = 0.1f;
            float tMax = (float)(cvs.Count - 1);
            int safeCount = Mathf.CeilToInt(tMax / dt) + 1;

            for (int i = 0; i < safeCount; i++) {
                ControlVertex cvCurrent = CalcVertex(t);
                dist += Vector3.Distance(cvPrev.position, cvCurrent.position);
                if (t >= tMax) {
                    break;
                }

                t += dt;
                t = Mathf.Min(t, tMax);
                cvPrev = cvCurrent;
            }

            return dist;
        }
    }

    public class Tracer {
        public Curve curve = null;
        public ControlVertex cv = new ControlVertex();
        public bool isEnded = false;
        public float t = -1.0f;
        public float distance = 0f;

        public void Attach(Curve curve) {
            this.curve = curve;
        }

        public void Restart() {
            isEnded = false;
            t = -1.0f;
            distance = 0f;
            cv = new ControlVertex();
        }

        public ControlVertex GetCurrent(){ return cv; }

        public void SetCurrentByDistance(float distance) {
            ProceedToDistance(distance);
            isEnded = false;
        }

        public void Proceed(float dt) {
            if (!isEnded) {
                if (t < 0f)
                {
                    t = 0f;
                }
                else {
                    t += dt;
                }

                if (t >= curve.GetEnd()) {
                    t = curve.GetEnd();
                    isEnded = true;
                }

                cv = curve.CalcVertex(t);
            }
        }

        public void ProceedToDistance(float dist) {
            if (distance < 0f) {
                distance = 0f;
                t = 0f;
            }

            ProceedByDistance(dist - distance);
            distance = dist;
        }

        public void ProceedByDistance(float dist) {
            do {
                if (isEnded) break;
                ControlVertex cv0;

                if (t < 0f) {
                    t = 0f;
                }

                cv0 = curve.CalcVertex(t);
                cv = cv0;

                float dt = dist / curve.CalcTotalDistance() * curve.GetEnd();
                float sdt = dt >= 0f ? 1f : -1f;
                dt = Mathf.Abs(dt);
                float dt0 = dt;
                float t0 = t;
                float advanced = 0f;
                float advanced0 = advanced;
                int i = 0;
                int maxTimes = Mathf.CeilToInt(curve.GetEnd() / dt);

                maxTimes = Mathf.Max(maxTimes, 1);

                for (i = 0; i < maxTimes; i++) {
                    t0 = t;
                    t += sdt * dt;
                    if (t >= curve.GetEnd() && sdt >= 0f)
                    {
                        t = curve.GetEnd();
                        if (dt < dt0)
                        {
                            isEnded = true;
                        }

                        dt = Mathf.Max(0f, t - t0);

                    }
                    else if (t < 0f && sdt < 0f) {
                        t = 0f;
                        if (dt < dt0) {
                            isEnded = true;
                        }

                        dt = Mathf.Max(0f, Mathf.Abs(t - t0));
                    }

                    cv = curve.CalcVertex(t);
                    advanced0 = advanced;
                    advanced += Vector3.Distance(cv0.position, cv.position);

                    if (isEnded) break;

                    if (advanced >= Mathf.Abs(dist))
                    {
                        dt *= 0.5f;
                        if (dt < dt0 / 100f)
                        {
                            break;
                        }

                        t = t0;
                        advanced = advanced0;
                    }
                    else {
                        cv0 = cv;
                    }
                }
                
            }
            while (false);
        }
    }
}