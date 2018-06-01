using System.Collections.Generic;

public interface ISavedData {
    Dictionary<string, object> GetSavedData();
    void LoadData(Dictionary<string, object> data);
    string GetPath();
}
