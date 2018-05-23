using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Environment;

public class EnvironmentManager : IObserver
{
    private static EnvironmentManager _manager = null;
    public static EnvironmentManager Manager {
        get {
            if (_manager == null) {
                _manager = new EnvironmentManager();
            }
            return _manager;
        }
    }

    private Dictionary<int, EnvironmentTypeInfo> _infos = new Dictionary<int, EnvironmentTypeInfo>();
    private List<EnvironmentModel> _envs = new List<EnvironmentModel>();
    private long _instIdIndex = 0;

    EnvironmentManager() {
        ObserveNotices();
    }

    ~EnvironmentManager() {
        try
        {
            RemoveNotices();
        }
        catch {

        }
    }

    public void InitInfoList(List<EnvironmentTypeInfo> infos) {
        _infos.Clear();
        foreach (var info in infos) {
            _infos.Add(info.Id, info);
        }
    }

    public EnvironmentTypeInfo GetEnvInfo(int id) {
        EnvironmentTypeInfo info = null;
        if (!_infos.TryGetValue(id, out info)) {

#if UNITY_EDITOR
            UnityEngine.Debug.LogError("Could not find EnvInfo : " + id);
#endif
        }
        return info;
    }

    public EnvironmentModel MakeEnvironment(int metaId) {
        EnvironmentTypeInfo info = GetEnvInfo(metaId);
        if (info == null) return null;
        EnvironmentModel model = new EnvironmentModel()
        {
            instanceId = _instIdIndex++
        };

        model.SetMetaInfo(info);
        if (GlobalGameManager.Instance.GameState == GameState.Stage) {
            EnvironmentLayer.CurrentLayer.MakeUnit(model);
        }
        _envs.Add(model);
        return model;
    }

    void Update() {
        foreach (var env in _envs) {

        }
    }

    void FixedUpdate() { }

    public void OnNotice(string notice, params object[] param)
    {
        if (notice == NoticeName.Update) {
            Update();
        }
        else if (notice == NoticeName.FixedUpdate) {
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

    public bool IsValidateId(int metaId) {
        return _infos.ContainsKey(metaId);
    }
}
