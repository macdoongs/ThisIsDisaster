using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NPC {
    public enum NPCState {
        Generated,
        Execute,
        Destroied,
        None//dummy and end Index
    }

    public enum NPCExectueState {
        Wander,
        Movement,
        Battle,
        None//dummy and end Index
    }

    public class NPCModel : UnitModel {
        public NPCUnit Unit {
            get; private set;
        }

        public NPCTypeInfo MetaInfo {
            get; private set;
        }

        public NPCScriptBase Script {
            get; private set;
        }

        private NPCState _state = NPCState.None;
        public NPCState State { get { return _state; } }

        private NPCExectueState _executeState = NPCExectueState.None;
        public NPCExectueState ExecuteState { get { return _executeState; } }

        #region Stats
        public float MaxHP
        {
            get
            {
                float output = MetaInfo.GetMaxHp();
                if (output == 0f) { output = 10f; }
                return output;
            }
        }
        public float CurrentHp = 0f;

        public float Defense { get{ return MetaInfo.GetDefense(); } }
        #endregion

        /// <summary>
        /// Call After MetaInfo Set
        /// </summary>
        void InvokeScript() {
            string scriptSrc = MetaInfo.Script;
            if (string.IsNullOrEmpty(scriptSrc)) {
                scriptSrc = "NPCScriptBase";
            }
            scriptSrc = "NPC." + scriptSrc;
            NPCScriptBase script = (NPCScriptBase)System.Activator.CreateInstance(System.Type.GetType(scriptSrc));
            Script = script;
            Script.SetModel(this);
            //init
        }

        public void SetMetaInfo(NPCTypeInfo info) {
            MetaInfo = info;
            InvokeScript();
        }

        public void SetUnit(NPCUnit unit) {
            Unit = unit;
        }

        public void Init() {
            _state = NPCState.Generated;

            CurrentHp = MaxHP;
        }

        public void OnGenerated() {
            Script.OnGenerated();
        }

        public void OnExecute() {
            if (State != NPCState.Execute) return;

            Script.OnExecute();
            switch (_executeState) {
                case NPCExectueState.None:
                    _executeState = NPCExectueState.Wander;
                    Unit.SetSensing(true);
                    break;
                case NPCExectueState.Wander:
                    WanderExectue();
                    break;
                case NPCExectueState.Movement:
                    MoveExecute();
                    break;
                case NPCExectueState.Battle:
                    BattleExecute();
                    break;
            }
        }

        public void OnDestroied() {

        }

        public void WanderExectue() {
            Script.WanderExectue();
        }

        public void MoveExecute() {
            Script.MoveExecute();
        }

        public void BattleExecute() {
            Script.BattleExectue();
        }
        
        public void DetectAction() {
            Script.DetectAtion();
        }

        public void ArriveAction() {
            Script.ArriveAction();
        }

        public void OnDefeated() {
            Debug.Log(GetUnitName() + " died");
            Script.OnDefeated();
            _state = NPCState.Destroied;
        }

        public void OnVictoried() {
            Script.OnVictoried();
            _executeState = NPCExectueState.Wander;
            Unit.SetSensing(true);
        }

        public void Update() {
            switch (_state) {
                case NPCState.Generated:
                    OnGenerated();
                    _state = NPCState.Execute;
                    break;
                case NPCState.Execute:
                    OnExecute();
                    break;
                case NPCState.Destroied:
                    OnDestroied();
                    _state = NPCState.None;
                    break;
            }
        }

        /// <summary>
        /// For dev
        /// </summary>
        /// <returns></returns>
        public Vector3 GetRandomMovement() {
            float randomDist = UnityEngine.Random.value * 10f;
            float randomFactor = UnityEngine.Random.value;
            Vector3 distVector = new Vector3(
                UnityEngine.Random.value >= 0.5f ? 1f : -1f * randomFactor,
                UnityEngine.Random.value >= 0.5f ? 1f : -1f * (1 - randomFactor),
                0f);
            distVector *= randomDist;
            return Unit.transform.position + distVector;
        }

        public override void OnTakeDamage(UnitModel attacker, float damage) {
            Debug.Log(GetUnitName() + " Attacked By " + attacker.GetUnitName());
            CurrentHp -= damage;
            if (CurrentHp <= 0f) {
                CurrentHp = 0f;
                OnDefeated();
            }
        }

        public void OnDetectedTarget(UnitModel target) {
            Unit.SetSensing(false);
            _executeState = NPCExectueState.Movement;
            //move to target
        }

        public override string GetUnitName()
        {
            return MetaInfo.Name;
        }

        public override float GetHpRate()
        {
            return Mathf.Clamp(CurrentHp / MaxHP, 0f, 1f);
        }
    }
    
}
