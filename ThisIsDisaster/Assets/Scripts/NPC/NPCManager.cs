using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using NPC;

public class NPCManager : IObserver
{
    private static NPCManager _manager = null;
    public static NPCManager Manager
    {
        get
        {
            if (_manager == null)
            {
                _manager = new NPCManager();
            }
            return _manager;
        }
    }

    private Dictionary<int, NPCTypeInfo> _infoDic = new Dictionary<int, NPCTypeInfo>();
    private List<NPCModel> _npcs = new List<NPCModel>();
    private long _instIdIndex = 0;

    NPCManager()
    {
        ObserveNotices();
    }

    void OnDestroy()
    {
        RemoveNotices();
    }

    public void SetNpcInfos(List<NPCTypeInfo> list) {
        _infoDic.Clear();
        foreach (var v in list) {
            _infoDic.Add(v.Id, v);
        }
    }

    public NPCTypeInfo GetNPCInfo(int id) {
        NPCTypeInfo info = null;
        if (!_infoDic.TryGetValue(id, out info)) {
            //log
#if UNITY_EDITOR
            Debug.LogError("Could not find NpcInfo : " + id);
#endif
        }
        return info;
    }

    public NPCModel MakeNPC(int metaId) {
        NPCTypeInfo info = GetNPCInfo(metaId);
        if (info == null) {
            return null;
        }
        NPCModel model = new NPCModel()
        {
            instanceId = _instIdIndex++
        };
        model.SetMetaInfo(info);

        if (GlobalGameManager.Instance.GameState == GameState.Stage) {
            NPCLayer.CurrentLayer.MakeUnit(model);
        }

        _npcs.Add(model);

        model.Init();

        return model;
    }

    void Update() {
        foreach (var n in _npcs) {
            n.Update();
        }
    }

    void FixedUpdate() { }

    public List<NPCModel> GetAllNPCs() {
        return new List<NPCModel>(_npcs.ToArray());
    }
    
    public void OnNotice(string notice, params object[] param)
    {
        if (notice == NoticeName.Update)
        {
            Update();
        }
        else if (notice == NoticeName.FixedUpdate)
        {
            FixedUpdate();
        }
    }

    public void ObserveNotices()
    {
        Notice.Instance.Observe(NoticeName.Update, this);
        Notice.Instance.Observe(NoticeName.FixedUpdate, this);
    }

    public void RemoveNotices()
    {
        Notice.Instance.Remove(NoticeName.Update, this);
        Notice.Instance.Remove(NoticeName.FixedUpdate, this);
    }
}