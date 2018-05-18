using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeatherType {
    Cyclone = 0,
    Flood,
    Yellowdust,
    Drought,
    Fire,
    Earthquake,
    Lightning,
    Landsliding,
    Heavysnow,




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

    private Dictionary<WeatherType, EventBase> dictioary = new Dictionary<WeatherType, EventBase>();

    public GameObject cycloneObject = null;
    public GameObject rainObject = null;
    public GameObject darkObject = null;
    public GameObject snowObject = null;
    public GameObject sandObject = null;
    public GameObject cloudyObject = null;

    public WeatherType currentTestType = WeatherType.None;
    
    private void Awake()
    {
        if (Manager != null) {
            Destroy(Manager.gameObject);
        }
        Manager = this;
    }

    public List<EventBase> GetAllEvents() {
        List<EventBase> output = new List<EventBase>();
        foreach (var kv in dictioary) {
            output.Add(kv.Value);
        }
        return output;
    }

    public void OnGenerate(WeatherType type) {
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

    EventBase GenEvent(WeatherType type) {
        switch (type) {
            case WeatherType.Cyclone:
                return new CycloneEvent();
            case WeatherType.Flood:
                return new FloodEvent();
            case WeatherType.Yellowdust:
                return new YellowdustEvent();
            case WeatherType.Drought:
                return new DroughtEvent();
            case WeatherType.Fire:
                return new FireEvent();
            case WeatherType.Earthquake:
                return new EarthquakeEvent();
            case WeatherType.Lightning:
                return new LightningEvent();
            case WeatherType.Landsliding:
                return new LandslidingEvent();
            case WeatherType.Heavysnow:
                return new HeavysnowEvent();
            default:
                return null;
        }
    }

    EventBase GetEvent(WeatherType type) {
        EventBase output = null;
        if (dictioary.TryGetValue(type, out output)) {
            return output;
        }
        return null;
    }

    public void OnStart(WeatherType type) {
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
                //Debug.Log("Execute "+v.type);
            }
        }
    }

    public void OnEnd(WeatherType type) {
        var e = GetEvent(type);
        if (e == null) return;
        e.OnEnd();
        e.IsStarted = false;
        Debug.Log("End " + e.type);
    }

    public void OnDestroyEvent(WeatherType type)
    {
        var e = GetEvent(type);
        if (e == null) return;
        e.OnDestroy();
        dictioary.Remove(type);
        Debug.Log("Destroy " + e.type);
    }

    public void OnGiveDamage(WeatherType type, UnitModel target, float damageValue) {
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
    }         // 메뉴 실행 키

    public GameObject MakeWorldRain() {                       // 공통 비
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

    public GameObject MakeWorldDark() {                       //공통 어둠
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
    }     // map을 어둡게하는 effect

    public void SetWorldFilterColor(Color color) {

        SpriteRenderer renderer = darkObject.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = color;
        }
    }

    public SnowEffect MakeWorldSnow()
    {
        if (snowObject == null)
        {
            GameObject effectObject = Resources.Load<GameObject>("Prefabs/SnowEffect");
            if (effectObject)
            {
                effectObject = Instantiate(effectObject);
                effectObject.transform.SetParent(Camera.main.transform);
                effectObject.transform.localPosition = new Vector3(1f, 1f, 1f);    // 0  5 10
                effectObject.transform.localScale = Vector3.one;
                effectObject.transform.localRotation = Quaternion.Euler(0f, 1f, 1f);
                effectObject.SetActive(false);

            }

            snowObject = effectObject;

        }

        return snowObject.GetComponent<SnowEffect>();
    }

   public CloudyEffect MakeWorldCloudy()
    {                       
        if (cloudyObject == null)
        {
            GameObject effectObject = Resources.Load<GameObject>("Prefabs/Cloudy");
            if (effectObject)
            {
                effectObject = Instantiate(effectObject);
                effectObject.transform.SetParent(Camera.main.transform);
                effectObject.transform.localPosition = new Vector3(0f, 0f, 1f);
                effectObject.transform.localScale = new Vector3(10f, 10f, 1f);
                effectObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                effectObject.SetActive(false);

            }
            cloudyObject = effectObject;

        }

        return cloudyObject.GetComponent<CloudyEffect>();
    }     // map을 하얗게하는 effect   

    public SandEffect MakeWorldSand()
    {
        if (sandObject == null)
        {
            GameObject effectObject = Resources.Load<GameObject>("Prefabs/SandEffect");
            if (effectObject)
            {
                effectObject = Instantiate(effectObject);
                effectObject.transform.SetParent(Camera.main.transform);
                effectObject.transform.localPosition = new Vector3(0f, 5f, 10f);
                effectObject.transform.localScale = Vector3.one;
                effectObject.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                effectObject.SetActive(false);

            }

            sandObject = effectObject;

        }

        return sandObject.GetComponent<SandEffect>();
    }

    public CycloneEffect MakeWorldCyclone()
    {
        if (cycloneObject == null)
        {
            GameObject effectObject = Resources.Load<GameObject>("Prefabs/CycloneEffect");
            if (effectObject)
            {
                effectObject = Instantiate(effectObject);
                effectObject.transform.SetParent(Camera.main.transform);
                effectObject.transform.localPosition = new Vector3(0f, 5f, 10f);
                effectObject.transform.localScale = new Vector3(3f,1f,1f);
                effectObject.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                effectObject.SetActive(false);

            }

            cycloneObject = effectObject;

        }

        return cycloneObject.GetComponent<CycloneEffect>();
    }


    public GameObject MakeFire(Vector3 position)
    {
        GameObject effectObject = Resources.Load<GameObject>("Prefabs/fire");
        if (effectObject)
        {
            effectObject = Instantiate(effectObject);
            //effectObject.transform.SetParent(Camera.main.transform);
            effectObject.transform.localPosition = position;
            effectObject.transform.localScale = Vector3.one;
            effectObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            effectObject.SetActive(false);

        }

        return effectObject;
    }
}

