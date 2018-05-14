using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using NPC;
using System;

public class DevelopmentConsole : MonoBehaviour
{
    public InputField input;
 

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

    private void Start()
    {

        input.placeholder.enabled = false;
    }

    public void Close() {
        input.text = "";
        gameObject.SetActive(false);
    }

    public void Open() {
        input.text = "";
        gameObject.SetActive(true);
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
}