using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Environment
{
    public class TreeScript : EnvironmentScriptBase
    {
        string _burnnedSprite = "Sprites/GoldenSkullStudios/2D_Iso_FoliagePack/Sprites/GSS_Foliage_Forest_deadTree (2)";
        string _burnnedEffect = "Environments/TreeBurn";

        private Timer _layTimer = new Timer();
        const float _LAY_TIME = 2f;

        private Timer _brokenTimer = new Timer();
        const float _BROKEN_MAKE_WOODTIMER = 20f;
        private float _layRotation = 0f;

        GameObject _laySprite = null;

        private bool _burnned = false;
        private bool _broken = false;

        public void OnBurn() {
            if (_burnned || _broken) return;
            _burnned = true;
            var renderer= Model.Unit.animTarget.GetComponent<SpriteRenderer>();
            renderer.sprite = Resources.Load<Sprite>(_burnnedSprite);

            GameObject effect = Prefab.LoadPrefab(_burnnedEffect);
            effect.transform.position = Model.Unit.animTarget.transform.position;
            effect.transform.localScale = Vector3.one;
            effect.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            _brokenTimer.StopTimer();
        }

        public bool IsBronkenOrBurnned() {
            return _burnned || _broken;
        }

        public void OnBroken()
        {
            if (_burnned || _broken) return;
            _broken = true;

            _laySprite = Model.Unit.animTarget;
            //_laySprite.sprite = Model.Unit.animTarget.GetComponent<SpriteRenderer>().sprite;
            //_laySprite.transform.localPosition = Model.Unit.animTarget.transform.position;
            //_laySprite.transform.localRotation = Quaternion.Euler(Vector3.zero);
            //_laySprite.transform.localScale = Vector3.one;
            //_laySprite.gameObject.SetActive(true);

            //Model.Unit.animTarget.SetActive(false);

            _brokenTimer.StartTimer(_BROKEN_MAKE_WOODTIMER);
            _layTimer.StartTimer(_LAY_TIME);
            _layRotation = UnityEngine.Random.value <= 0.5f ? -90f : 90f;
        }

        public void Update() {
            if (_layTimer.started) {
                float rate = _layTimer.Rate;
                if (_layTimer.RunTimer()) {
                    rate = 1f;
                }
                float rot = Mathf.Lerp(0f, _layRotation, rate);
                _laySprite.transform.localRotation = Quaternion.Euler(0f, 0f, rot);
            }

            if (_brokenTimer.started) {
                if (_brokenTimer.RunTimer()) {
                    //_laySprite.gameObject.SetActive(false);
                }
            }
        }
    }
}
