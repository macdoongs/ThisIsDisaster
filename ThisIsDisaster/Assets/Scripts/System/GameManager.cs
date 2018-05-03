using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager CurrentGameManager {
        private set;
        get;
    }

    private UnitControllerBase _localPlayer;
    private Dictionary<string, UnitControllerBase> _remotePlayer;

    public UnitControllerBase CommonPlayerObject;

    private void Awake()
    {
        CurrentGameManager = this;
    }

    // Use this for initialization
    void Start () {
        var localPlayer = MakePlayerCharacter("local", true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static UnitControllerBase MakePlayerCharacter(string id, bool isLocal) {
        UnitControllerBase output = null;
        if (!isLocal)
        {
            if (CurrentGameManager._remotePlayer.TryGetValue(id, out output))
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

        return output;
    }
}
