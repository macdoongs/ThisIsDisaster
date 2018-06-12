using UnityEngine;
using NetworkComponents;
using System.Collections;
using System.Collections.Generic;

public class UnitControllerBase : MonoBehaviour
{
    protected string _unitName = "";
    public UnitBehaviourBase behaviour = null;
    private ItemManager _itemManager = null;
    public int AccountId = 0;
    private Transform _flipPivot = null;

    public NetworkComponents.NetworkModule Network {
        get {
            return NetworkModule.Instance;
        }
    }

    public UnityEngine.UI.Text NameText;

    private void Awake()
    {
        _itemManager = ItemManager.Manager;
    }

    // Use this for initialization
    void Start()
    {
        //Network.RegisterReceiveNotification(PacketId.Coordinates, OnReceiveCharacterCoordinate);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetFlipPivot(Transform pivot) {
        this._flipPivot = pivot;
    }

    public void SetUnitName(string name) {
        if (NameText) {
            NameText.text = name;
        }
        _unitName = name;
    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetDirection(float xDiff) {
        if (_flipPivot == null) return;
        if (xDiff == 0f) return;
        Vector3 current = _flipPivot.localScale;
        if (xDiff < 0f)
        {
            if (current.x > 0f)
            {
                current.x = -1f;
            }
        }
        else {
            if (current.x < 0f) {
                current.x = 1f;
            }
        }
        _flipPivot.localScale = current;
    }

    public float GetDirection() {
        return _flipPivot.localScale.x;
    }

    public void OnReceiveMovingPacket(PacketId id, byte[] data) {

    }

    public void SendCharacterCoordinate(int index, List<CharacterCoordinates> coords) {
        if (Network && GlobalGameManager.Instance.GameNetworkType == GameNetworkType.Multi)
        {
            //check connection
            CharacterData data = new CharacterData()
            {
                index = index,
                dataNum = coords.Count,
                coordinates = new CharacterCoordinates[coords.Count]
            };

            for (int i = 0; i < coords.Count; i++)
            {
                data.coordinates[i] = coords[i];
            }

            CharacterMovingPacket packet = new CharacterMovingPacket(data);
            //int sendSize = Network.SendUnreliable<CharacterData>(packet);
            Network.SendUnreliableToAll(packet);

        }
    }

    public void OnReceiveCharacterCoordinate(CharacterData data) {
        behaviour.CalcCoordinates(data.index, data.coordinates);
        //check movable state

        //behaviour.CalcRemotePosition();
        //update position
    }

    public void OnEventHandling(NetEventState state) {
        switch (state.type) {
            case NetEventType.Connect:
                 
                break;
            case NetEventType.Disconnect:

                break;
        }
    }
}
