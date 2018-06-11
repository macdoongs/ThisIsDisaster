
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
        StartCoroutine("LoadUser");
    }
	


	// Update is called once per frame
	void Update ()
    {

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

    IEnumerator LoadUser()
    {
        while (true)
        {
            string email = GlobalParameters.Param.accountEmail;

            WebManager.SendRequest(Json.RequestMethod.GET, "user?email=" + email, "");

            yield return new WaitForSeconds(30);
            Debug.Log("Sending");
        }
    }
    
    void SetUserData(Json.UserResponse data) {
        PlayerName.text = GlobalGameManager.Param.accountName;
        PlayerLevel.text = data.result_data.level.ToString();
        PlayerExp.text = data.result_data.exp.ToString();
        float rate = data.result_data.exp * 0.01f;
        PlayerExpFill.fillAmount = rate;
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
