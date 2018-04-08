using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenObjectSctipt : MonoBehaviour {

    public GameObject openObject;

    public void OpenObject()
    {
        openObject.SetActive(true);
    }
}
