using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Json
{
    [Serializable]
    public class SessionNode {
        public string email = "";
        public int id = -1;
        public string ip = "";
        public int level = -1;
        public string nickname = "";
        public string role = "";
    }

    [Serializable]
    public class SessionData {
        public string email = "";
        public string stage = "";

        public SessionNode[] user_list;
    }

    [System.Serializable]
    public class MultiPlayLobby : Response
    {
        public SessionData result_data;

        public MultiPlayLobby() {
            result_type = typeof(MultiPlayLobby).ToString();
        }

        public static new MultiPlayLobby CreateFromJson(string jsonString) {
            return JsonUtility.FromJson<MultiPlayLobby>(jsonString);
        }
    }
}
