using System;
using UnityEngine;

namespace Json
{
    
    [Serializable]
    public class Response
    {
        public int result_code;
        public string result_msg;
        public string result_type;

        public Response() {
            result_type = typeof(Response).ToString();
        }

        public bool GetResult()
        {
			return result_code >= 200 && result_code < 300;
        }
        
        public static Response CreateFromJson(string jsonString) {
            return JsonUtility.FromJson<Response>(jsonString);
        }
    }
    
    #region UserResponse
    [Serializable]
    public class User
    {
        public int id;
        public string email;
        public string nickname;
        public int score;
        public int level;
        public int exp;
        public int gold;
        public string ip;
        public string role;
    }

    [Serializable]
    public class UserResponse : Response
    {
        public User result_data;

        public UserResponse() {
            result_type = "User";
        }
        
        public static new UserResponse CreateFromJson(string jsonString) {
            return JsonUtility.FromJson<UserResponse>(jsonString);
        }
    }
    #endregion
}