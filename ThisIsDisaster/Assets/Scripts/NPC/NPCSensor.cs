using UnityEngine;
using System.Collections;
using System;

namespace NPC
{
    public class NPCSensor : MonoBehaviour, IObserver
    {
        public NPCUnit Unit;
        bool _init = false;
        CharacterModel _player = null;
        Timer _checkTimer = new Timer();
        float _checkFreq = 1f;
        public float senseRange = 3f;
        //Collider2D col;
        // Use this for initialization
        void Start()
        {
            try
            {
                Init();
            }
            catch {
                ObserveNotices();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (_checkTimer.started) {
                if (_checkTimer.RunTimer()) {
                    _checkTimer.StartTimer(_checkFreq);
                    //check
                    if (Check()) {
                        Unit.OnSensePlayer(_player.GetPlayerModel());
                    }
                }
            }
        }

        public bool Check() {
            Vector3 target = _player.transform.position;
            Vector3 current = transform.position;
            Vector3 diff = target - current;
            diff.z = 0f;
            return (diff).sqrMagnitude <= senseRange;
        }

        private void OnDestroy()
        {
            RemoveNotices();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //PlayerSensor ps = collision.GetComponent<PlayerSensor>();
            //if (!ps) return;
            //Unit.OnSensePlayer(ps);
        }

        public void OnNotice(string notice, params object[] param)
        {
            if (notice == NoticeName.LocalPlayerGenerated)
            {
                Init();
                RemoveNotices();
            }
        }

        public void ObserveNotices()
        {
            Notice.Instance.Observe(NoticeName.LocalPlayerGenerated, this);
        }

        public void RemoveNotices()
        {
            Notice.Instance.Remove(NoticeName.LocalPlayerGenerated, this);
        }

        void Init()
        {
            _player = GameManager.CurrentGameManager.GetLocalPlayer().GetComponent<CharacterModel>();
            _init = true;
            _checkTimer.StartTimer(_checkFreq);
        }
    }
}