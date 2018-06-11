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

        public static Response CreateDummy() {
            Response re = new Response
            {
                result_code = 200,
                result_msg = "Success",
                result_type = "Response"
            };

            return re;
        }
        public static string CreateDummyString() {
            var js = JsonUtility.ToJson(CreateDummy());
            return js;
        }
    }
    
    #region UserResponse
    [Serializable]
    public class User
    {
        public string email;
        public string nickname;
        public string score;
        public string level;
        public string gold;
        public string exp;
    }

    [Serializable]
    public class UserResponse : Response
    {
        public User result_data;

        public UserResponse() {
            result_type = typeof(UserResponse).ToString();
        }
        
        public static new UserResponse CreateFromJson(string jsonString) {
            return JsonUtility.FromJson<UserResponse>(jsonString);
        }
    }
    #endregion
}