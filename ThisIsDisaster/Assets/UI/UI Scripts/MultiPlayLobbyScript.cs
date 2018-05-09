using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using LitJson;


public class MultiPlayLobbyScript : MonoBehaviour {


    private string url = "http://api.thisisdisaster.com/game/multiplay/lobby";
    private string result = null;


    int resultCode = 0;
    string resultMsg;


    public Text PlayerName;
    public Text PlayerLevel;
    public Text PlayerExp;

    public Text[][] User_Text;
    public Text[] User1;
    public Text[] User2;
    public Text[] User3;
    public Text[] User4;


    public string name;
    public int level;
    public float exp;
    public string role;

    User[] User_info;
    User user1_info;
    User user2_info;
    User user3_info;
    User user4_info;

    public class User
    {
        public string name;
        public int level;        
        public string role;

        public User()
        {
            name = "";
            level = 0;
            role = "";
        }
    }


    // Use this for initialization
    void Start()
    {
        User_Text = new Text[][] { User1, User2, User3, User4};
        User_info = new User[] {user1_info, user2_info, user3_info, user4_info };

        GetHttpRequest();
        GetDataFromJson();
        if (resultCode == 200)
            SetUI();
    }

    // Update is called once per frame
    void Update()
    {
  //      if (resultCode == 200)
  //          SetUI();
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
        resultMsg = jsonResult["result_msg"].ToString();

        JsonData result_data = jsonResult["result_data"];

        JsonData UserList = result_data["user_list"];

        JsonData User1_data = UserList[0];
        JsonData User2_data = UserList[1];
        JsonData User3_data = UserList[2];

        

        name = result_data["name"].ToString();
        level = int.Parse(result_data["level"].ToString());
        role = result_data["role"].ToString();

        User_info[0] = new User();

        User_info[0].name = name;
        User_info[0].level = level;
        User_info[0].role = role;

        for(int i = 0; i < 3; i++)
        {
            string tempName = UserList[i]["name"].ToString();

            if (!tempName.Equals(""))
            {
                User_info[i+1] = new User();
                User_info[i+1].level = int.Parse(UserList[i]["level"].ToString());
                User_info[i+1].name = UserList[i]["name"].ToString();
                User_info[i+1].role = UserList[i]["role"].ToString();
            }
        }
    }

    public void SetUI()
    {
        PlayerName.text = name;
        PlayerLevel.text = level.ToString();
        //    PlayerExp.text = exp.ToString();

        User_Text[0][0].text = name;
        User_Text[0][1].text = level.ToString();


        for (int i = 1; i < 4; i++)
        {
            User_Text[i][0].text = User_info[i].name;
            User_Text[i][1].text = User_info[i].level.ToString();
        }
    }
}
