using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPC;

public class NPCLayer : MonoBehaviour {
    public static NPCLayer CurrentLayer {
        private set;
        get;
    }

    private void Awake()
    {
        CurrentLayer = this;
    }

    public NPCUnit MakeUnit(NPCModel model) {
        GameObject unitObject = new GameObject(model.MetaInfo.Name);
        unitObject.transform.SetParent(transform);
        unitObject.transform.localPosition = Vector3.zero;
        unitObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
        unitObject.transform.localScale = Vector3.one;

        NPCUnit unit = unitObject.AddComponent<NPCUnit>();
        unit.SetModel(model);
        model.SetUnit(unit);
        unit.LoadPrefab();

        return unit;
    }

}
