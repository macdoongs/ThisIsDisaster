using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DevelopmentConsole : MonoBehaviour
{
    public InputField input;
 

    public void OnEndEdit() {
        string txt = input.text;
        if (!string.IsNullOrEmpty(txt)) {
            //execute
            if (txt.ToLower() == "ChangeLanguage".ToLower()) {
                if (GameStaticDataLoader.CurrentLoadLanguage == "kr")
                {
                    GameStaticDataLoader.Loader.SetLanguage("en");
                }
                else {
                    GameStaticDataLoader.Loader.SetLanguage("kr");
                }
                LocalizeTextDataModel.Instance.LogAllData();
            }
        }

        input.text = "";
        
    }

    private void Start()
    {

        input.placeholder.enabled = false;
    }

    public void Close() {
        input.text = "";
        gameObject.SetActive(false);
    }

    public void Open() {
        input.text = "";
        gameObject.SetActive(true);
    }
}
