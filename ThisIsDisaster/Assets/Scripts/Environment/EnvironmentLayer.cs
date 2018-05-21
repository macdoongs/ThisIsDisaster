using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Environment {
    public class EnvironmentLayer : MonoBehaviour {
        public static EnvironmentLayer CurrentLayer {
            private set;
            get;
        }

        public GameObject Unit;

        private void Awake()
        {
            CurrentLayer = this;
        }

        public EnvironmentUnit MakeUnit(EnvironmentModel model) {
            GameObject unitObject = Instantiate(Unit);
            unitObject.name = model.MetaInfo.Name;

            unitObject.transform.SetParent(transform);
            unitObject.transform.localPosition = Vector3.zero;
            unitObject.transform.localScale = Vector3.one;
            unitObject.transform.localRotation = Quaternion.Euler(Vector3.zero);

            EnvironmentUnit unit = unitObject.GetComponent<EnvironmentUnit>();
            unit.SetModel(model);
            model.SetUnit(unit);
            unit.LoadPrefab();

            return null;
        }
    }
}