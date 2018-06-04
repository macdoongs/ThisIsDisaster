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

    private List<StaticDataLoader> dataLoaders = new List<StaticDataLoader>();
    private List<StaticDataLoader> invalidatedLoaders = new List<StaticDataLoader>();

    
    public GameStaticDataLoader() { }

    public void LoaderInit() {
        dataLoaders.Clear();
        invalidatedLoaders.Clear();
        SetLanguage("kr");//make kr as Variables


        ItemDataLoader itemLoader = new ItemDataLoader();
        RecipeDataLoader recipeLoater = new RecipeDataLoader();
        NPCDataLoader npcLoader = new NPCDataLoader();
        EnvironmentDataLoader envLoader = new EnvironmentDataLoader();
        StageInfoDataLoader stageLoader = new StageInfoDataLoader();
        
        itemLoader.Initialize(ItemDataLoader._itemXmlFilePath);
        dataLoaders.Add(itemLoader);
        //itemLoader.LoadData();
        if (!itemLoader.Validtion()) {
            invalidatedLoaders.Add(itemLoader);
        }

        recipeLoater.Initialize(RecipeDataLoader._recipeXmlFilePath);
        dataLoaders.Add(recipeLoater);
        if (!recipeLoater.Validtion())
        {
            invalidatedLoaders.Add(recipeLoater);
        }
        //recipeLoater.LoadData();

        npcLoader.Initialize(NPCDataLoader._npcXmlFilePath);
        dataLoaders.Add(npcLoader);
        if (!npcLoader.Validtion())
        {
            invalidatedLoaders.Add(npcLoader);
        }
        //npcLoader.LoadData();

        envLoader.Initialize(EnvironmentDataLoader._envXmlFilePath);
        dataLoaders.Add(envLoader);
        if (!envLoader.Validtion())
        {
            invalidatedLoaders.Add(envLoader);
        }
        //envLoader.LoadData();

        stageLoader.Initialize(StageInfoDataLoader._xmlPath);
        dataLoaders.Add(stageLoader);
        if (!stageLoader.Validtion())
        {
            invalidatedLoaders.Add(stageLoader);
        }
        //stageLoader.LoadData();

        if (invalidatedLoaders.Count == 0) {
            LoadXmls();
        }
    }

    public void ReLoadInvalidatedLoaders() {
        //download is needed
        
    }

    public void LoadXmls() {
        foreach (var load in dataLoaders) {
            load.LoadData();
        }
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

    public int GetXmlHash(XmlDocument doc) {
        return doc.InnerText.GetHashCode();
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

        public virtual bool Validtion() {
            //networking to server
            var doc = GetDocument();
            if (doc == null) return false;
            
            ///TODO : REST API Sync

            return true;
        }

        public XmlDocument GetDocument() {
            if (string.IsNullOrEmpty(_path)) return null;
            return GameStaticDataLoader.Loader.GetXmlDocuments(_path);
        }

        
    }

}

