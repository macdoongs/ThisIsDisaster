
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class LobbySceneScript : MonoBehaviour {

    private string url = "http://api.thisisdisaster.com/user/lobby";
    private string result = null;

    public Text PlayerName;
    public Text PlayerLevel;
    public Text PlayerExp;


    public string name;
    public int level;
    public float exp;
    public long gold;

    int resultCode = 0;
    string resultMsg;

    // Use this for initialization
    void Start () {
        GetHttpRequest();
        GetDataFromJson();
        if(resultCode == 200)
            SetUI();
    }
	
	// Update is called once per frame
	void Update () {
        if (resultCode == 200)
        SetUI();
    }

    public void GetHttpRequest()
    {
        WebClient webClient = new WebClient();
        Stream stream = webClient.OpenRead(url);
        result = new StreamReader(stream).ReadToEnd();
    }

    public void GetDataFromJson()
    {
        JsonData jsonResult = JsonMapper.ToObject(result);
        resultCode = int.Parse(jsonResult["result_code"].ToString());

        JsonData result_data = jsonResult["result_data"];
        resultMsg = jsonResult["result_msg"].ToString();

        name = result_data["nickname"].ToString();
        level = int.Parse(result_data["level"].ToString());
        exp = float.Parse(result_data["exp"].ToString());
        gold = int.Parse(result_data["gold"].ToString());
    }

    public void SetUI()
    {
        PlayerName.text = name;
        PlayerLevel.text = level.ToString();
        PlayerExp.text = exp.ToString();
    }
}
