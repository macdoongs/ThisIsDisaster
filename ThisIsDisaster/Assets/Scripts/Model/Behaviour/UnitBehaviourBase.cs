using UnityEngine;
using System.Collections;
using SimpleSpline;
using System.Collections.Generic;
using NetworkComponents;

public class UnitBehaviourBase : MonoBehaviour
{
    private const int _PLOT_NUM = 4;
    private const int _CULLING_NUM = 10;
    private int _plotIndex = 0;

    private const float _SEND_SQR_DIFF = 0.0001f;
    protected int _coordSendCount = 0;
    protected Vector3 _prevPos = Vector3.zero;
    private const float _ZERO_VECTOR_MAG = 0.005f;

    public bool IsRemoteCharacter = false;

    public UnitControllerBase Controller = null;

    
    private List<CharacterCoordinates> _culling = new List<CharacterCoordinates>();
    private List<CharacterCoordinates> _plots = new List<CharacterCoordinates>();
    
    // Use this for initialization
    void Start()
    {
        _prevPos = Controller.GetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
        }
        catch (System.Exception e) {
            Debug.Log(e);
        }
    }

    public virtual void Initialize() { }

    public virtual void OnStart() { }

    public virtual void OnExecute() { }

    public virtual void OnLateExecute() { }

    public virtual void CalcCoordinates(int index, CharacterCoordinates[] data)
    {
        try
        {
            do
            {
                if (data.Length <= 0) break;

                if (index <= _plotIndex) { break; }
                int s = data.Length - (index - _plotIndex);
                if (s < 0) break;

                for (int i = s; i < data.Length; i++)
                {
                    _culling.Add(data[i]);
                }
                _plotIndex = index;
                SplineData spline = new SplineData();
                spline.CalcSpline(_culling);
                CharacterCoordinates plot = new CharacterCoordinates();
                for (int i = 0; i < spline.GetPlotNum(); i++)
                {
                    spline.GetPoint(i, out plot);
                    _plots.Add(plot);
                }
                if (_culling.Count > _PLOT_NUM)
                {
                    _culling.RemoveRange(0, _culling.Count - _PLOT_NUM);
                }
            }
            while (false);
        }
        catch {

        }
    }
    
    protected void ExecuteStepMove()
    {
    }

    public void ReceivePointFromNetwork(Vector3 pos)
    {
        CharacterCoordinates coord;
        coord.x = pos.x;
        coord.y = pos.y;
        coord.z = pos.z;

        _culling.Add(coord);

        SplineData spline = new SplineData();
        spline.CalcSpline(_culling);
        _plots.Clear();
        if (spline.GetPlotNum() > 0)
        {
            for (int i = 0; i < spline.GetPlotNum(); i++)
            {
                CharacterCoordinates plot;
                spline.GetPoint(i, out plot);
                _plots.Add(plot);
            }
        }

    }

    public void CalcRemotePosition() {
        Vector3 oldPos = Controller.GetPosition();
        Vector3 newPos = oldPos;
        if (_plots.Count > 0) {
            CharacterCoordinates coord = _plots[0];
            newPos = new Vector3(coord.x, coord.y, coord.z);
            _plots.RemoveAt(0);
        }

        if (Vector3.Distance(newPos, Controller.GetPosition()) > 0f) {
            if (newPos.magnitude <= _ZERO_VECTOR_MAG)
            {
                if (Controller.GetPosition().magnitude > _ZERO_VECTOR_MAG)
                {
                    return;
                }
            }
            Controller.SetPosition(newPos);
            Controller.SetDirection((newPos - oldPos).x);
        }
        
    }

    public void CalcLocalPosition() {
        do
        {
            _coordSendCount = (_coordSendCount + 1) % SplineData.SEND_ITERVAL;
            if (_coordSendCount != 0 && _culling.Count < _PLOT_NUM)
            {
                break;
            }
            Vector3 target = Controller.GetPosition();
            Vector3 diff = _prevPos - target;
            if (diff.sqrMagnitude > _SEND_SQR_DIFF) {
                CharacterCoordinates coord = new CharacterCoordinates(target.x, target.y, target.z);
                _culling.Add(coord);
                Controller.SendCharacterCoordinate(_plotIndex++, _culling);
                if (_culling.Count >= _PLOT_NUM) {
                    _culling.RemoveAt(0);
                }
                _prevPos = target;
            }
        }
        while (false);
    }
}
