using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        
            //FOR MIDDLE PRES OR DEMO
        public const float STAGE_READY_TIME = 10f;
        public const float EVENT_GENERATE_TIME = 20f;
        public const float EVENT_RUN_TIME = 70f;
        public const float EVENT_END_TIME = 20f;
        public const float STAGE_CLOSE_TIME = 5f;

        public const float MONSTER_REGEN_TIME = 30f;

        public Timer stageTimer = new Timer();
        public StageEventType currentEventType = StageEventType.Init;
        public StageEventType nextEventType = StageEventType.Ready;
        public Timer eventHandleTimer = new Timer();
        public Timer monsterRegenTimer = new Timer();

        private bool clockEnabled = false;
        public int EventGenerateCount = 2;

        WeatherType generatedEvent = new WeatherType();

        public void StartStage() {
            stageTimer.StartTimer();
            currentEventType = StageEventType.Init;
            nextEventType = StageEventType.Ready;
            SetNextEventTime(nextEventType, true);

            monsterRegenTimer.StartTimer(MONSTER_REGEN_TIME);
        }

        public void SetEventGenerateCount(int count) {
            EventGenerateCount = count;
        }

        public void SetNextEventTime(float time) {
            eventHandleTimer.StartTimer(time);
        }

        public void SetNextEventTime(StageEventType type, bool clock, bool isClose = false) {
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
                    eventHandleTimer.StartTimer(EVENT_END_TIME);
                    break;
                case StageEventType.Close:
                    eventHandleTimer.StartTimer(STAGE_CLOSE_TIME);
                    break;
            }
            if (clock)
            {
                CurrentGameManager.OnStartClock(eventHandleTimer.maxTime);
            }
            else {
                CurrentGameManager.OnEndClock();
            }
            clockEnabled = clock;
        }

        public void Update() {
            stageTimer.RunTimer();
            if (monsterRegenTimer.RunTimer()) {
                monsterRegenTimer.StartTimer(MONSTER_REGEN_TIME);
                //
                Notice.Instance.Send(NoticeName.MonsterRegen);
            }
            if (clockEnabled) {
                CurrentGameManager.UpdateClock(eventHandleTimer.elapsed, eventHandleTimer.maxTime);
            }

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
                    nextEventType = StageEventType.Event_Generated;
                    SetNextEventTime(nextEventType, true);
                    break;
                case StageEventType.Event_Generated:
                    //start current generated event
                    StartCurrentEvent();
                    nextEventType = StageEventType.Event_Started;
                    SetNextEventTime(nextEventType, true);
                    break;
                case StageEventType.Event_Started:
                    //end current event
                    EndCurrentEvent();
                    if (EventGenerateCount > 0)
                    {
                        nextEventType = StageEventType.Event_Ended;
                        SetNextEventTime(nextEventType, true);
                    }
                    else {
                        nextEventType = StageEventType.Close;
                        SetNextEventTime(nextEventType, true);
                    }
                    break;
                case StageEventType.Event_Ended:
                    GenerateEvent();
                    nextEventType = StageEventType.Event_Generated;
                    SetNextEventTime(nextEventType, true);
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
            Notice.Instance.Send(NoticeName.SaveGameLog, GameLogType.EventEnded, generatedEvent);

        }

        void StartCurrentEvent()
        {
            EventManager.Manager.OnStart(generatedEvent);
            Notice.Instance.Send(NoticeName.SaveGameLog, GameLogType.EventStarted, generatedEvent);

        }

        void GenerateEvent()
        {
            EventGenerateCount--;
            generatedEvent = GameManager.CurrentGameManager.GetWeatherType();
            Notice.Instance.Send(NoticeName.SaveGameLog, GameLogType.EventGenerated, generatedEvent);
            
            EventManager.Manager.OnGenerate(generatedEvent);
            //EventManager.Manager.OnStart(generatedEvent);
        }

        void EndStage() {
            CurrentGameManager.EndStage(true);

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

    public ClimateType CurrentStageClimateType = ClimateType.Island;

    private StageClockInfo _stageClock = new StageClockInfo();
    private StageGenerator.ClimateInfo _currentClimateInfo = null;

    public UnityEngine.UI.Text Clock;
    int[] fieldDropItems = new int[] {
            5,1,6,10001,20001,30001,310004
        };

    private void Awake()
    {
        if (GlobalGameManager.Instance != null) {

            GlobalGameManager.Instance.SetGameState(GameState.Stage);
        }
        CurrentGameManager = this;
        _remotePlayer = new Dictionary<int, UnitControllerBase>();
        //Init();
        if (Clock != null)
            Clock.gameObject.SetActive(false);
        NPCManager.Manager.Clear();

    }
    
    /// <summary>
    //  
    /// </summary>
    public void Init() {

        GenerateWorld(StageGenerator.Instance.ReadNextValue());

        TileUnit zeroTile = RandomMapGenerator.Instance.GetRandomTileByHeight_Sync(1);

        var localPlayer = MakePlayerCharacter(GlobalParameters.Param.accountName,
            GlobalParameters.Param.accountId, true);
        localPlayer.AccountId = GlobalParameters.Param.accountId;

        localPlayer.GetComponent<CharacterModel>().GetPlayerModel().SetCurrentTileForcely(zeroTile);

        if (NetworkComponents.NetworkModule.Instance != null)
        {
            NetworkComponents.NetworkModule.Instance.RegisterReceiveNotification(
                NetworkComponents.PacketId.Coordinates, OnReceiveCharacterCoordinate);
        }

        if (GlobalGameManager.Instance.GameNetworkType == GameNetworkType.Multi) {
            foreach (var kv in GlobalGameManager.Instance._remotePlayers) {
                var c = MakePlayerCharacter(kv.Value.ToString(), kv.Key, false);
                c.AccountId = kv.Value;

                c.GetComponent<CharacterModel>().GetPlayerModel().SetCurrentTileForcely(zeroTile);
            }
        }

        InGameUIScript.Instance.Init();
#if MIDDLE_PRES
        ProtoInit();
#endif
    }

    public void ProtoInit()
    {
        //make shelter
        //add shelter item
        var randTile = RandomMapGenerator.Instance.GetRandomTileByHeight_Sync(3);
        //Debug.LogError(randTile.x + " " + randTile.y);
        Shelter.ShelterManager.Instance.MakeRandomShelter(randTile);
        
        var list = CellularAutomata.Instance.GetRoomsCoord(3, 20);
        foreach (var v in list) {
            TileUnit tile = RandomMapGenerator.Instance.GetTile(v.tileX, v.tileY);
            ItemManager.Manager.MakeDropItem(fieldDropItems[StageGenerator.Instance.ReadNextValue(0, fieldDropItems.Length)], tile);
        }
    }
    
#if MIDDLE_PRES
#endif

    public void GenerateWorld(int seed)
    {
        CurrentStageClimateType = StageGenerator.Instance.GetRandomClimateType();

        StageGenerator.ClimateInfo info = StageGenerator.Instance.GetClimateInfo(CurrentStageClimateType);
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
                    model.SetCurrentTileForcely(tile);

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
        
    }

    
	
	// Update is called once per frame
	void Update ()
    {
        _stageClock.Update();
        Notice.Instance.Send(NoticeName.Update);

        if (Input.GetKeyDown(KeyCode.F12)) {
            EndStage(true);
        }
	}

    public void StartStage() {
        Notice.Instance.Send(NoticeName.SaveGameLog, GameLogType.StageStart, GlobalGameManager.Instance.GameNetworkType, CurrentStageClimateType);
        _stageClock.StartStage();
    }

    public void EndStage(bool isVictory) {
        //do smth
        Debug.LogError("Stage Ended");

        var script = Camera.main.gameObject.GetComponent<ScreenCapture>();
        if (script != null)
        {
            script.Capature();
        }

        Notice.Instance.Send(NoticeName.SaveGameLog, GameLogType.StageEnd, GlobalGameManager.Instance.GameNetworkType, isVictory ? "승리" : "패배", CurrentStageClimateType);

        if (isVictory)
            InGameUIScript.Instance.StageClear();
    }

    public UnitControllerBase GetLocalPlayer() {
        return _localPlayer;
    }

    public static UnitControllerBase MakePlayerCharacter(string name, int id, bool isLocal) {
        UnitControllerBase output = null;
        //Debug.LogError("Make Chararcter " + isLocal + " " + name);
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
                output.SetFlipPivot(moveScript.FlipPivot);
            }
        }

        if (isLocal)
        {
            output.GetComponent<CharacterModel>().SetInstance();
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

    public void OnReceiveCharacterCoordinate(int node, NetworkComponents.PacketId packetId, byte[] data) {
        UnitControllerBase controller = null;

        Debug.LogError("Coordinate from " + node);

        if (RemotePlayer.TryGetValue(node, out controller))
        {
            NetworkComponents.CharacterMovingPacket packet = new NetworkComponents.CharacterMovingPacket(data);
            NetworkComponents.CharacterData charData = packet.GetPacket();
            //Debug.Log(charData.ToString());
            //Debug.LogError("Position Info " + packetSender);
            controller.OnReceiveCharacterCoordinate(charData);
        }
    }

    public void OnStartClock(float maxTime) {
        if (!Clock) return;
        Clock.gameObject.SetActive(true);
        string clockTime = string.Format("{0}:{1}", ((int)(maxTime / 60)).ToString("D2"),((int)(maxTime % 60)).ToString("D2"));
        Clock.text = clockTime;
    }

    public void UpdateClock(float elap, float maxTime) {
        if (!Clock) return;
        float time = maxTime - elap;
        string clockTime = string.Format("{0}:{1}", ((int)(time / 60)).ToString("D2"), ((int)(time % 60)).ToString("D2"));
        Clock.text = clockTime;
    }

    public void OnEndClock() {
        if (!Clock) return;
        Clock.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GlobalGameManager.Instance.OnSceneLoaded();
        Debug.LogError("OnSceneLoaded: " + scene.name);
        //Debug.Log(mode);
    }

    public void CheckStartMultiStage() {
        StartCoroutine(StartMultiStage());
    }

    IEnumerator StartMultiStage() {
        while (true) {
            yield return new WaitForSeconds(1f);
            if (NetworkComponents.GameServer.Instance.IsLoadEnded()) {
                //start at 5 seconds
                //Debug.LogError("Should Start Game");
                if (GlobalGameManager.Instance.IsHost)
                {
                    DateTime now = DateTime.Now;
                    DateTime next = now.AddSeconds(5);
                    if (NetworkComponents.GameServer.Instance != null)
                    {
                        NetworkComponents.GameServer.Instance.SendStageStartTime(next);
                        StartMultiStage(next);
                    }
                }
                break;
            }
        }
    }

    IEnumerator StartStageDelayed(DateTime time) {
        while (true) {
            yield return new WaitForFixedUpdate();
            DateTime now = DateTime.Now;
            if (now >= time) {
                //start
                StartStage();
                break;
            }
        }
    }

    public void StartMultiStage(DateTime time)
    {
        StartCoroutine(StartStageDelayed(time));
    }

    public void OnRemoteChararcterAnimTrigger(int node, string anim) {
        UnitControllerBase control = null;
        if (RemotePlayer.TryGetValue(node, out control)) {
            PlayerMoveController ctrl = control.GetComponent<PlayerMoveController>();
            if (ctrl != null) {
                AnimatorUtil.SetTrigger(ctrl.PlayerMovementCTRL, anim);
            }
        }
    }

    public void OnRemoteCharacterItemInfoSetted(NetworkComponents.PlayerItemInfo info) {
        var ctrl = GetRemotePlayerById(info.accountId);
        if (ctrl != null) {
            CharacterModel model = ctrl.GetComponent<CharacterModel>();
            model.WearEquipmentNetwork(info.itemId, info.isAcquire);
        }
    }

    public void OnRemotePlayerStateInfoReceived(int node, int health, bool isDead) {
        UnitControllerBase ctrl = null;
        if (RemotePlayer.TryGetValue(node, out ctrl)) {
            CharacterModel model = ctrl.GetComponent<CharacterModel>();
            model.CurrentStats.Health = health;
            if (isDead) {
                model.SubtractHealth(model.CurrentStats.MaxHealth);
            }
        }
    }

    public UnitControllerBase GetRemotePlayerById(int id) {
        UnitControllerBase output = null;
        foreach (var kv in _remotePlayer) {
            if (kv.Value.AccountId == id) {
                output = kv.Value;
                break;
            }
        }
        return output;
    }
}
