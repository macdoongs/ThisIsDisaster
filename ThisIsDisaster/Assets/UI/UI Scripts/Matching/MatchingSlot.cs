using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchingSlot : MonoBehaviour {
    [Header("Player Info")]
    public Text PlayerLevel;
    public Text PlayerName;
    public Text PlayerState;
    //portriat omitted

    public Image ReadyState;
    [Header("Block State")]
    public CanvasGroup Block;
    [Space(10)]
    public Button Input;

    private int _accountId = -1;
    public int AccountId {
        get { return _accountId; }
    }

    private bool _isReady = false;
    public bool IsReady {
        get { return _isReady; }
    }

    public void SetBlockState(bool state) {
        Block.gameObject.SetActive(state);
    }

	// Use this for initialization
	void Start () {
        SetBlockState(true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetPlayer(int AccountId, int level, string name, bool isLocalPlayer) {
        PlayerLevel.text = "Lv. " + level;
        PlayerName.text = name;

        SetBlockState(false);

        //SetPlayerReady(false);
        this._accountId = AccountId;

        Input.interactable = isLocalPlayer;
        _isReady = false;
    }

    public void ClearPlayer() {
        SetBlockState(true);
        SetPlayerReady(false);
        _accountId = -1;
    }

    public void SetPlayerReady(bool isReady) {
        _isReady = isReady;
        ReadyState.gameObject.SetActive(isReady);
        PlayerState.text = isReady ? "Ready" : "Waiting";
    }

    public void OnClickReady() {
        if (_accountId == -1) return;
        SetPlayerReady(!_isReady);
        if (!GlobalGameManager.Instance.IsHost)
        {
            NetworkComponents.GameServer.Instance.SendMatchingReady(_isReady);
        }
        else {
            NetworkComponents.GameServer.Instance.SetHostReady(_isReady);
        }
        //NetworkComponents.GameServer.Instance.SendGameServerRequest(NetworkComponents.GameServerRequestType.MatchingData);
    }
    
}
