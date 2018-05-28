using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefab {
    /// <summary>
    /// Find Gameobject in Prefabs/
    /// </summary>
    /// <param name="prefabSrc"></param>
    /// <returns></returns>
    public static GameObject LoadPrefab(string prefabSrc) {
        GameObject load = Resources.Load<GameObject>("Prefabs/" + prefabSrc);
        if (load == null) {
#if UNITY_EDITOR
            Debug.LogError("Could not find prefab : " + prefabSrc);
#endif
        }
        GameObject copy = GameObject.Instantiate(load);
        return copy;
    }
}

public class GameManager : MonoBehaviour {
    public enum StageEventType {
        Init,
        Ready,
        Event_Generated,
        Event_Started,
        Event_Ended,
        Close
    }


    public class StageClockInfo {
        /*
        public const float STAGE_READY_TIME = 60f;
        public const float EVENT_GENERATE_TIME = 60f;
        public const float EVENT_RUN_TIME = 120f;
        public const float EVENT_END_TIME = 60f;
        public const float STAGE_CLOSE_TIME = 10f;
        */
        
        public const float STAGE_READY_TIME = 10f;
        public const float EVENT_GENERATE_TIME = 10f;
        public const float EVENT_RUN_TIME = 20f;
        public const float EVENT_END_TIME = 10f;
        public const float STAGE_CLOSE_TIME = 5f;
        public Timer stageTimer = new Timer();
        public StageEventType currentEventType = StageEventType.Init;
        public StageEventType nextEventType = StageEventType.Ready;
        public Timer eventHandleTimer = new Timer();

        public int EventGenerateCount = 2;

        WeatherType generatedEvent = new WeatherType();

        public void StartStage() {
            stageTimer.StartTimer();
            currentEventType = StageEventType.Init;
            nextEventType = StageEventType.Ready;
            SetNextEventTime(nextEventType);
            
        }

        public void SetEventGenerateCount(int count) {
            EventGenerateCount = count;
        }

        public void SetNextEventTime(float time) {
            eventHandleTimer.StartTimer(time);
        }

        public void SetNextEventTime(StageEventType type, bool isClose = false) {
            switch (type) {
                case StageEventType.Ready:
                    eventHandleTimer.StartTimer(STAGE_READY_TIME);
                    break;
                case StageEventType.Event_Generated:
                    eventHandleTimer.StartTimer(EVENT_GENERATE_TIME);
                    break;
                case StageEventType.Event_Started:
                    eventHandleTimer.StartTimer(EVENT_RUN_TIME);
                    break;
                case StageEventType.Event_Ended:
                    if (isClose)
                    {
                        eventHandleTimer.StartTimer(STAGE_CLOSE_TIME);
                    }
                    else {
                        eventHandleTimer.StartTimer(EVENT_END_TIME);
                    }
                    break;
            }
        }

        public void Update() {
            stageTimer.RunTimer();

            if (eventHandleTimer.RunTimer()) {
                ExecuteNextStep();
            }
        }

        void ExecuteNextStep()
        {
            Debug.LogError(currentEventType + " Expired");
            switch (nextEventType)
            {
                case StageEventType.Ready:
                    //generate next event
                    GenerateEvent();
                    SetNextEventTime(nextEventType);
                    nextEventType = StageEventType.Event_Generated;
                    break;
                case StageEventType.Event_Generated:
                    //start current generated event
                    StartCurrentEvent();
                    SetNextEventTime(nextEventType);
                    nextEventType = StageEventType.Event_Started;
                    break;
                case StageEventType.Event_Started:
                    //end current event
                    EndCurrentEvent();
                    SetNextEventTime(nextEventType);
                    nextEventType = StageEventType.Event_Ended;
                    break;
                case StageEventType.Event_Ended:
                    //generate next event or close stage
                    if (EventGenerateCount > 0)
                    {
                        GenerateEvent();
                        SetNextEventTime(nextEventType);
                        nextEventType = StageEventType.Event_Generated;
                    }
                    else
                    {
                        SetNextEventTime(nextEventType, true);
                        nextEventType = StageEventType.Close;
                    }

                    /*
                     if MAKE_NEXT_EVENT
            SetNextEventTime(nextEventType);
                        nextEventType = StageEventType.Event_Generated;
                     else
            SetNextEventTime(nextEventType, true);
                        nextEventType = StageEventType.Close;
                     */
                    break;
                case StageEventType.Close:
                    //close game
                    EndStage();
                    break;
            }

            currentEventType = nextEventType;
        }

        void EndCurrentEvent()
        {
            EventManager.Manager.EndEvent(generatedEvent);
        }

        void StartCurrentEvent()
        {
            EventManager.Manager.OnStart(generatedEvent);

        }

        void GenerateEvent()
        {
            EventGenerateCount--;
            generatedEvent = GameManager.CurrentGameManager.GetWeatherType();
            EventManager.Manager.OnGenerate(generatedEvent);
            //EventManager.Manager.OnStart(generatedEvent);
        }

        void EndStage() {
            CurrentGameManager.EndStage();
            
        }
    }

    public static GameManager CurrentGameManager {
        private set;
        get;
    }

    public Dictionary<int, UnitControllerBase> RemotePlayer
    {
        get
        {
            return _remotePlayer;
        }
    }

    private UnitControllerBase _localPlayer;
    private Dictionary<int, UnitControllerBase> _remotePlayer = null;

    public UnitControllerBase CommonPlayerObject;

