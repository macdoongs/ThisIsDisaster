using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkComponents;

public enum GameState
{
    Lobby,
    Stage,
    None
}

public enum GameNetworkType {
Single,
Multi,
None//Lobby etc
}

public class GlobalParameters {
    public static GlobalParameters Param { get { return GlobalGameManager.Param; } }
    public int accountId = 0;
    public string accountName = "";
    public bool isHost = false;
    public bool isConnected = false;
    public bool isDisconnnected = false;

    public int AdditionalPortNum = 0;
    
    public void Init() { }
}

public class GlobalGameManager {

    private static GlobalGameManager _instance = null;
    public static GlobalGameManager Instance {
        get {
            if (_instance == null) {
                _instance = new GlobalGameManager();
                _instance.Init();
            }
            return _instance;
        }
    }

    public static GlobalParameters Param {
        get { return Instance.param; }
    }

    public DevelopmentConsole developmentConsole;
    public GlobalParameters param = null;
    
    [ReadOnly]
    public GameState GameState = GameState.Lobby;

    public GameNetworkType GameNetworkType {
        private set;
        get;
    }

    private void Init()
    {
        //if (Instance != null) {
        //    //Destroy(gameObject); return;
        //}
        //Instance = this;
        //DontDestroyOnLoad(gameObject);

        param = new GlobalParameters();
        param.Init();

        GameStaticDataLoader.Loader.LoadAll();
        GameStaticData.ItemDataLoader itemLoader = new GameStaticData.ItemDataLoader();
        itemLoader.Initialize(GameStaticData.ItemDataLoader._itemXmlFilePath);
        itemLoader.LoadData();

        GameStaticData.NPCDataLoader npcLoader = new GameStaticData.NPCDataLoader();
        npcLoader.Initialize(GameStaticData.NPCDataLoader._npcXmlFilePath);
        npcLoader.LoadData();

        GameStaticData.EnvironmentDataLoader envLoader = new GameStaticData.EnvironmentDataLoader();
        envLoader.Initialize(GameStaticData.EnvironmentDataLoader._envXmlFilePath);
        envLoader.LoadData();

        GameStaticData.StageInfoDataLoader stageLoader = new GameStaticData.StageInfoDataLoader();
        stageLoader.Initialize(GameStaticData.StageInfoDataLoader._xmlPath);
        stageLoader.LoadData();
        
        //LocalizeTextDataModel.Instance.LogAllData();

        GameObject networkObject = GameObject.Find("NetworkModule");
        if (networkObject) {
            NetworkModule network = networkObject.GetComponent<NetworkModule>();
            if (network) {
                
            }
        }

    }
    // Use this for initialization
    void Start () {
        //developmentConsole.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update ()
    {
        //if (Input.GetKeyDown(KeyCode.Tab))
        //{
        //    if (developmentConsole.gameObject.activeInHierarchy)
        //    {
        //        developmentConsole.Close();
        //    }
        //    else
        //    {
        //        developmentConsole.Open();
        //    }
        //}
    }

    public void SetGameState(GameState state) {
        Debug.Log("Set State: " + state);
        GameState = state;
    }
}
