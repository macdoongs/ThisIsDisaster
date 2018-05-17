using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    public enum UnitDirection {
        LEFT,
        RIGHT
    }

    public class NPCUnit : MonoBehaviour
    {
        [System.Serializable]
        public class NPCAttackController {
            public AttackSender Sender;
            public float AttackDelay;

            public bool isDebuggingEnabled = false;
            public SpriteRenderer _debugRenderer;
            Timer _attackDelayTimer = new Timer();

            public bool IsAttackable() {
                return !_attackDelayTimer.started;
            }

            public void StartAttack() {
                Sender.OnAttack();
                _attackDelayTimer.StartTimer(AttackDelay);
            }

            public void Update() {
                if (_attackDelayTimer.started) {
                    if (!_attackDelayTimer.RunTimer()) return;
                }
                if (isDebuggingEnabled && _debugRenderer) {
                    if (Sender.IsAttacking())
                    {
                        if (!_debugRenderer.enabled) _debugRenderer.enabled = true;
                    }
                    else {
                        if (_debugRenderer.enabled) _debugRenderer.enabled = false;
                    }
                }
            }
        }

        public NPCModel Model
        {
            get; private set;
        }

        #region Inspector
        public Transform FlipPivot;
        public GameObject animTarget;
        public Animator animator;

        public NPCSensor Sensor;
        public AttackSender AttackSender;
        public AttackReceiver AttackReceiver;
        public RenderLayerChanger RenderLayerChanger;
        public NPCAttackController AttackControl;

        [Header("UI")]
        public UnityEngine.UI.Slider hpSlider;
        #endregion

        Vector3 oldPos = Vector3.zero;

        public void SetModel(NPCModel model) {
            Model = model;
            //onsetevent

            AttackSender.SetOwner(model);
            AttackReceiver.SetOwner(model);
        }
        
        // Use this for initialization
        void Start()
        {
            oldPos = transform.position;
            SetSensing(false);
        }

        // Update is called once per frame
        void Update()
        {
            AttackControl.Update();

        }

        public void OnStartAttack() {
            AttackControl.StartAttack();
        }

        public void SetSensing(bool state) {
            Sensor.gameObject.SetActive(state);
        }

        public void OnDestroied() {
            //play dead animation if exist
            AnimatorUtil.SetTrigger(animator, "Dead");
        }

        public GameObject LoadPrefab() {
            if (Model == null) return null;
            GameObject loaded = Prefab.LoadPrefab("NPCs/" + Model.MetaInfo.Prefab);
            animator = loaded.GetComponent<Animator>();
            animTarget = loaded;

            loaded.transform.SetParent(FlipPivot);
            loaded.transform.localPosition = Vector3.zero;
            loaded.transform.localScale = Vector3.one;
            loaded.transform.localRotation = Quaternion.Euler(Vector3.zero);
            return loaded;
        }

        private void LateUpdate()
        {
            Vector3 currentPos = transform.position;
            
            if (currentPos.x > oldPos.x) {
                //set left
                SetDirection(UnitDirection.LEFT);
            }
            else if (currentPos.x < oldPos.x) {
                //set right
                SetDirection(UnitDirection.RIGHT);
            }
            oldPos = currentPos;


            if (hpSlider != null)
            {
                hpSlider.value = Model.GetHpRate();
            }
        }

        public void SetDirection(UnitDirection dir) {
            Vector3 scale = Vector3.one;
            if (dir == UnitDirection.LEFT)
            {
                scale.x *= -1;
            }

            FlipPivot.transform.localScale = scale;

        }

        public void OnSensePlayer(PlayerSensor sensor) {
            Debug.Log(string.Format("{0} {1} Sensored {2}", Model.GetUnitName(), Model.instanceId, sensor.Owner.GetUnitName()));
            Model.OnDetectedTarget(sensor.Owner);
        }
        
    }
}