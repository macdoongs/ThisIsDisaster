using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour {

    public void SceneChange1()
    {
        SceneManager.LoadScene("GameLobby");
    }

    public void SceneChange2()
    {
        SceneManager.LoadScene("MapGenerationScene");
    }
}
