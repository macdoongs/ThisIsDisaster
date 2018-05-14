using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;

namespace GameStaticData
{
    public class LocalizeTextDataLoader : StaticDataLoader
    {
        const string fileFormat = "xml/Localize/{0}/{1}_{0}";

        Dictionary<string, string> _localizedTextList = new Dictionary<string, string>();

        protected override bool Load()
        {
            if (!base.Load()) return false;

            try
            {
                XmlDocument doc = GameStaticDataLoader.Loader.GetXmlDocuments(_path);

                XmlNodeList fileList = doc.SelectNodes("root/file");
                foreach (XmlNode node in fileList)
                {
                    string fileName = node.InnerText.Trim();
                    //load file and texts
                    LoadFileText(fileName);
                }

            }
            catch (Exception exeption)
            {
                return false;
            }
            return true;
        }

        void LoadFileText(string fileName)
        {
            try
            {

                XmlDocument doc = GameStaticDataLoader.Loader.GetXmlDocuments((GetLocalizeTextFilePath(fileName)));

                XmlNodeList nodeList = doc.SelectNodes("root/node");
                foreach (XmlNode node in nodeList)
                {
                    string key = node.Attributes.GetNamedItem("id").InnerText;
                    string text = node.InnerText.Trim();
                    try
                    {
                        _localizedTextList.Add(key, text);
                    }
                    catch (ArgumentException argumentException)
                    {
#if UNITY_EDITOR
                        Debug.LogError("ArgumentException At " + key + System.Environment.NewLine + argumentException.ToString());
#endif
                        continue;
                    }
                }

            }
            catch (Exception exception)
            {
#if UNITY_EDITOR
                Debug.LogError(string.Format("Failed To Load Localize Text File : {0}", fileName));

#endif
            }
        }

        string GetLocalizeTextFilePath(string path)
        {
            return string.Format(fileFormat, GameStaticDataLoader.CurrentLoadLanguage, path);
        }

        public Dictionary<string, string> GetLoadedData()
        {
            return _localizedTextList;
        }
    }
}

public class LocalizeTextDataModel
{
    private static LocalizeTextDataModel _instance = null;
    public static LocalizeTextDataModel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LocalizeTextDataModel();
            }
            return _instance;
        }
    }

    public const string failureText = "Unknown";

    private Dictionary<string, string> _textLib = null;
    public bool IsInit
    {
        get { return _textLib != null; }
    }

    public void Init(Dictionary<string, string> lib)
    {
        _textLib = lib;
    }

    public void LogAllData()
    {
        foreach (var kv in _textLib)
        {
            Debug.Log(kv.Key + " " + kv.Value);
        }
    }

    public string GetText(string key)
    {
        string output = "";
        if (!_textLib.TryGetValue(key, out output))
        {
            output = failureText;
        }
        return output;
    }
}