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
    Thunderstorm,
    Landslide,
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
    public GameObject fireObject = null;
    public GameObject snowObject = null;
    public GameObject sandObject = null;
    public GameObject cloudObject = null;
    public GameObject blinkObject = null;
    public GameObject lightningObject = null;
    public GameObject dryObject = null;
    public GameObject crackObject = null;
    private EarthquakeEffect earthquakeEffect = null;
	private LandslideEffect landslideEffect = null;
	private FloodEffect floodEffect = null;

	public GameObject tsunamiObject = null;
	public GameObject landslideObject = null;

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
        //이벤트 시작
        InGameUIScript.Instance.EventNotice(evntBase.type.ToString(), 0);
        InGameUIScript.Instance.EventDescSetting(type);
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
			case WeatherType.Thunderstorm:
				return new ThunderstormEvent();
			case WeatherType.Landslide:
				return new LandslideEvent();
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
        DisorderController.Instance.MakeDisorder();
        e.OnStart();
        e.IsStarted = true;

        Debug.Log("Start " + e.type);
        InGameUIScript.Instance.EventNotice(e.type.ToString(), 1);
    }

    public void OnExecute() {

        foreach (var v in dictioary.Values) {
            if (v.IsStarted) {
                v.OnExecute();
                //Debug.Log("Execute "+v.type);
            }
        }
    }

    public void EndEvent(WeatherType type) {
        var e = GetEvent(type);
        if (e == null) return;
        e.OnEnd();
        e.IsStarted = false;
        Debug.Log("End " + e.type);
        InGameUIScript.Instance.EventNotice(e.type.ToString(), 2);
        InGameUIScript.Instance.DefaultEventDesc();
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

		if (Input.GetKeyDown(KeyCode.O))
        {
            EventManager.Manager.OnGenerate(currentTestType);
        }

		if (Input.GetKeyDown(KeyCode.P))
        {
            EventManager.Manager.OnStart(currentTestType);
        }

		if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            EventManager.Manager.EndEvent(currentTestType);
        }

		if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            EventManager.Manager.OnDestroyEvent(currentTestType);
        }
    }         // 메뉴 실행 키

	public GameObject MakeWorldBlink() {                       // 공통 비
		if (blinkObject == null)
		{
			GameObject effectObject = Resources.Load<GameObject>("Prefabs/Blink");
			if (effectObject)
			{
				effectObject = Instantiate(effectObject);
				effectObject.transform.SetParent(Camera.main.transform);
				effectObject.SetActive(false);

			}

			blinkObject = effectObject;

		}

		return blinkObject;
	}

    public GameObject MakeWorldRain() {                       // 공통 비
        if (rainObject == null)
        {
            GameObject effectObject = Resources.Load<GameObject>("Prefabs/Rain");
            if (effectObject)
            {
                effectObject = Instantiate(effectObject);
                effectObject.transform.SetParent(Camera.main.transform);
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

    public GameObject MakeWorldLightning()
    {                       
        if (lightningObject == null)
        {
            GameObject effectObject = Resources.Load<GameObject>("Prefabs/LightningEffect");
            if (effectObject)
            {
                effectObject = Instantiate(effectObject);
                effectObject.transform.SetParent(Camera.main.transform);
                effectObject.SetActive(false);

            }

            lightningObject = effectObject;

        }

        return lightningObject;
    }

    public GameObject MakeWorldDry()
    {
        if (dryObject == null)
        {
            GameObject effectObject = Resources.Load<GameObject>("Prefabs/Dry");
            if (effectObject)
            {
                effectObject = Instantiate(effectObject);
                effectObject.transform.SetParent(Camera.main.transform);
                effectObject.SetActive(false);

            }

            dryObject = effectObject;

        }

        return dryObject;
    }

    public GameObject MakeWorldCrack()
    {
        if (crackObject == null)
        {
            GameObject effectObject = Resources.Load<GameObject>("Prefabs/Crack");
            if (effectObject)
            {
                effectObject = Instantiate(effectObject);
                effectObject.transform.SetParent(Camera.main.transform);
                effectObject.SetActive(false);

            }

            crackObject = effectObject;

        }

        return crackObject;
    }


    public void SetWorldFilterColor(Color color) {
		if (darkObject = null) {
			SpriteRenderer renderer = darkObject.GetComponent<SpriteRenderer>();
			if (renderer != null)
			{
				renderer.color = color;
			}
		}
    }

    public FireEffect GetFireEffect()
    {
        if (fireObject == null)
        {
            GameObject effectObject = Resources.Load<GameObject>("Prefabs/FireEffect");
            if (effectObject)
            {
                effectObject = Instantiate(effectObject);
                effectObject.SetActive(false);

            }

            fireObject = effectObject;

        }

        return fireObject.GetComponent<FireEffect>();
    }

    public SnowEffect GetSnowEffect()
    {
        if (snowObject == null)
        {
            GameObject effectObject = Resources.Load<GameObject>("Prefabs/SnowEffect");
            if (effectObject)
            {
                effectObject = Instantiate(effectObject);
                effectObject.transform.SetParent(Camera.main.transform);
                effectObject.transform.localPosition = new Vector3(0f, 0f, 0f);    // 0  5 10
                effectObject.transform.localScale = Vector3.one;
                effectObject.transform.localRotation = Quaternion.Euler(0f, 1f, 1f);
                effectObject.SetActive(false);

            }

            snowObject = effectObject;

        }

        return snowObject.GetComponent<SnowEffect>();
    }

   public CloudEffect GetCloudyEffect()
    {                       
        if (cloudObject == null)
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
            cloudObject = effectObject;

        }

        return cloudObject.GetComponent<CloudEffect>();
    }     // map을 하얗게하는 effect   

    public SandEffect GetSandEffect()
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

    public CycloneEffect GetCycloneEffect()
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
                effectObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                effectObject.SetActive(false);

            }

            cycloneObject = effectObject;

        }

        return cycloneObject.GetComponent<CycloneEffect>();
    }

	public GameObject MakeWorldLandslide(Transform _transform)
	{
		GameObject effectObject = Resources.Load<GameObject>("Prefabs/Landslide");
		if (effectObject)
		{
			effectObject = Instantiate(effectObject);
			effectObject.transform.SetParent(_transform);
			effectObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			effectObject.transform.localScale = new Vector3(1f,1f,1f);
			effectObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			effectObject.SetActive(false);

		}

		return effectObject;
	}

	public LandslideEffect GetLandslideEffect()
	{
		if (this.landslideEffect != null) return landslideEffect;
		GameObject effectObject = Prefab.LoadPrefab("Events/LandslideEffect");
		LandslideEffect output = null;
		if (effectObject) {
			effectObject.transform.SetParent(Camera.main.transform);
			effectObject.transform.localPosition = new Vector3(0f, 0f, 10f);
			effectObject.SetActive(false);
			output = effectObject.GetComponent<LandslideEffect>();
			landslideEffect = output;
		}
		return output;
	}

	public GameObject MakeWorldTsunami(Transform _transform)
	{
		GameObject effectObject = Resources.Load<GameObject>("Prefabs/Tsunami");
		if (effectObject)
		{
			effectObject = Instantiate(effectObject);
			effectObject.transform.SetParent(_transform);
			effectObject.transform.localPosition = new Vector3(0f, 5f, 10f);
			effectObject.transform.localScale = new Vector3(3f,1f,1f);
			effectObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			effectObject.SetActive(false);

		}

		return effectObject;
	}

	public FloodEffect GetFloodEffect()
	{
		if (this.floodEffect != null) return floodEffect;
		GameObject effectObject = Prefab.LoadPrefab("Events/FloodEffect");
		FloodEffect output = null;
		if (effectObject) {
			effectObject.transform.SetParent(Camera.main.transform);
			effectObject.transform.localPosition = new Vector3(20f, 0f, 10f);
			effectObject.SetActive(false);
			output = effectObject.GetComponent<FloodEffect>();
			floodEffect = output;
		}
		return output;
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

    public EarthquakeEffect GetEarthquakeEffect() {
        if (this.earthquakeEffect != null) return earthquakeEffect;
        GameObject effectObject = Prefab.LoadPrefab("Events/EarthquakeEffect");
        EarthquakeEffect output = null;
        if (effectObject) {
            effectObject.transform.SetParent(transform);
            effectObject.transform.localPosition = Vector3.zero;
            effectObject.SetActive(false);
            output = effectObject.GetComponent<EarthquakeEffect>();
            earthquakeEffect = output;
        }
        return output;
    }

    /*일정 시간동안 이벤트에 의해 데미지 받음
     * 60데미지 120초동안
     */
    public void StartDamagedByEvent(int damage, int time)
    {
        Timer damageTimer = new Timer();

    }

    public void ApplyEventStat(WeatherType type)
    {
        if (type.Equals(WeatherType.Cyclone))
        {
        }
    }

}
		