public class EventBase {
    public WeatherType type = WeatherType.None;

    protected bool _isStarted = false;
    public bool IsStarted { get { return _isStarted; } set { _isStarted = value; } }
    public int Level = 0;

    public EventBase() {
        type = WeatherType.None;
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
    CycloneEffect cycloneObject = null;
    GameObject darkObject = null;
    
    public CycloneEvent() {
        type = WeatherType.Cyclone;
    }

    public override void OnGenerated()
    {
        cycloneObject = EventManager.Manager.MakeWorldCyclone();
        darkObject = EventManager.Manager.MakeWorldDark();

      
    }

    public override void OnStart()
    {
        cycloneObject.SetActive(true);
        darkObject.SetActive(true);
    }

    public override void OnEnd()
    {
        cycloneObject.SetActive(false);
        darkObject.SetActive(false);
    }

    public override void OnDestroy()
    {

    }
}      // 태풍 이벤트

public class FloodEvent : EventBase {
    GameObject rainObject = null;
    GameObject darkObject = null;
    GameObject fillObject = null;    // 맵에 물차오르는거
    public FloodEvent() {
        type = WeatherType.Flood;
    }

    public override void OnGenerated()
    {
        rainObject = EventManager.Manager.MakeWorldRain();
        darkObject = EventManager.Manager.MakeWorldDark();

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
}  // 홍수 이벤트

public class YellowdustEvent : EventBase
{
    SandEffect SandparticleObject = null;     // 모래입자
    GameObject YellowObject = null;          // 노란 화면
    public YellowdustEvent()
    {
        type = WeatherType.Yellowdust;
    }

    public override void OnGenerated()
    {
        SandparticleObject = EventManager.Manager.MakeWorldSand();
        YellowObject = null;
    }

    public override void OnStart()
    {
        SandparticleObject.SetActive(true);
       
       YellowObject = null;
    }

    public override void OnEnd()
    {
        SandparticleObject.SetActive(false);
        YellowObject = null;
    }

    public override void OnDestroy()
    {
        
    }
}  // 황사 이벤트

public class DroughtEvent : EventBase
{
    GameObject dryObject = null;         // 맵 건조
    GameObject crackObject = null;      // 공통1 (바닥 갈라짐)

