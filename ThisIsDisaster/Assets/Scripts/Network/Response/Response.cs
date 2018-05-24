using System;
using UnityEngine;

namespace Json
{
    
    [Serializable]
    public class Response
    {
        public string result_code;
        public string result_msg;
        public string response_type;

        public Response() {
            response_type = typeof(Response).ToString();
        }

        public bool GetResult()
        {
            int resultValue = int.Parse(result_code.Trim());
            return resultValue >= 200 && resultValue < 300;
        }
        
        public static Response CreateFromJson(string jsonString) {
            return JsonUtility.FromJson<Response>(jsonString);
        }
    }
    
    #region UserResponse
    [Serializable]
    public class User
    {
        public string nickname;
        public string score;
        public string level;
        public string gold;
    }

    [Serializable]
    public class UserResponse : Response
    {
        public User result_data;

        public UserResponse() {
            response_type = typeof(UserResponse).ToString();
        }
        
        public static new UserResponse CreateFromJson(string jsonString) {
            return JsonUtility.FromJson<UserResponse>(jsonString);
        }
    }
    #endregion
}