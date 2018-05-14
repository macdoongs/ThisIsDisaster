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
        public NPCModel Model
        {
            get; private set;
        }

        #region Inspector
        public GameObject animTarget;
        public Animator animator;
        #endregion

        public void SetModel(NPCModel model) {
            Model = model;
            //onsetevent
        }


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

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

            loaded.transform.SetParent(transform);
            loaded.transform.localPosition = Vector3.zero;
            loaded.transform.localScale = Vector3.one;
            loaded.transform.localRotation = Quaternion.Euler(Vector3.zero);
            return loaded;
        }
    }
}