    public DroughtEvent()
    {
        type = WeatherType.Drought;
    }

    public override void OnGenerated()
    {
        dryObject = null;
        crackObject = null;
    }

    public override void OnStart()
    {
        dryObject = null;
        crackObject = null;
    }

    public override void OnEnd()
    {
        dryObject = null;
        crackObject = null;
    }

    public override void OnDestroy()
    {

    }
}  // 가뭄 이벤트

public class FireEvent : EventBase
{
    GameObject fireObject = null;

    public FireEvent()
    {
        type = WeatherType.Fire;
    }

    public override void OnGenerated()
    {
        fireObject = null;
    }
    public override void OnStart()
    {
        fireObject = null;
    }

    public override void OnEnd()
    {
        fireObject = null;
    }

    public override void OnDestroy()
    {
        
    }
}  // 화재 이벤트

public class EarthquakeEvent : EventBase
{
    GameObject quakeObject = null;          // 공통2  (맵흔들림)
    GameObject crackObject = null;          // 공통1 (갈라짐)
    public EarthquakeEvent()
    {
        type = WeatherType.Earthquake;
    }

    public override void OnGenerated()
    {
        quakeObject = null;
        crackObject = null;
        
    }

    public override void OnStart()
    {
        quakeObject = null;
        crackObject = null;
    }

    public override void OnEnd()
    {
        quakeObject = null;
        crackObject = null;
    }

    public override void OnDestroy()
    {
        
    }
}  // 지진 이벤트

public class LightningEvent : EventBase
{
    GameObject rainObject = null;
    GameObject darkObject = null;
    GameObject blinkObject = null;      //  깜빡이
    GameObject lightningObject = null;     // 낙뢰 효과
    public LightningEvent()
    {
        type = WeatherType.Lightning;
    }

    public override void OnGenerated()
    {
        rainObject = EventManager.Manager.MakeWorldRain();
        darkObject = EventManager.Manager.MakeWorldDark();

        blinkObject = null;
        lightningObject = null;
    }

    public override void OnStart()
    {
        rainObject.SetActive(true);
        darkObject.SetActive(true);

        blinkObject = null;
        lightningObject = null;
    }

    public override void OnEnd()
    {
        rainObject.SetActive(false);
        darkObject.SetActive(false);

        blinkObject = null;
        lightningObject = null;
    }

    public override void OnDestroy()
    {
        
    }
}  // 낙뢰 이벤트

public class LandslidingEvent : EventBase
{
    GameObject quackObject = null;    // 공통2  (맵 흔들림)
    GameObject collapseObject = null;      // 맵 이벤트

    public LandslidingEvent()
    {
        type = WeatherType.Landsliding;
    }

    public override void OnGenerated()
    {
        quackObject = null;
        collapseObject = null;
    }

    public override void OnStart()
    {
        quackObject = null;
        collapseObject = null;
    }

    public override void OnEnd()
    {
        quackObject = null;
        collapseObject = null;
    }

    public override void OnDestroy()
    {
        
    }
}  // 산사태 이벤트

public class HeavysnowEvent : EventBase
{
    int max = -1;//max level of this event
    SnowEffect snowObject = null;
    CloudyEffect cloudyObject = null;  

    public HeavysnowEvent()
    {
        type = WeatherType.Heavysnow;
    }

    public override void OnGenerated()
    {
        snowObject = EventManager.Manager.MakeWorldSnow();

        cloudyObject = EventManager.Manager.MakeWorldCloudy();
    }

    public override void OnStart()
    {
        snowObject.SetActive(true);
        snowObject.SetLevel(5);
        EventManager.Manager.SetWorldFilterColor(new Color(71f/255f, 73f/255f, 73f/255f, 98f/255f));
        cloudyObject.SetActive(true);
        
    }

    public override void OnEnd()
    {
        snowObject.SetActive(false);
        cloudyObject.SetActive(false);
    }

    public override void OnDestroy()
    {
        
    }
}  // 폭설 이벤트

