using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NetworkComponents;

public class MatchingPanel : MonoBehaviour {
    public Animator PivotCTRL;

    public MatchingSlot[] matchingSlots;

    private Dictionary<int, int> _slotAccount = new Dictionary<int, int>();

    private void Start()
    {
        //zero is account
        _slotAccount.Add(0, GlobalParameters.Param.accountId);
    }

    public void OnOpenPanel() {
        Show();

        //zero is localplayer
        matchingSlots[0].SetPlayer(GlobalParameters.Param.accountId, 1, GlobalParameters.Param.accountName, true);

    }

    public void OnClosePanel() {
        //stop
        Hide();
        LobbyUIScript.Instance.DefaultMenu();
    }

    void Show() {
        AnimatorUtil.SetTrigger(PivotCTRL, "Show");
    }

    void Hide() {
        AnimatorUtil.SetTrigger(PivotCTRL, "Hide");
    }
}
