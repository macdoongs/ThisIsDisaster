using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Json;

public class JsonTest : MonoBehaviour
{

    // Use this for initialization 
    void Start()
    {

        //var data = new UserResponse() 
        //{ 
        //    result_code = 200, 
        //    result_msg = "testing2", 
        //    result_type = typeof(UserResponse).ToString(), 
        //    result_data = new User() { 
        //        nickname = "testChar", 
        //        score = 10, 
        //        level = 1, 
        //        gold = 0 
        //    } 
        //}; 

        //var json = JsonUtility.ToJson(data); 

        ////WebCommunicationManager.Manager.OnReceiveGETMessage(json); 

        //var failData = new UserResponse() 
        //{ 
        //    result_code = 400, 
        //    result_msg = "testing2", 

        //    result_data = new User() 
        //    { 
        //        nickname = "testChar", 
        //        score = 10, 
        //        level = 1, 
        //        gold = 0 
        //    } 
        //}; 

        //var failJson = JsonUtility.ToJson(failData); 

        //WebCommunicationManager.Manager.OnReceiveGETMessage(failJson); 
        string jsonBody = @"{ 
     “email”: “test@test.com”, 
    “nickname”: “test”, 
    “password”: “assdfa” 
}";

        string deleteBody = @"{ 
    “title”: “test”, 
    “content”: “hello world” 
} 
";



        //WebCommunicationManager.Manager.SendRequest(RequestMethod.GET, "user"); 
        //WebCommunicationManager.Manager.SendRequest(RequestMethod.POST, "user", jsonBody); 
        //WebCommunicationManager.Manager.SendRequest(RequestMethod.DELETE, "user", jsonBody); 
        //WebCommunicationManager.Manager.SendRequest(RequestMethod.PUT, "notice/" + 0, deleteBody); 
        Debug.Log("SaveLog");
        //GameLogManager.Instance.SaveLog("Testing Log"); 


    }

    // Update is called once per frame 
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            Debug.Log("GetLog");
            GameLogManager.Instance.GetLog();
        }
    }
}