    public ClimateType CurrentStageClimateTpye = ClimateType.Island;

    private StageClockInfo _stageClock = new StageClockInfo();
    private StageGenerator.ClimateInfo _currentClimateInfo = null;

    private void Awake()
    {
        CurrentGameManager = this;
        //Init();
    }

    /// <summary>
    //  
    /// </summary>
    public void Init() {
        _remotePlayer = new Dictionary<int, UnitControllerBase>();

        GenerateWorld(StageGenerator.Instance.ReadNextValue());

        var localPlayer = MakePlayerCharacter(GlobalParameters.Param.accountName,
            GlobalParameters.Param.accountId, true);

    }

    public void GenerateWorld(int seed)
    {
        CurrentStageClimateTpye = StageGenerator.Instance.GetRandomClimateType();

        StageGenerator.ClimateInfo info = StageGenerator.Instance.GetClimateInfo(CurrentStageClimateTpye);
        CellularAutomata.Instance.MaxHeightLevel = info.MaxHeightLevel;
        List<string> tileSrc = new List<string>(info.tileSpriteSrc.Values);
        RandomMapGenerator.Instance.SetTileSprite(tileSrc);

        CellularAutomata.Instance.SetSeed(seed);

        CellularAutomata.Instance.GenerateMap();

        //generate world by input
        try
        {
            foreach (var env in info.envInfoList)
            {
                if (!EnvironmentManager.Manager.IsValidateId(env.id)) continue;
                int count = StageGenerator.Instance.ReadNextValue(env.min, env.max);
                var coords = CellularAutomata.Instance.GetRoomsCoord(env.height, count);

                for (int i = 0; i < count; i++)
                {
                    var model = EnvironmentManager.Manager.MakeEnvironment(env.id);
                    TileUnit tile = RandomMapGenerator.Instance.GetTile(coords[i].tileX, coords[i].tileY);
                    model.UpdatePosition(tile.transform.position);
                }
            }
        }
        catch (System.Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError(e);
#endif
        }

        NPCManager.Manager.SetNpcGenInfo(info.npcInfoList);
        NPCManager.Manager.CheckGeneration();
        this._currentClimateInfo = info;

    }

    public WeatherType GetWeatherType() {
        return _currentClimateInfo.GetNextWeather();
    }

    // Use this for initialization
    void Start () {

        if (NetworkComponents.NetworkModule.Instance != null)
        {
            //NetworkComponents.NetworkModule.Instance.RegisterReceiveNotification(
            //    NetworkComponents.PacketId.Coordinates, OnReceiveCharacterCoordinate);
        }

        GlobalGameManager.Instance.SetGameState(GameState.Stage);

        //Init();
        //Debug.LogError("Stage Started");
        //StartStage();
    }
	
	// Update is called once per frame
	void Update ()
    {
        _stageClock.Update();
        Notice.Instance.Send(NoticeName.Update);
	}

    public void StartStage() {
        _stageClock.StartStage();
    }

    public void EndStage() {
        //do smth
        Debug.LogError("Stage Ended");
        InGameUIScript.Instance.StageClear();
    }

    public UnitControllerBase GetLocalPlayer() {
        return _localPlayer;
    }

    public static UnitControllerBase MakePlayerCharacter(string name, int id, bool isLocal) {
        UnitControllerBase output = null;
        if (!isLocal)
        {
            if (CurrentGameManager.RemotePlayer.TryGetValue(id, out output))
            {
                return output;
            }
        }
        else
        {
            if (CurrentGameManager._localPlayer != null) {
                return CurrentGameManager._localPlayer;
            }
        }

        GameObject copy = Instantiate(CurrentGameManager.CommonPlayerObject.gameObject);
        copy.transform.SetParent(CurrentGameManager.transform);
        copy.transform.localPosition = Vector3.zero;
        copy.transform.localRotation = Quaternion.Euler(Vector3.zero);
        copy.transform.localScale = Vector3.one;
        output = copy.GetComponent<UnitControllerBase>();
        output.SetUnitName(name);

        PlayerMoveController moveScript = copy.GetComponent<PlayerMoveController>();
        if (moveScript) {
            if (!isLocal) {
                moveScript.enabled = false;
            }
        }

        if (isLocal)
        {
            output.behaviour.IsRemoteCharacter = false;
            CurrentGameManager._localPlayer = output;
            Notice.Instance.Send(NoticeName.LocalPlayerGenerated);
            //attach chase
            CurrentGameManager.MakeCameraMoveScript(output.gameObject);
        }
        else {
            output.behaviour.IsRemoteCharacter = true;
            CurrentGameManager.RemotePlayer.Add(id, output);
        }

        return output;
    }

    void MakeCameraMoveScript(GameObject attach) {
        Chasing script = Camera.main.gameObject.GetComponent<Chasing>();
        if (!script) {
            script = Camera.main.gameObject.AddComponent<Chasing>();
        }
        script.Target = attach;
    }

    public void OnReceiveCharacterCoordinate(NetworkComponents.PacketId packetId, int packetSender, byte[] data) {
        UnitControllerBase controller = null;
        if (RemotePlayer.TryGetValue(packetSender, out controller))
        {
            NetworkComponents.CharacterMovingPacket packet = new NetworkComponents.CharacterMovingPacket(data);
            NetworkComponents.CharacterData charData = packet.GetPacket();

            //Debug.LogError("Position Info " + packetSender);
            controller.OnReceiveCharacterCoordinate(charData);
        }
    }
}
