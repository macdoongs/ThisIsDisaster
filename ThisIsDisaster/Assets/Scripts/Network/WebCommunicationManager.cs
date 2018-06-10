using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Json;
using System.Collections;
using UnityEngine.Networking;

namespace Json
{
    public enum RequestMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }
    public class WebCommunicationManager : MonoBehaviour
    {

        const string url = "http://api.thisisdisaster.com/";

        public delegate void JsonResponseAction(string message, Response response);

        private static WebCommunicationManager _manager = null;
        public static WebCommunicationManager Manager {
            get {
                return _manager;
            }
        }

        private Dictionary<string, JsonResponseAction> _recieveActions = new Dictionary<string, JsonResponseAction>();

        private bool _requestErrorOccured = false;

        private void Awake()
        {
            if (Manager != null && Manager.gameObject != null) {
                Destroy(gameObject);return;
            }
            _manager = this;
            DontDestroyOnLoad(gameObject);
        }

        public WebCommunicationManager() {
            _recieveActions.Add(typeof(Response).ToString(), ReceiveResponse);
            _recieveActions.Add("User", ReceiveUserResponse);
        }

        #region Request
        public void SendRequest(RequestMethod method, string api, string jsonBody = "")
        {
            switch (method)
            {
                case RequestMethod.GET:
                    StartCoroutine(GetRequest(api));
                    break;
                case RequestMethod.POST:
                    StartCoroutine(PostRequest(api, jsonBody));
                    break;
                case RequestMethod.DELETE:
                    StartCoroutine(DeleteRequest(api, jsonBody));
                    break;
                case RequestMethod.PUT:
                    StartCoroutine(PutRequest(api, jsonBody));
                    break;
            }
        }

        IEnumerator GetRequest(string api)
        {
            UnityWebRequest request = UnityWebRequest.Get(url + api);
            yield return request.SendWebRequest();
            string msg = "";
            if (request.isNetworkError)
            {
                Debug.LogError("NetworkError in Get Request : " + api);
            }
            else
            {
                msg = request.downloadHandler.text;
                Debug.Log(msg);
                if (request.responseCode == 200)
                {
                    //Debug.Log(request.downloadHandler.text);
                }
                else if (request.responseCode == 401)
                {
                    //unauthorized

                    _requestErrorOccured = true;
                }
                else
                {
                    //error
                    _requestErrorOccured = true;
                }
            }

            if (!_requestErrorOccured)
            {
                yield return null;
                OnReceiveGETMessage(msg);
            }
        }

        IEnumerator PostRequest(string api, string jsonBody)
        {
            var request = new UnityWebRequest(url + api, "POST");
            //var request = UnityWebRequest.Post(url + apiUrl, jsonBody);
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonBody);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "Application/json");

            yield return request.SendWebRequest();
            string msg = "";

            if (request.isNetworkError)
            {
                Debug.LogError("NetworkError in POST Request : " + api);
            }
            else
            {
                msg = request.downloadHandler.text;
                Debug.Log(msg);

                if (request.responseCode == 201)
                {
                    //post success
                }
                else if (request.responseCode == 401)
                {
                    //unauthorized

                    _requestErrorOccured = true;
                }
                else
                {
                    //error
                    _requestErrorOccured = true;
                }
            }

            if (!_requestErrorOccured)
            {
                yield return null;
                Debug.Log(api + " POST request : " + msg);
            }
        }

        IEnumerator DeleteRequest(string api, string jsonBody)
        {
            var request = new UnityWebRequest(url + api, "DELETE");
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonBody);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "Application/json");

            yield return request.SendWebRequest();
            string msg = "";

            if (request.isNetworkError)
            {
                Debug.LogError("NetworkError in DELETE Request : " + api);
            }
            else
            {
                msg = request.downloadHandler.text;
                Debug.Log(msg);

                if (request.responseCode == 201)
                {
                    //DELETE success
                }
                else if (request.responseCode == 401)
                {
                    //unauthorized

                    _requestErrorOccured = true;
                }
                else
                {
                    //error
                    _requestErrorOccured = true;
                }
            }

            if (!_requestErrorOccured)
            {
                yield return null;
                Debug.Log(api + " DELETE request : " + msg);
            }
        }

        IEnumerator PutRequest(string api, string jsonBody)
        {
            var request = new UnityWebRequest(url + api, "PUT");
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonBody);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "Application/json");

            yield return request.SendWebRequest();
            string msg = "";

            if (request.isNetworkError)
            {
                Debug.LogError("NetworkError in PUT Request : " + api);
            }
            else
            {
                msg = request.downloadHandler.text;
                Debug.Log(msg);

                if (request.responseCode == 201)
                {
                    //PUT success
                }
                else if (request.responseCode == 401)
                {
                    //unauthorized

                    _requestErrorOccured = true;
                }
                else
                {
                    //error
                    _requestErrorOccured = true;
                }
            }

            if (!_requestErrorOccured)
            {
                yield return null;
                Debug.Log(api + " PUT request : " + msg);
            }
        }

        #endregion

        #region Response
        public void OnReceiveGETMessage(string message)
        {
            var response = JsonUtility.FromJson<Response>(message);
            if (_recieveActions.ContainsKey(response.result_type))
            {

                _recieveActions[response.result_type](message, response);
            }
        }

        void ReceiveResponse(string message, Response response)
        {
            Debug.Log("Received Response, result: " + response.GetResult());
        }

        void ReceiveUserResponse(string message, Response rootResponse)
        {
            if (!rootResponse.GetResult())
            {
                //failure on user response
                Debug.Log("Failed User Response");
                return;
            }
            GlobalParameters.Param.isLoad = true;

            UserResponse ur = JsonUtility.FromJson<UserResponse>(message);
            Debug.Log("Received User Response");
            if(ur.result_code == 200)
            {
                GlobalParameters.Param.accountId = ur.result_data.id;
                GlobalParameters.Param.accountName = ur.result_data.nickname;
                GlobalParameters.Param.accountEmail = ur.result_data.email;
                GlobalParameters.Param.accountLevel = ur.result_data.level;
                GlobalParameters.Param.accountExp = ur.result_data.exp;
                GlobalParameters.Param.accountScore = ur.result_data.score;
                GlobalParameters.Param.accountGold = ur.result_data.gold;

            }
        }

        void ReceiveMultiplayLobby(string message, Response rootResponse)
        {
            if (!rootResponse.GetResult())
            {
                //failure on user response
                Debug.Log("Failed User Response");
                return;
            }

            UserResponse[] ur = JsonUtility.FromJson<UserResponse[]>(message);
            Debug.Log("Received ReceiveMultiplayLobby");
            Debug.Log(ur[0].result_data.nickname);
        }
        #endregion
    }
}