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

    private int AccountId = -1;
    private bool _isReady = false;

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

        SetPlayerReady(false);
        this.AccountId = AccountId;

        Input.interactable = isLocalPlayer;
        _isReady = false;
    }

    public void ClearPlayer() {
        SetBlockState(true);
        SetPlayerReady(false);
        AccountId = -1;
    }

    public void SetPlayerReady(bool isReady) {
        _isReady = isReady;
        ReadyState.gameObject.SetActive(isReady);
        PlayerState.text = isReady ? "Ready" : "Waiting";
    }

    public void OnClickReady() {
        if (AccountId == -1) return;
        SetPlayerReady(!_isReady);
    }
}
