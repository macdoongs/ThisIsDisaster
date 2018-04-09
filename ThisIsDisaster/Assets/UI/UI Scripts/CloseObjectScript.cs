using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseObjectScript : MonoBehaviour {

    public GameObject closeObject;

    public void CloseObject()
    {
        closeObject.SetActive(false);
    }



}
