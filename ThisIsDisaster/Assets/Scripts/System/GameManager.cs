using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager CurrentGameManager {
        private set;
        get;
    }

    public Dictionary<int, UnitControllerBase> RemotePlayer
    {
        get
        {
            return _remotePlayer;
        }
    }

    private UnitControllerBase _localPlayer;
    private Dictionary<int, UnitControllerBase> _remotePlayer = null;

    public UnitControllerBase CommonPlayerObject;

    private void Awake()
    {
        CurrentGameManager = this;
        _remotePlayer = new Dictionary<int, UnitControllerBase>();
    }

    // Use this for initialization
    void Start () {
        var localPlayer = MakePlayerCharacter(GlobalParameters.Param.accountName, 
            GlobalParameters.Param.accountId, true);

        NetworkComponents.NetworkModule.Instance.RegisterReceiveNotification(
            NetworkComponents.PacketId.Coordinates, OnReceiveCharacterCoordinate);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public UnitControllerBase GetLocalPlayer() {
        return _localPlayer;
    }

    public static UnitControllerBase MakePlayerCharacter(string name, int id, bool isLocal) {
        UnitControllerBase output = null;
        if (!isLocal)
        {
            if (CurrentGameManager.RemotePlayer.TryGetValue(id, out output))
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
        output.SetUnitName(name);

        PlayerMoveController moveScript = copy.GetComponent<PlayerMoveController>();
        if (moveScript) {
            if (!isLocal) {
                moveScript.enabled = false;
            }
        }

        if (isLocal)
        {
            CurrentGameManager._localPlayer = output;
        }
        else {
            CurrentGameManager.RemotePlayer.Add(id, output);
        }

        return output;
    }

    public void OnReceiveCharacterCoordinate(NetworkComponents.PacketId packetId, int packetSender, byte[] data) {
        NetworkComponents.CharacterMovingPacket packet = new NetworkComponents.CharacterMovingPacket(data);
        NetworkComponents.CharacterData charData = packet.GetPacket();

        UnitControllerBase controller = null;
        if (RemotePlayer.TryGetValue(packetSender, out controller)) {
            Debug.LogError("Position Info " + packetSender);
            controller.OnReceiveCharacterCoordinate(charData);
            foreach (var c in charData.coordinates) {
                Debug.Log(c.ToString());
            }
        }
    }
}
