using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPC {
    public class NPCScriptBase {
        public NPCModel Model { get; private set; }
        public NPCUnit Unit { get { return Model.Unit; } }

        protected bool _wanderDestInit = false;
        protected TileUnit _wanderDest = null;
        protected Timer _wanderTimer = new Timer();

        public void SetModel(NPCModel model) {
            Model = model;
            Initialize();
        }

        public virtual void Initialize() {

        }

        public virtual void OnGenerated() { }
        public virtual void OnExecute() { }
        public virtual void OnDestroied() { }

        public virtual void WanderExectue() {

            if (_wanderTimer.started)
            {
                if (_wanderTimer.RunTimer())
                {
                    _wanderDestInit = false;
                }
                else
                    return;
            }
            if (!_wanderDestInit)
            {
                _wanderDestInit = true;
                _wanderDest = Model.GetRandomMovementTile();

                Model.MoveToTile(_wanderDest);
            }
            
        }

        public virtual void StopWandering() {
            _wanderTimer.StopTimer();
            _wanderDestInit = false;
            _wanderDest = null;
        }

        public virtual void MoveExecute() {
        }

        public virtual void BattleExectue() { }
        public virtual void DetectAtion() { }
        public virtual void ArriveAction() { }
        public virtual void OnDefeated() { }
        public virtual void OnVictoried() { }

        public virtual void OnWanderDestArrived() {
            _wanderTimer.StartTimer(UnityEngine.Random.Range(5f, 10f));
        }

        public virtual void OnStartAttack() {
            Model.Unit.OnStartAttack();
        }
    }
}