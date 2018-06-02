using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class NoticeName {
    public static string Log = "Log";
    public static string Update = "Update";
    public static string FixedUpdate = "FixedUpdate";
    public static string LocalPlayerGenerated = "LocalPlayerGenerated";

    public static string AddShelter = "AddShelter";
    public static string OnPlayerEnterShelter = "OnPlayerEnterShelter";
    public static string OnPlayerExitShelter = "OnPlayerExitShelter";

    //network message
    public static string OnPlayerConnected = "OnPlayerConnected";
    public static string OnPlayerDisconnected = "OnPlayerDisconnected";
    public static string OnStartSession = "OnStartSession";
    public static string OnReceiveMatchingResponse = "OnReceiveMatchingResponse";
}

public class Notice {

    private static Notice _instance;
    public static Notice Instance {
        get {
            if (_instance == null) {
                _instance = new Notice();
            }
            return _instance;
        }
    }

    private Dictionary<string, List<IObserver>> noticeList;
    private int lastNoticeId;

    private Notice() {
        lastNoticeId = 0;
        noticeList = new Dictionary<string, List<IObserver>>();
    }

    public void Observe(string notice, IObserver observer) {
        List<IObserver> list = null;
        if (noticeList.TryGetValue(notice, out list))
        {
            if (list.Contains(observer)) return;
            list.Add(observer);
        }
        else {
            list = new List<IObserver>();
            list.Add(observer);
            noticeList.Add(notice, list);
        }
    }

    public void Remove(string notice, IObserver observer) {
        List<IObserver> list = null;
        if (noticeList.TryGetValue(notice, out list)) {
            list.Remove(observer);
        }
    }

    public void Send(string notice, params object[] param) {
        List<IObserver> list = null;
        if (noticeList.TryGetValue(notice, out list)) {
            foreach (var target in list) {
                target.OnNotice(notice, param);
            }
        }
    }
}


public interface IObserver {
    void OnNotice(string notice, params object[] param);

    void ObserveNotices();
    void RemoveNotices();
}