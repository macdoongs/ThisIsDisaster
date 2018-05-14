using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPC {
    public class NPCScriptBase {
        public NPCModel Model { get; private set; }
        public NPCUnit Unit { get { return Model.Unit; } }

        protected bool _wanderDestInit = false;
        protected Vector3 _wanderDest = Vector3.zero;
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

            if (_wanderTimer.started) {
                if (_wanderTimer.RunTimer()) {
                    _wanderDestInit = false;
                }
                else
                    return;
            }
            if (!_wanderDestInit)
            {
                _wanderDestInit = true;
                _wanderDest = Model.GetRandomMovement();

            }

            Vector3 current = Unit.transform.position;
            if (_wanderDest == current)
            {
                _wanderTimer.StartTimer(UnityEngine.Random.Range(2f, 5f));
            }
            else {
                //Vector3 direction = (_wanderDest - current).normalized;

                //Unit.transform.Translate(direction * Model.MetaInfo.GetSpeed() * Time.deltaTime);
                Unit.transform.position = Vector3.MoveTowards(current, _wanderDest, Model.MetaInfo.GetSpeed() * Time.deltaTime);
            }
        }

        public virtual void MoveExecute() { }
        public virtual void BattleExectue() { }
        public virtual void DetectAtion() { }
        public virtual void ArriveAction() { }
        public virtual void OnDefeated() { }
        public virtual void OnVictoried() { }
    }
}