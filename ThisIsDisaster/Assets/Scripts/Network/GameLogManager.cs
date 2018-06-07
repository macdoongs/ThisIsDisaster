using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Json;
using UnityEngine;

public class GameLogManager {
    public class LogMessage {
        public int id = 0;
        public string title = "error";
        public string log = "";
        public long create = 1525685748965;

        public LogMessage(string log) {
            id = GlobalParameters.Param.accountId;
            this.log = log;
        }
    }

    private static GameLogManager _instance = null;
    public static GameLogManager Instance {
        get
        {
            if (_instance == null) {
                _instance = new GameLogManager();
            }
            return _instance;
        }
    }
    private const string _errorLog = "error/";
    private const string logFormat = "“id”:“{0}”, “title”:“error”, “log”:“{1}”, “create”: 1525685748965";

    static string LogSrc
    {
        get
        {
            return _errorLog;
        }
    }

    private StringBuilder builder = new StringBuilder();

    Json.WebCommunicationManager WebManager {
        get {
            return Json.WebCommunicationManager.Manager;
        }
    }

    public bool SaveLog(string log) {
        var json = new LogMessage(log);
        builder.AppendLine(log);

        var jsonMsg = JsonUtility.ToJson(json);

        WebManager.SendRequest(Json.RequestMethod.POST, LogSrc, jsonMsg);
        return true;
    }

    public string GetLog() {
        WebManager.SendRequest(Json.RequestMethod.GET, "errors");
        return "";  
    }
}
