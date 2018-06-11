
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class LobbySceneScript : MonoBehaviour, IObserver {

    //private string url = "http://api.thisisdisaster.com/user";
    private string result = null;

    public Text PlayerName;
    public Text PlayerLevel;
    public Text PlayerExp;
    public Image PlayerExpFill;


    public string nickname = "DefaultName";
    public int level = 1;
    public int exp = 0;
    public int gold = 0;

    int resultCode = 0;
    string resultMsg;

    // Use this for initialization
    void Start ()
    {
        LoadUser();
        SetUI();
        InvokeRepeating("SetUI", 0f, 5.0f);
    }
	
	// Update is called once per frame
	void Update ()
    {
        SetUI();
    }

    Json.WebCommunicationManager WebManager
    {
        get
        {
            return Json.WebCommunicationManager.Manager;
        }
    }

    private void OnEnable()
    {
        ObserveNotices();
    }

    private void OnDisable()
    {
        RemoveNotices();
    }

    public void LoadUser()
    {
        string email = GlobalParameters.Param.accountEmail;

        WebManager.SendRequest(Json.RequestMethod.GET, "user?email=" + email, "");
    }
    
    public void SetUI()
    {

        PlayerName.text = GlobalParameters.Param.accountName;
        //PlayerLevel.text = GlobalParameters.Param.accountLevel.ToString();
        //PlayerExp.text = GlobalParameters.Param.accountExp.ToString();
    }

    void SetUserData(Json.UserResponse data) {
        PlayerLevel.text = data.result_data.level.ToString();
        PlayerExp.text = data.result_data.exp;

        int value = 0;
        if (int.TryParse(data.result_data.exp, out value)) {
            float rate = value * 0.01f;
            PlayerExpFill.fillAmount = rate;
        }
    }

    public void OnNotice(string notice, params object[] param)
    {
        if (NoticeName.OnReceiveUserData == notice) {
            Json.UserResponse ur = param[0] as Json.UserResponse;

            SetUserData(ur);
        }
    }

    public void ObserveNotices()
    {
        Notice.Instance.Observe(NoticeName.OnReceiveUserData, this);
    }

    public void RemoveNotices()
    {
        Notice.Instance.Remove(NoticeName.OnReceiveUserData, this);
    }
}
