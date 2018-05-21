using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLayer : MonoBehaviour {
    public static ItemLayer CurrentLayer {
        private set;
        get;
    }

    public GameObject DropItemUnit;
    private void Awake()
    {
        CurrentLayer = this;
    }

    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update () {
	}

    public DropItem MakeDropItem(ItemModel item) {
        GameObject copy = Instantiate(DropItemUnit);
        copy.transform.SetParent(transform);
        copy.transform.localPosition = Vector3.zero;
        copy.transform.localRotation = Quaternion.Euler(Vector3.zero);
        copy.transform.localScale = Vector3.one;
        DropItem output = copy.GetComponent<DropItem>();

        output.SetModel(item);

        return output;
    }
}
