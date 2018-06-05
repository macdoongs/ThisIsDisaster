using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene;
    public Image LoadingBackground;



    [SerializeField]
    Image progressBar;
    public Text progressText;

    private void Start()
    {


        StartCoroutine(LoadScene());
    }

    string nextSceneName;


    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("Loading Scene");
    }

    IEnumerator LoadScene()
    {

        System.Random random = new System.Random();
        int x = random.Next(1, 190);
        x = x % 19 + 1;
        string backgroundSrc = "loading/loading" + x.ToString();
        Sprite s = Resources.Load<Sprite>(backgroundSrc);

        LoadingBackground.sprite = s;
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0.0f;
        while (!op.isDone)
        {
            yield return null;

            timer += Time.deltaTime;

            if (op.progress >= 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                progressText.text = ((int)(Mathf.Lerp(progressBar.fillAmount, 1f, timer)*100)).ToString() + " %";


                if (progressBar.fillAmount == 1.0f)
                    op.allowSceneActivation = true;
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                progressText.text =((int)(Mathf.Lerp(progressBar.fillAmount, 1f, timer)*100)).ToString() + " %";

                if (progressBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
        }
    }
}
