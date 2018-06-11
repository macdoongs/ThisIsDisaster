using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Json;

public class JsonTest : MonoBehaviour {

    // Use this for initialization
    void Start() {
        

        //WebCommunicationManager.Manager.SendRequest(RequestMethod.GET, "user");
        //WebCommunicationManager.Manager.SendRequest(RequestMethod.POST, "user", jsonBody);
        //WebCommunicationManager.Manager.SendRequest(RequestMethod.DELETE, "user", jsonBody);
        //WebCommunicationManager.Manager.SendRequest(RequestMethod.PUT, "notice/" + 0, deleteBody);
        Debug.Log("SaveLog");
        //GameLogManager.Instance.SaveLog("Testing Log");

        
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            Debug.Log("GetLog");
            GameLogManager.Instance.GetLog();   
        }
	}
}
