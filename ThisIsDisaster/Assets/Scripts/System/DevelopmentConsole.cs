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
        gameObject.SetActive(false);
    }

    public void Open() {
        input.text = "";
        gameObject.SetActive(true);
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
                for (int i = 0; i < targetCommand.paramTypes.Count; i++)
                {
                    object curType = targetCommand.paramTypes[i];
                    if (curType == typeof(int))
                    {
                        int parse = Command.ParseInt(param[i]);
                        c.parameters.Add(parse);
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

    void MakeNPC(params object[] p) {
        NPCManager.Manager.MakeNPC((int)p[0]);
    }

    void MakeNPCMany(params object[] p) {
        int count = (int)p[1];
        int width = RandomMapGenerator.Instance.Width;
        int height = RandomMapGenerator.Instance.Height;

        int xRange = width / 2;
        int yRange = height / 2;
        for (int i = 0; i < count; i++) {
            var model = NPCManager.Manager.MakeNPC((int)p[0]);
            int x = UnityEngine.Random.Range(xRange -10, xRange + 10);
            int y = UnityEngine.Random.Range(yRange - 10, yRange + 10);
            TileUnit tile = RandomMapGenerator.Instance.GetTile(x, y);

            Vector3 pos = model.Unit.transform.position;
            pos.x = tile.transform.position.x;
            pos.y = tile.transform.position.y;
            model.Unit.transform.position = pos;
        }
    }
}