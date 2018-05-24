using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Json;

public class ServerRequest : MonoBehaviour{
    const string url = "http://api.thisisdisaster.com/";

    /*
	void Start() {
		StartCoroutine(GetText());
	}

	IEnumerator GetText() {
		UnityWebRequest www = UnityWebRequest.Get("http://api.thisisdisaster.com/user/lobby");

        WWWForm form = new WWWForm();
        
		yield return www.Send();

		if(www.isNetworkError) {
			Debug.Log(www.error);
		}
		else {
			// Show results as text
			Debug.Log(www.downloadHandler.text);

			UserResponse userResponse;

			string json = www.downloadHandler.text;

			userResponse = JsonUtility.FromJson<UserResponse>(json);

			Debug.Log (userResponse.result_data);
		}
	}*/
}
