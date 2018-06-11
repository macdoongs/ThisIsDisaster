using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountManager : ISavedData
{
    private static AccountManager _instance = null;
    public static AccountManager Instance {
        get {
            if (_instance == null) {
                _instance = new AccountManager();
            }
            return _instance;
        }
    }

    public AccountManager() {

    }

    public bool CheckAccount() {
        bool output = FileManager.Instance.ExistFile(this);
        if (output) {
            FileManager.Instance.LoadData(this);
        }
        return output;
    }

    public void SaveAccount() {
        FileManager.Instance.SaveData(this);
    }

    public void ClearAccount() {
        FileManager.Instance.DeleteData(this);
    }

    public Dictionary<string, object> GetSavedData()
    {
        Dictionary<string, object> output = new Dictionary<string, object>();
        output.Add("accountEmail", GlobalParameters.Param.accountEmail);
        return output;
    }

    Json.WebCommunicationManager WebManager
    {
        get
        {
            return Json.WebCommunicationManager.Manager;
        }
    }

    public void LoadData(Dictionary<string, object> data)
    {
        string accountEmail = "";
        FileManager.TryGetValue(data, "accountEmail", ref accountEmail);

        if (string.IsNullOrEmpty(accountEmail)) return;
        //서버와의 통신을 통해 계정 정보를 불러와야 합니다
        GlobalParameters.Param.accountEmail = accountEmail;
        WebManager.SendRequest(Json.RequestMethod.GET, "user?email=" + accountEmail, "");
    }

    public string GetPath()
    {
        return "accountInfo";
    }
}
