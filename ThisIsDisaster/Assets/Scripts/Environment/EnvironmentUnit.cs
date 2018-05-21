using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Environment
{
    public class EnvironmentUnit : MonoBehaviour
    {
        public EnvironmentModel Model {
            private set;
            get;
        }

        public Transform FlipPivot;
        public Animator animator = null;
        public GameObject animTarget = null;

        public AutoTileMovementSetter TileSetter;

        public void SetModel(EnvironmentModel model) {
            Model = model;

            TileSetter.SetChangeAction(model.SetCurrentTile);
        }

        public GameObject LoadPrefab() {
            if (Model == null) return null;
            GameObject loaded = Prefab.LoadPrefab("Environments/" + Model.MetaInfo.Prefab);

            animator = loaded.GetComponent<Animator>();
            animTarget = loaded;

            loaded.transform.SetParent(FlipPivot);
            loaded.transform.localPosition = Vector3.zero;
            loaded.transform.localScale = Vector3.one;
            loaded.transform.localRotation = Quaternion.Euler(Vector3.zero);
            return loaded;
        }
        
    }
}
