using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalGameManager : MonoBehaviour {

    public static GlobalGameManager GlobalGameManagement {
        private set;
        get;
    }

    public DevelopmentConsole developmentConsole;

    private void Awake()
    {
        if (GlobalGameManagement != null) {
            Destroy(gameObject); return;
        }
        GlobalGameManagement = this;
        DontDestroyOnLoad(gameObject);

        GameStaticDataLoader.Loader.LoadAll();

        LocalizeTextDataModel.Instance.LogAllData();
    }
    // Use this for initialization
    void Start () {
        developmentConsole.gameObject.SetActive(false);	
	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Tab)) {
            if (developmentConsole.gameObject.activeInHierarchy)
            {
                developmentConsole.Close();
            }
            else {
                developmentConsole.Open();
            }
        }
#endif
    }
}
