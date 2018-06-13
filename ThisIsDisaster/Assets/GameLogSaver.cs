using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameLogSaver : MonoBehaviour {

    public class GlobalGameLogSaver
    {
        StringBuilder builder = new StringBuilder();
        string nameStamp = "";

        public void MakeNameStamp() {

            System.TimeSpan ts = new System.TimeSpan(System.DateTime.Now.Ticks);
            nameStamp = string.Format("{0}{1}{2}", ts.Hours, ts.Minutes, ts.Seconds);
        }

        public void AddLog(string log, string stackTrace, LogType logType)
        {
            if (logType != LogType.Error && logType != LogType.Log) return;
            builder.AppendFormat("[Log]\r\n{0}\r\n[StackTrace]\r\n{1}", log, stackTrace);
            builder.AppendLine();
            builder.AppendLine();
        }

        public void SaveLog()
        {
            FileManager.Instance.SaveLocalTextData(builder.ToString(), "gameLog" + nameStamp);
        }
    }

    GlobalGameLogSaver _saver = new GlobalGameLogSaver();

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Application.logMessageReceived += _saver.AddLog;
        _saver.MakeNameStamp();
    }

    private void OnApplicationPause(bool pause)
    {
        _saver.SaveLog();
    }

}
