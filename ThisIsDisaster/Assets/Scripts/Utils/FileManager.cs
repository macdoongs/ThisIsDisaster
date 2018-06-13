using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class FileManager {
    private static FileManager _instance = null;
    public static FileManager Instance {
        get {
            if (_instance == null) {
                _instance = new FileManager();
            }
            return _instance;
        }
    }

    public string Persistent = "/Files/";
    public string LocalData = "/LocalData/";

    void CheckDataPath() {
        DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath + Persistent);
        if (!di.Exists) di.Create();

        di = new DirectoryInfo(Application.dataPath + LocalData);
        if (!di.Exists) di.Create();
    }

    public string GetPath(string path, string format) {
        return Application.persistentDataPath + Persistent + path + format;
    }

    public string GetPath(ISavedData saved, string format) {
        return GetPath(saved.GetPath(), format);
    }

    public bool SaveFileData(string path, Dictionary<string, object> data, string format = ".dat")
    {
        try
        {
            CheckDataPath();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(GetPath(path, format), FileMode.Create);
            bf.Serialize(file, data);
            file.Close();
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            return false;
        }
        return true;
    }

    public bool ExistFile(ISavedData savedData, string format = ".dat")
    {
        string path = GetPath(savedData.GetPath(), format);
        return File.Exists(path);
    }

    public bool LoadFileData(string path, ref Dictionary<string, object> data, string format = ".dat") {
        try
        {
            CheckDataPath();
            string fileSrc = GetPath(path, format);
            if (!File.Exists(fileSrc)) {
                return false;
            }

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(fileSrc, FileMode.Open);
            data = (Dictionary<string, object>)bf.Deserialize(file);
            file.Close();
        }
        catch (System.Exception e) {
            Debug.LogError(e);
            return false;
        }
        return true;
    }

    public bool SaveData(ISavedData savedData, string format = ".dat") {
        return SaveFileData(savedData.GetPath(), savedData.GetSavedData(), format);
    }

    public bool LoadData(ISavedData savedData, string format = ".dat") {
        Dictionary<string, object> loaded = new Dictionary<string, object>();
        bool output = LoadFileData(savedData.GetPath(), ref loaded, format);
        if (!output) return false;
        savedData.LoadData(loaded);
        return true;
    }

    public void DeleteData(ISavedData savedData, string format = ".dat") {
        string path = GetPath(savedData, format);
        if (File.Exists(path)) {
            File.Delete(path);
        }
    }

    public static bool TryGetValue<T>(Dictionary<string, object> dic, string name, ref T field) {
        object output;
        if (dic.TryGetValue(name, out output) && output is T) {
            field = (T)output;
            return true;
        }
        return false;
    }

    public string GetLocalTextData(string src, string format) {
        CheckDataPath();
        string path = Application.dataPath + LocalData + src + format;
        string output = "";
        if (File.Exists(path)) {
            try
            {
                output = File.ReadAllText(path);
            }
            catch (System.Exception e){
                Debug.LogError(e);
                return "";
            }
        }

        return output;
    }

    public void SaveLocalTextData(string savedText, string src, string format = ".txt") {
        CheckDataPath();
        string path = Application.persistentDataPath + LocalData + src + format;
        try {
            File.WriteAllText(path, savedText);
        }
        catch {
        }

    }
}
