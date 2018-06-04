using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalParamSaveTest : MonoBehaviour {

    private void Awake()
    {
        if (GlobalGameManager.Instance != null) {
            Debug.Log("Init");
        }
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnEnterAccountId(string text) {
        int parsed = -1;
        if (int.TryParse(text, out parsed)) {
            GlobalParameters.Param.accountId = parsed;
        }
    }

    public void OnEnterAccountName(string text) {
        if (string.IsNullOrEmpty(text)) return;
        GlobalParameters.Param.accountName = text;
    }

    public void OnClickSave() {
        if (FileManager.Instance.SaveData(GlobalParameters.Param))
        {
            Debug.LogError("Saved Data : " + FileManager.Instance.GetPath(GlobalParameters.Param, ".dat"));
        }
        else
            Debug.LogError("Failed To Save Data");
        //FileManager.Instance.SaveFileData()
    }
}
