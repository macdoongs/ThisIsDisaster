using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShelterIndicator : MonoBehaviour, IObserver {
    private Shelter.ShelterModel _shelter;
    public Image indicator;
    bool state = false;
    public Camera cam;
    public Canvas canvas;
    public CharacterModel player;
    RectTransform CanvasRect {
        get { return canvas.GetComponent<RectTransform>(); }
    }

    void SetActive(bool state) {
        indicator.enabled = state;
        this.state = state;
    }

    // Use this for initialization
    void Start () {
        SetActive(false);
	}

    public void OnEnable()
    {
        ObserveNotices();
    }

    private void OnDisable()
    {
        RemoveNotices();
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.F1)) {
            SetActive(!state);
        }

        if (_shelter != null && state && player != null) {
            UpdateIndicator();
        }
    }

    void UpdateIndicator() {
        var tilePos = _shelter.AccessTile.transform.position;
        var playerPos = player.transform.position;

        Vector3 dist = tilePos - playerPos;
        float distValue = dist.sqrMagnitude;
        float factor = 500f;
        if (distValue <= Camera.main.orthographicSize) {
            factor = distValue / Camera.main.orthographicSize * factor;
        }
        dist.z = 0f;

        var norm = dist.normalized;

        indicator.rectTransform.anchoredPosition = norm * factor;
        //indicator.rectTransform.anchoredPosition = pos;
    }

    void SetShelter(Shelter.ShelterModel shelter) {
        Debug.Log("setshelter");
        _shelter = shelter;
    }

    public void ObserveNotices()
    {
        Notice.Instance.Observe(NoticeName.AddShelter, this);
        Notice.Instance.Observe(NoticeName.LocalPlayerGenerated, this);
    }

    public void OnNotice(string notice, params object[] param)
    {
        if (notice == NoticeName.AddShelter)
        {
            if (param[0] is Shelter.ShelterModel) {
                SetShelter((Shelter.ShelterModel)param[0]);
                //SetActive(true);
            }
        }

        if (notice == NoticeName.LocalPlayerGenerated) {
            player = GameManager.CurrentGameManager.GetLocalPlayer().GetComponent<CharacterModel>();
        }
    }

    public void RemoveNotices()
    {
        Notice.Instance.Remove(NoticeName.AddShelter, this);
        Notice.Instance.Remove(NoticeName.LocalPlayerGenerated, this);
    }

}
