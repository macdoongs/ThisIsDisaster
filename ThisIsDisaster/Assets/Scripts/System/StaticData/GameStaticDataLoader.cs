using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;
using GameStaticData;

public class GameStaticDataLoader {
    static string _localizeListXmlPath = "xml/localize/_localizeList";
    private static GameStaticDataLoader _loader = null;
    public static GameStaticDataLoader Loader {
        get {
            if (_loader == null) {
                _loader = new GameStaticDataLoader();
            }
            return _loader;
        }
    }
    
    public static string CurrentLoadLanguage
    {
        get;
        private set;
    }

    
    public GameStaticDataLoader() { }

    public void LoadAll() {

        SetLanguage("kr");//make kr as Variables
    }

    public void SetLanguage(string language) {
        CurrentLoadLanguage = language;
        LoadLocalizeTexts();
    }

    public TextAsset GetTextAsset(string src) {
        TextAsset output = null;
        output = Resources.Load<TextAsset>(src);
        if (output == null) {
#if UNITY_EDITOR
            Debug.LogError("Cannot find file : " + src);
#endif
        }
        return output;
    }

    public XmlDocument GetXmlDocuments(string src) {
        TextAsset textAsset = GetTextAsset(src);
        if (!textAsset) return null;
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(textAsset.text);
        return doc;
    }

    public void LoadLocalizeTexts() {
        LocalizeTextDataLoader loader = new LocalizeTextDataLoader();
        loader.Initialize(_localizeListXmlPath, null);
        loader.LoadData();

        LocalizeTextDataModel.Instance.Init(loader.GetLoadedData());
    }
}

namespace GameStaticData {

    public class StaticDataLoader
    {
        public delegate void OnLoadAction();
        protected string _path = "";
        protected OnLoadAction _loadedAction = null;

        public virtual void Initialize(string filePath, OnLoadAction loadedAction = null)
        {
            _path = filePath;
            _loadedAction = loadedAction;
        }

        public virtual void LoadData()
        {
            if (Load())
            {
                OnLoaded();
            }
        }

        protected virtual bool Load()
        {
            if (string.IsNullOrEmpty(_path)) return false;
            //do smth
            return true;
        }

        public virtual void OnLoaded()
        {
            if (_loadedAction != null)
            {
                _loadedAction();
            }
        }
    }

}

