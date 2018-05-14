using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType {
    Cyclone = 0,
    Flood,


    None,//dummy for end
    /*
     and so on
     fire, 
     rainy
     */
}

public class EventManager : MonoBehaviour {
    public static EventManager Manager {
        private set;
        get;
    }

    private Dictionary<EventType, EventBase> dictioary = new Dictionary<EventType, EventBase>();

    public GameObject rainObject = null;
    public GameObject darkObject = null;
    public EventType currentTestType = EventType.None;
    
    private void Awake()
    {
        if (Manager != null) {
            Destroy(Manager.gameObject);
        }
        Manager = this;
    }

    public void OnGenerate(EventType type) {
        EventBase e = null;
        if (dictioary.TryGetValue(type, out e)) {
            return;
        }
        //generate;
        EventBase evntBase = GenEvent(type);
        if (evntBase == null) return;
        dictioary.Add(type, evntBase);

        evntBase.OnGenerated();
        Debug.Log("Generate " + evntBase.type);
    }

    EventBase GenEvent(EventType type) {
        switch (type) {
            case EventType.Cyclone:
                return new CycloneEvent();
            case EventType.Flood:
                return new FloodEvent();
            default:
                return null;
        }
    }

    EventBase GetEvent(EventType type) {
        EventBase output = null;
        if (dictioary.TryGetValue(type, out output)) {
            return output;
        }
        return null;
    }

    public void OnStart(EventType type) {
        var e = GetEvent(type);
        if (e == null) return;
        if (e.IsStarted) return;
        e.OnStart();
        e.IsStarted = true;
        Debug.Log("Start " + e.type);
    }

    public void OnExecute() {

        foreach (var v in dictioary.Values) {
            if (v.IsStarted) {
                v.OnExecute();
                Debug.Log("Execute "+v.type);
            }
        }
    }

    public void OnEnd(EventType type) {
        var e = GetEvent(type);
        if (e == null) return;
        e.OnEnd();
        e.IsStarted = false;
        Debug.Log("End " + e.type);
    }

    public void OnDestroyEvent(EventType type)
    {
        var e = GetEvent(type);
        if (e == null) return;
        e.OnDestroy();
        dictioary.Remove(type);
        Debug.Log("Destroy " + e.type);
    }

    public void OnGiveDamage(EventType type, UnitModel target, float damageValue) {
        var e = GetEvent(type);
        if (e == null) return;
        //
        //e.OnGiveDamage(target, damageValue);
    }

    private void FixedUpdate()
    {
        OnExecute();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.F5))
        {
            EventManager.Manager.OnGenerate(currentTestType);
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            EventManager.Manager.OnStart(currentTestType);
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            EventManager.Manager.OnEnd(currentTestType);
        }

        if (Input.GetKeyDown(KeyCode.F8))
        {
            EventManager.Manager.OnDestroyEvent(currentTestType);
        }
    }

    public GameObject MakeWorkdRain() {
        if (rainObject == null)
        {
            GameObject effectObject = Resources.Load<GameObject>("Prefabs/Rain");
            if (effectObject)
            {
                effectObject = Instantiate(effectObject);
                effectObject.transform.SetParent(Camera.main.transform);
                effectObject.transform.localPosition = new Vector3(0f, 5f, 10f);
                effectObject.transform.localScale = Vector3.one;
                effectObject.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                effectObject.SetActive(false);

            }

            rainObject = effectObject;
            
        }

        return rainObject;
    }

    public GameObject MakeWorkdDark() {
        if (darkObject == null)
        {
            GameObject effectObject = Resources.Load<GameObject>("Prefabs/Brightness");
            if (effectObject)
            {
                effectObject = Instantiate(effectObject);
                effectObject.transform.SetParent(Camera.main.transform);
                effectObject.transform.localPosition = new Vector3(0f,0f,1f);
                effectObject.transform.localScale = new Vector3(10f, 10f, 1f);
                effectObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                effectObject.SetActive(false);

            }

            darkObject = effectObject;

        }

        return darkObject;
    }
}

public class EventBase {
    public EventType type = EventType.None;

    protected bool _isStarted = false;
    public bool IsStarted { get { return _isStarted; } set { _isStarted = value; } }
    public int Level = 0;

    public EventBase() {
        type = EventType.None;
    }

    public virtual void OnGenerated() { }

    public virtual void OnStart() {
    }

    public virtual void OnExecute() { }

    public virtual void OnEnd() { }

    public virtual void OnDestroy() { }

    public virtual void OnGiveDamage(UnitModel target, float DamageValue) {

    }
}

public class CycloneEvent : EventBase {
    GameObject rainObject = null;
    GameObject darkObject = null;
    public CycloneEvent() {
        type = EventType.Cyclone;
    }

    public override void OnGenerated()
    {
        rainObject = EventManager.Manager.MakeWorkdRain();
        darkObject = EventManager.Manager.MakeWorkdDark();
    }

    public override void OnStart()
    {
        rainObject.SetActive(true);
        darkObject.SetActive(true);
    }

    public override void OnEnd()
    {
        rainObject.SetActive(false);
        darkObject.SetActive(false);
    }

    public override void OnDestroy()
    {

    }
}

public class FloodEvent : EventBase {
    public FloodEvent() {
        type = EventType.Flood;
    }
}