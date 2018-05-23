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

    private List<StageGenerator.ClimateInfo.NpcInfo> _npcGenInfo = null;

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

    public void SetNpcGenInfo(List<StageGenerator.ClimateInfo.NpcInfo> infos) {
        this._npcGenInfo = infos;
    }

    /// <summary>
    /// called by host
    /// </summary>
    public void CheckGeneration() {
        //check count
        foreach (var info in _npcGenInfo) {
            try
            {
                int genCount = info.max - GetAliveCount(info.id);//0 will be current alive npc
                var coords = CellularAutomata.Instance.GetRoomsCoord(1, genCount);
                for (int i = 0; i < genCount; i++)
                {
                    var model = MakeNPC(info.id);
                    var tile = RandomMapGenerator.Instance.GetTile(coords[i].tileX, coords[i].tileY);
                    model.UpdatePosition(tile.transform.position);
                }
            }
            catch {
                Debug.LogError("Monster gen error");
            }
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

    int GetAliveCount(int metaId) {
        int count = _npcs.FindAll((x) => (x.MetaInfo.Id == metaId) && x.State == NPCState.Execute).Count;
        return count;
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