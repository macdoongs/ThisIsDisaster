using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using NPC;
using System;

public class DevelopmentConsole : MonoBehaviour
{
    public Text fullLogArea;
    public Transform FullLogParent;
    public Text extractedLogArea;
    public Transform ExtractedLogParent;

    [Space(10)]
    public InputField input;
    StringBuilder fullLog = new StringBuilder();
    StringBuilder extractedLog = new StringBuilder();

    public GameObject ActiveControl;

    public bool ShowFullLog = true;

    public void OnEndEdit() {
        string txt = input.text;
        if (!string.IsNullOrEmpty(txt)) {
            //execute
            if (txt.ToLower() == "ChangeLanguage".ToLower())
            {
                if (GameStaticDataLoader.CurrentLoadLanguage == "kr")
                {
                    GameStaticDataLoader.Loader.SetLanguage("en");
                }
                else
                {
                    GameStaticDataLoader.Loader.SetLanguage("kr");
                }
                LocalizeTextDataModel.Instance.LogAllData();
            }
            else {
                ConsoleScript.Instance.OnInput(txt);
            }
        }

        input.text = "";
        
    }

    private void Awake()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void Start()
    {
        input.placeholder.enabled = false;
        Close();
    }

    private void OnEnable()
    {
        SetText();
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            if (ActiveControl.activeInHierarchy)
            {
                Close();
            }
            else {
                Open();
            }
        }

        if (ShowFullLog)
        {
            if (!FullLogParent.gameObject.activeInHierarchy) {
                FullLogParent.gameObject.SetActive(true);
            }
            if (ExtractedLogParent.gameObject.activeInHierarchy) {
                ExtractedLogParent.gameObject.SetActive(false);
            }
        }
        else
        {
            if (FullLogParent.gameObject.activeInHierarchy)
            {
                FullLogParent.gameObject.SetActive(false);
            }
            if (!ExtractedLogParent.gameObject.activeInHierarchy)
            {
                ExtractedLogParent.gameObject.SetActive(true);
            }
        }
    }

    public void Close() {
        input.text = "";
        ActiveControl.SetActive(false);
    }

    public void Open() {
        input.text = "";
        ActiveControl.SetActive(true);
    }
    
    public void HandleLog(string logString, string stackTrace, LogType logType) {
        if (logType != LogType.Error && logType != LogType.Log) return;
        string fullFormat = "<color={2}>{0}\r\n{1}</color>\r\n\r\n";
        string extractedFormat = "<color={1}>{0}</color>\r\n\r\n";
        Color logColor = Color.black;
        if (logType == LogType.Error) {
            logColor = Color.red;
        }
        string colorText = ColorUtility.ToHtmlStringRGBA(logColor);
        fullLog.AppendFormat(fullFormat, logString, stackTrace, colorText);
        extractedLog.AppendFormat(extractedFormat, logString, colorText);
        if (isActiveAndEnabled) {
            SetText();
        }
    }

    void SetText() {
        fullLogArea.text = fullLog.ToString();
        extractedLogArea.text = extractedLog.ToString();
    }
}

public class ConsoleScript {
    public class Command {
        public delegate void Action(params object[] param);

        public string name = "";
        public Action action = null;
        public List<object> paramTypes = new List<object>();
        public List<object> parameters = new List<object>();
        public bool ignoreParameterLength = false;
        

        public void SetParameters(params object[] param) {
            paramTypes.Clear();
            foreach (object o in param) {
                paramTypes.Add(o);
            }
        }

        public void SetAction(Action a) {
            action = a;
        }

        public Command Copy() {
            Command output = new Command()
            {
                name = name,
                paramTypes = new List<object>(paramTypes.ToArray()),
                action = action
            };
            return output;
        }

        public static int ParseInt(string input) {
            return (int)float.Parse(input);
        }

        public static float ParseFloat(string input) {
            return float.Parse(input);
        }

        public static long ParseLong(string input) {
            return long.Parse(input);
        }

        public static bool ParseBool(string input) {
            return bool.Parse(input);
        }
        
    }

    const char _sep = ' ';

    private static ConsoleScript _instance = null;
    public static ConsoleScript Instance {
        get { if (_instance == null) _instance = new ConsoleScript(); return _instance; }
    }

    Dictionary<string, Command> commands = new Dictionary<string, Command>();

