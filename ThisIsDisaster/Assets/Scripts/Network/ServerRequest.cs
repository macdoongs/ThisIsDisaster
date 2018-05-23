using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class ServerRequest : MonoBehaviour{
	void Start() {
		StartCoroutine(GetText());
	}

	IEnumerator GetText() {
		UnityWebRequest www = UnityWebRequest.Get("http://api.thisisdisaster.com/user/lobby");
        
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
	}
}
