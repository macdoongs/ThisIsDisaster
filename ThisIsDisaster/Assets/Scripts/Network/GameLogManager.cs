using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Json;
using UnityEngine;

public enum GameLogType {
    StageStart,
    StageEnd,
    EventGenerated,
    EventStarted,
    EventEnded,
    PlayerDead,
    PlayerItemAcquired,
    PlayerEnterShelter,
    PlayerExitShelter,
    PlayerGetDisorder,
    PlayerRemoveDisorder,
}

public enum PlayerDeadType {
    Monster,
    Event,
    Environment
}

public class GameLogManager : IObserver
{
    static class GameLogFormat {
        //ko-kr
        public const string StageStart = "{0} {1} 스테이지가 시작되었습니다.";//싱글/멀티
        public const string StageEnd = "{0} {2} 스테이지가 종료되었습니다. {1}";//싱글/멀티, 승리/패배
        public const string EventGenerated = "{0} 재난이 감지됩니다.";//재난 종류
        public const string EventStarted = "{0} 재난이 발생하였습니다.";//재난 종류
        public const string EventEnded = "{0} 재난이 종료되었습니다.";//재난 종류
        public const string PlayerDead = "{0} 플레이어가 {1}로 인해 사망하였습니다.";//플레이어, 사망 원인
        public const string PlayerItemAcquired = "{0} 플레이어가 {1}를 획득하였습니다.";//플레이어, 아이템
        public const string PlayerEnterShelter = "{0} 플레이어가 피난처에 입장하였습니다.";//플레이어
        public const string PlayerExitShelter = "{0} 플레이어가 피난처에서 퇴장하였습니다.";//플레이어
        public const string PlayerGetDisorder = "{0} 플레이어가 {1}상태이상에 빠졌습니다.";//플레이어, 상태이상
        public const string PlayerRemovetDisorder = "{0} 플레이어가 {1}상태이상을 치료하였습니다.";//플레이어, 상태이상

        public static string GetLogFormat(GameLogType type)
        {
            switch (type)
            {
                case GameLogType.StageStart: return StageStart;
                case GameLogType.StageEnd: return StageEnd;
                case GameLogType.EventGenerated: return EventGenerated;
                case GameLogType.EventStarted: return EventStarted;
                case GameLogType.EventEnded: return EventEnded;
                case GameLogType.PlayerDead: return PlayerDead;
                case GameLogType.PlayerItemAcquired: return PlayerItemAcquired;
                case GameLogType.PlayerEnterShelter: return PlayerEnterShelter;
                case GameLogType.PlayerExitShelter: return PlayerExitShelter;
                case GameLogType.PlayerGetDisorder: return PlayerGetDisorder;
                case GameLogType.PlayerRemoveDisorder: return PlayerRemovetDisorder;
            }
            return "";
        }
    }

    public class LogMessage {
        public int id = 0;
        public string title = "gamelog";
        public string log = "";

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
                _instance.ObserveNotices();
            }
            return _instance;
        }
    }
    private const string _gameLog = "game/log/";
    private const string logFormat = "“id”:“{0}”, “title”:“error”, “log”:“{1}”";

    static string LogSrc
    {
        get
        {
            return _gameLog;
        }
    }

    private StringBuilder builder = new StringBuilder();

    Json.WebCommunicationManager WebManager {
        get {
            return Json.WebCommunicationManager.Manager;
        }
    }

    public void SaveLog(string log) {
        var json = new LogMessage(log);
        builder.AppendLine(log);

        var jsonMsg = JsonUtility.ToJson(json);

        WebManager.SendRequest(Json.RequestMethod.POST, LogSrc, jsonMsg);
    }

    public string GetLog() {
        WebManager.SendRequest(Json.RequestMethod.GET, "game/logs");
        return "";  
    }

    public void OnNotice(string notice, params object[] param)
    {
        if (notice == NoticeName.SaveGameLog) {
            if (param[0] is GameLogType) {
                GameLogType type = (GameLogType)param[0];
                try
                {
                    MakeLog(type, param);
                }
                catch {
                    //format error
                    Debug.LogError("Format error");
                }
            }
        }
    }

    void MakeLog(GameLogType type, params object[] param) {
        //ignore param[0] because that is type
        string format = GameLogFormat.GetLogFormat(type);
        List<object> p = new List<object>(param);
        p.RemoveAt(0);
        string log = string.Format(format, p.ToArray());
        SaveLog(log);

        Debug.LogError("[GameLog]" + log);
    }

    public string OnGameEnd() {
        string output = builder.ToString();
        builder.Length = 0;
        return output;
    }
    
    public void ObserveNotices()
    {
        Notice.Instance.Observe(NoticeName.SaveGameLog, this);
    }

    public void RemoveNotices()
    {
        //may be not called till game closed
        Notice.Instance.Remove(NoticeName.SaveGameLog, this);
    }
}