    public ConsoleScript() {
        Command makeNpc = new Command()
        {
            name = "Make NPC"
        };
        makeNpc.SetParameters(typeof(int));
        makeNpc.SetAction(MakeNPC);
        commands.Add("npc", makeNpc);

        Command makeNpcMany = new Command() {
            name = "Make NPC Many"
        };
        makeNpcMany.SetParameters(typeof(int), typeof(int));
        makeNpcMany.SetAction(MakeNPCMany);
        commands.Add("npcmany", makeNpcMany);

        Command invokeWeather = new Command()
        {
            name = "Invoke New Weather"
        };
        invokeWeather.SetAction(MakeWeathers);
        commands.Add("invoke", invokeWeather);

        Command startWeather = new Command()
        {
            name = "Start Current Weathers",
        };
        startWeather.ignoreParameterLength = true;
        startWeather.SetAction(StartAllWeathers);
        commands.Add("startweather", startWeather);


        Command clearWeather = new Command()
        {
            name = "Clear Current Weathers",
        };
        clearWeather.ignoreParameterLength = true;
        clearWeather.SetAction(StopAllWeathers);
        commands.Add("stopweather", clearWeather);

        Command astarDebug = new Command()
        {
            name = "Make A* Debug NPC"
        };
        astarDebug.ignoreParameterLength = true;
        astarDebug.SetAction(AstarDebugNPC);
        commands.Add("astar", astarDebug);

        Command makeTree = new Command()
        {
            name = "Make Tree"
        };
        makeTree.ignoreParameterLength = true;
        makeTree.SetAction(MakeTreeEnv);
        commands.Add("tree", makeTree);

        Command makeTreeMany = new Command()
        {
            name = "Make Tree"
        };
        makeTreeMany.SetParameters(typeof(int));
        makeTreeMany.SetAction(MakeTreeManyEnv);
        commands.Add("treemany", makeTreeMany);

        Command makeShelter = new Command() {
            name = "Make Shelter"
        };
        makeShelter.ignoreParameterLength = true;
        makeShelter.SetAction(MakeShelterDev);
        commands.Add("shelter", makeShelter);

        Command makeDropItem = new Command() {
            name = "Make Drop Item"
        };

        makeDropItem.SetParameters(typeof(int));
        makeDropItem.SetAction(MakeItem);
        commands.Add("drop", makeDropItem);
    }

    public void OnInput(string commandText) {
        string[] split = commandText.Split(_sep);
        List<string> meaningful = new List<string>();
        for (int i = 0; i < split.Length; i++) {
            if (string.IsNullOrEmpty(split[i])) continue;
            meaningful.Add(split[i]);
        }

        if (meaningful.Count <= 0) return;
        string command = meaningful[0];
        meaningful.RemoveAt(0);
        Execute(command.ToLower(), meaningful.ToArray());
    }

    void PrintConsole(Command c, params string[] p) {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("[{0}] ", c.name);
        for(int i = 0; i < p.Length; i++){
            var v = p[i];
            sb.Append(v);
            if (i < p.Length -1)
                sb.Append(", ");
        }
        Debug.Log(sb.ToString());
    }

    public void Execute(string commandType, params string[] param) {
        Command targetCommand = null;

        if (commands.TryGetValue(commandType, out targetCommand))
        {
            PrintConsole(targetCommand, param);
            Command c = targetCommand.Copy();

            try
            {
                if (!targetCommand.ignoreParameterLength) {

                    if (targetCommand.paramTypes.Count != 0)
                    {

                        for (int i = 0; i < targetCommand.paramTypes.Count; i++)
                        {
                            object curType = targetCommand.paramTypes[i];
                            if (curType == typeof(int))
                            {
                                int parse = Command.ParseInt(param[i]);
                                c.parameters.Add(parse);
                            }
                            else if (curType == typeof(string))
                            {
                                c.parameters.Add(param[i]);
                            }

                        }
                    }
                    else {
                        c.parameters.AddRange(param);
                    }
                }
            }
            catch {
                return;
            }

            try
            {
                c.action(c.parameters.ToArray());
            }
            catch (System.Exception e) {
#if UNITY_EDITOR
                Debug.LogError("Console Error : " + System.Environment.NewLine + e);
#endif
            }
        }
    }

    void MakeNPC(params object[] p)
    {
        var model = NPCManager.Manager.MakeNPC((int)p[0]);
        TileUnit randTile = RandomMapGenerator.Instance.GetRandomTileByHeight(1);
        model.SetCurrentTileForcely(randTile);
    }

