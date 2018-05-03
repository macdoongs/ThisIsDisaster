using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class DevelopmentConsole : MonoBehaviour
{
    public Text fullLogArea;
    public Transform FullLogParent;
    public Text extractedLogArea;
    public Transform ExtractedLogParent;

    [Space(10)]
    public InputField input;
    StringBuilder fullLog = new StringBuilder();
    StringBuilder extractedLog = new StringBuilder();

    public bool ShowFullLog = true;

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

    private void Awake()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void Start()
    {
        input.placeholder.enabled = false;
    }

    private void OnEnable()
    {
        SetText();
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void Update()
    {
        if (ShowFullLog)
        {
            if (!FullLogParent.gameObject.activeInHierarchy) {
                FullLogParent.gameObject.SetActive(true);
            }
            if (ExtractedLogParent.gameObject.activeInHierarchy) {
                ExtractedLogParent.gameObject.SetActive(false);
            }
        }
        else
        {
            if (FullLogParent.gameObject.activeInHierarchy)
            {
                FullLogParent.gameObject.SetActive(false);
            }
            if (!ExtractedLogParent.gameObject.activeInHierarchy)
            {
                ExtractedLogParent.gameObject.SetActive(true);
            }
        }
    }

    public void Close() {
        input.text = "";
        gameObject.SetActive(false);
    }

    public void Open() {
        input.text = "";
        gameObject.SetActive(true);
    }
    
    public void HandleLog(string logString, string stackTrace, LogType logType) {
        if (logType != LogType.Error && logType != LogType.Log) return;
        string fullFormat = "<color={2}>{0}\r\n{1}</color>\r\n\r\n";
        string extractedFormat = "<color={1}>{0}</color>\r\n\r\n";
        Color logColor = Color.black;
        if (logType == LogType.Error) {
            logColor = Color.red;
        }
        string colorText = ColorUtility.ToHtmlStringRGBA(logColor);
        fullLog.AppendFormat(fullFormat, logString, stackTrace, colorText);
        extractedLog.AppendFormat(extractedFormat, logString, colorText);
        if (isActiveAndEnabled) {
            SetText();
        }
    }

    void SetText() {
        fullLogArea.text = fullLog.ToString();
        extractedLogArea.text = extractedLog.ToString();
    }
}
