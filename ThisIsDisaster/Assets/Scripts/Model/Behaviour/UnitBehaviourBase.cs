using UnityEngine;
using System.Collections;

public class UnitBehaviourBase : MonoBehaviour
{
    public UnitControllerBase Controller = null;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
<<<<<<< HEAD

=======
        try
        {
            //CalcRemotePosition();
            ExecuteStepMove();
            if (!IsRemoteCharacter)
                CalcLocalPosition();
        }
        catch (System.Exception e) {
            Debug.Log(e);
        }
>>>>>>> d97cb236d0e2e96f1c2a3d235d7ce60cc462394e
    }

    public virtual void Initialize() { }

    public virtual void OnStart() { }

    public virtual void OnExecute() { }

    public virtual void OnLateExecute() { }
    
<<<<<<< HEAD
=======
    protected void ExecuteStepMove()
    {
        return;
        Vector3 pos = Controller.GetPosition();
        if (_plots.Count > 0)
        {
            CharacterCoordinates coord = _plots[0];
            pos = new Vector3(coord.x, coord.y, coord.z);
            _plots.RemoveAt(0);
        }

        if (Vector3.Distance(pos, Controller.GetPosition()) > 0f)
        {
            Controller.SetPosition(pos);
        }

    }

    public void ReceivePointFromNetwork(Vector3 pos)
    {
        CharacterCoordinates coord;
        coord.x = pos.x;
        coord.y = pos.y;
        coord.z = pos.z;

        _culling.Add(coord);

        SplineData spline = new SplineData();
        spline.CalcSpline(_culling, 4);
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
        Vector3 newPos = Controller.GetPosition();
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
        }
        
    }

    public void CalcLocalPosition() {
        //check network state
        //currently considered as Connected
        do
        {
            _coordSendCount = (_coordSendCount + 1) % SplineData.SEND_ITERVAL;
            if (_coordSendCount != 0)
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
>>>>>>> d97cb236d0e2e96f1c2a3d235d7ce60cc462394e
}
