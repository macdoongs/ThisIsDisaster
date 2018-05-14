using UnityEngine;
using NetworkComponents;
using System.Collections;
using System.Collections.Generic;

public class UnitControllerBase : MonoBehaviour
{
    protected string _unitName = "";
    public UnitBehaviourBase behaviour = null;
    private ItemManager _itemManager = null;

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
        Debug.Log(string.Format("{0} set position {1}", _unitName, position));
    }

    public float GetDirection() {
        return transform.localScale.x;
    }

    public void OnReceiveMovingPacket(PacketId id, byte[] data) {

    }

    public void SendCharacterCoordinate(int index, List<CharacterCoordinates> coords) {
        if (Network) {
            if (!Network.IsConnected()) return;
            CharacterData data = new CharacterData()
            {
                index = index,
                dataNum = coords.Count,
                coordinates = new CharacterCoordinates[coords.Count]
            };

            for (int i = 0; i < coords.Count; i++) {
                data.coordinates[i] = coords[i];
            }

            CharacterMovingPacket packet = new CharacterMovingPacket(data);
            int sendSize = Network.SendUnreliable<CharacterData>(packet);

            if (sendSize > 0) {
                //success
            }

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
