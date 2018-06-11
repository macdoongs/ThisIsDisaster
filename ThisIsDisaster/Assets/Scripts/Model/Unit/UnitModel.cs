using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UnitModel {
    public long instanceId = 0;
    public UnitMoveControl MoveControl {
        private set;
        get;
    }

    public Shelter.ShelterModel CurrentShelter {
        private set;
        get;
    }

    public UnitModel() {
        MoveControl = new UnitMoveControl(this);
    }

    public virtual float GetSpeed() {
        return 1f;
    }

    public virtual float GetHpRate() {
        return 1f;
    }

    public virtual string GetUnitName() {
        return "";
    }

    public virtual float GetAttackDamage() {
        return 0f;
    }

    public virtual bool IsAttackTargetable() {
        return true;
    }

    public virtual void OnTakeDamage(UnitModel attacker, float damage) {
        
    }

    public virtual void MoveToTile(TileUnit destination) {
        MoveControl.MoveToTile(destination);
    }

    public virtual void SetCurrentTile(TileUnit current) {
        MoveControl.SetCurrentTile(current);
    }

    public virtual TileUnit GetCurrentTile() {
        return null;
    }

    public virtual void UpdatePosition(Vector3 pos) {

    }

    public virtual Vector3 GetCurrentPos() {
        return Vector3.one;
    }

    public virtual void OnArriedPath(TileUnit target) {

    }

    public virtual void SetCurrentTileForcely(TileUnit tile) {

    }

    public virtual bool IsInShelter() {
        return CurrentShelter != null;
    }

    public virtual void SetShelter(Shelter.ShelterModel shelter) {
        CurrentShelter = shelter;
    }

    public virtual AutoTileMovementSetter GetTileSetter() {
        return null;
    }
}

public class UnitMoveControl
{
    public delegate void OnMissTarget();
    const float _FOLLOW_PATH_UPDATE_FREQ = 1f;
    public UnitModel model;
    public TileUnit currentTile;
    public TileUnit currentDestTile;
    public AstarCalculator.PathInfo _currentPath;

    public OnMissTarget missedTarget;
    public UnitModel FollowModel {
        private set;
        get;
    }
    float _missingDistance = 0f;
    Timer _missingTimer = new Timer();

    Timer _pathUpdateTimer = new Timer();

    float Speed { get { return model.GetSpeed(); } }

    public bool IsMoving {
        private set;
        get;
    }

    public UnitMoveControl(UnitModel model) {
        this.model = model;
    }

    public void SetCurrentTile(TileUnit current) {
        this.currentTile = current;
        //current.OnEnterTile(model);
    }

    public void MoveToTile(TileUnit tile) {
        IsMoving = true;
        currentDestTile = tile;

        if (currentDestTile == currentTile && FollowModel == null) {
            StopMovement();
            return;
        }

        _currentPath = AstarCalculator.Instance.GetDestinationPath(model, currentTile, currentDestTile);
    }

    public void StopMovement() {
        IsMoving = false;
        _currentPath = null;
    }

    public void HaltMovement() {
        IsMoving = false;
    }

    public void RestartMovement() {
        IsMoving = true;
    }

    Vector3 PathStartPos() {
        return GetPathTile(_currentPath.currentPathIndex).transform.position;
    }

    Vector3 PathEndPos() {
        return GetCurrentDirectionTile().transform.position;
    }

    TileUnit GetPathTile(int index) {
        return _currentPath.path[index];
    }

    TileUnit GetCurrentDirectionTile() {
        return GetPathTile(_currentPath.currentPathIndex +1);
    }

    Vector3 GetDirectionVector() {
        return PathEndPos() - PathStartPos();
    }

    public void FollowUnit(UnitModel target, float missingDist, float missingTime = 1f) {
        this.FollowModel = target;
        _missingDistance = missingDist;
        _missingTimer.StartTimer(missingTime);
        _pathUpdateTimer.StartTimer(_FOLLOW_PATH_UPDATE_FREQ);

        MoveToTile(target.GetCurrentTile());
        
    }

    void FollowMissing() {
        Debug.Log("Missing");
        StopMovement();
        if (missedTarget != null) {
            missedTarget();
        }
        FollowModel = null;
    }

    bool CheckMissingTarget() {
        return (FollowModel.GetCurrentPos() - model.GetCurrentPos()).sqrMagnitude > _missingDistance;
    }

    public void Update() {
        if (!IsMoving) return;

        if (FollowModel != null) {
            if (CheckMissingTarget())
            {
                if (_missingTimer.started)
                {
                    if (_missingTimer.RunTimer())
                    {
                        FollowMissing();
                        return;
                    }
                }
            }
            else {
                _missingTimer.ResetTimer();
            }

            if (_pathUpdateTimer.started)
            {
                if (_pathUpdateTimer.RunTimer())
                {
                    _pathUpdateTimer.StartTimer(_FOLLOW_PATH_UPDATE_FREQ);
                    MoveToTile(FollowModel.GetCurrentTile());
                }
            }
        }

        if (_currentPath != null) {
            float movementValue = Speed * Time.deltaTime;
            if (_currentPath.currentPathIndex < _currentPath.path.Count - 1)
            {
                //var dir = GetDirectionVector();
                model.UpdatePosition(Vector3.MoveTowards(model.GetCurrentPos(), PathEndPos(), movementValue));
                //Vector3 pos = model.GetCurrentPos();
                //pos = pos + dir * movementValue;
                //model.UpdatePosition(pos);

                if (GetCurrentDirectionTile().IsArrived(model.GetCurrentPos()))
                {
                    _currentPath.currentPathIndex++;
                    //if (_currentPath.currentPathIndex == _currentPath.path.Count)
                    //{
                    //    //arrived
                    //    model.OnArriedPath(_currentPath.Destination);
                    //    StopMovement();
                    //}
                }
            }
            else if (_currentPath.currentPathIndex == _currentPath.path.Count - 1) {
                var end = _currentPath.Destination;
                //var dir = end.transform.position - GetPathTile(_currentPath.currentPathIndex - 2).transform.position;
                //Vector3 pos = model.GetCurrentPos();
                //pos = pos + dir * movementValue;
                //model.UpdatePosition(pos);
                model.UpdatePosition(Vector3.MoveTowards(model.GetCurrentPos(), GetPathTile(_currentPath.path.Count - 1).transform.position, movementValue));


                if (end.IsArrived(model.GetCurrentPos()))
                {
                    model.OnArriedPath(_currentPath.Destination);
                    if (FollowModel == null)
                        StopMovement();
                }
            }
        }
    }
}