    void MakeNPCMany(params object[] p) {
        int count = (int)p[1];
        int width = RandomMapGenerator.Instance.Width;
        int height = RandomMapGenerator.Instance.Height;

        int xRange = width / 2;
        int yRange = height / 2;
        for (int i = 0; i < count; i++) {
            var model = NPCManager.Manager.MakeNPC((int)p[0]);

            TileUnit randTile = RandomMapGenerator.Instance.GetRandomTileByHeight(1);
            model.SetCurrentTileForcely(randTile);
        }
    }

    /// <summary>
    /// p -> weatehr (string)
    /// </summary>
    /// <param name="p"></param>
    void MakeWeathers(params object[] p) {
        foreach (object o in p) {
            if (o is string) {
                var type = ParseEventType((string)o);
                if (type == WeatherType.None) continue;
                EventManager.Manager.OnGenerate(type);
                Debug.Log("Generated Weather : "+type);
            }
        }
    }

    void StartAllWeathers(params object[] p) {
        var list = EventManager.Manager.GetAllEvents();
        foreach (var e in list) {
            if (e.IsStarted == false) {
                EventManager.Manager.OnStart(e.type);
            }
        }
    }

    void StopAllWeathers(params object[] p) {
        var list = EventManager.Manager.GetAllEvents();
        foreach (var e in list) {
            if (e.IsStarted) {
                EventManager.Manager.OnEnd(e.type);
            }
        }
    }

    WeatherType ParseEventType(string eventString) {
        WeatherType output = WeatherType.None;
        switch (eventString.ToLower()) {
            case "cyclone":     output = WeatherType.Cyclone; break;
            case "flood":       output = WeatherType.Flood; break;
            case "yellowdust":  output = WeatherType.Yellowdust; break;
            case "drought":     output = WeatherType.Drought; break;
            case "fire":        output = WeatherType.Fire; break;
            case "earthquake":  output = WeatherType.Earthquake; break;
            case "lightning":   output = WeatherType.Lightning; break;
            case "landsliding": output = WeatherType.Landsliding; break;
            case "heavysnow":   output = WeatherType.Heavysnow; break;
        }
        return output;
    }

    public void AstarDebugNPC(params object[] p) {
        NPCModel model = NPCManager.Manager.MakeNPC(0);
        model.UpdatePosition(GameManager.CurrentGameManager.GetLocalPlayer().transform.position);
        AstarDebugger.Debugger.DebugUnit = model.Unit;
    }

    public void MakeTreeEnv(params object[] p) {
        Environment.EnvironmentModel model = EnvironmentManager.Manager.MakeEnvironment(0);
        List<CellularAutomata.Coord> randomCoord = new List<CellularAutomata.Coord>();
        randomCoord = CellularAutomata.Instance.GetRoomsCoord(3, 1);
        TileUnit randomTile = RandomMapGenerator.Instance.GetTile(randomCoord[0].tileX, randomCoord[0].tileY);
        model.UpdatePosition(randomTile.transform.position);// GameManager.CurrentGameManager.GetLocalPlayer().transform.position);
    }
    
    public void MakeTreeManyEnv(params object[] p)
    {
        int num = (int)p[0];
        List<CellularAutomata.Coord> randomCoord = new List<CellularAutomata.Coord>();

        randomCoord = CellularAutomata.Instance.GetRoomsCoord(3, num);
        for (int i = 0; i < num; i++)
        {
            Environment.EnvironmentModel model = EnvironmentManager.Manager.MakeEnvironment(0);
            TileUnit randomTile = RandomMapGenerator.Instance.GetTile(randomCoord[i].tileX, randomCoord[i].tileY);
            model.UpdatePosition(randomTile.transform.position);
        }
    }

    public void MakeShelterDev(params object[] p) {
        TileUnit current = RandomMapGenerator.Instance.GetTile(GameManager.CurrentGameManager.GetLocalPlayer().transform.position);
        Shelter.ShelterManager.Instance.MakeRandomShelter(current);
    }

    public void MakeItem(params object[] p) {
        TileUnit current = RandomMapGenerator.Instance.GetTile(GameManager.CurrentGameManager.GetLocalPlayer().transform.position);
        var item = ItemManager.Manager.MakeDropItem((int)p[0], current);
    }
}