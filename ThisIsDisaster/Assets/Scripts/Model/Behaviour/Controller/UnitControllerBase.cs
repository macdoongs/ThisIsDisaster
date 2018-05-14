using UnityEngine;
using NetworkComponents;
using System.Collections;
using System.Collections.Generic;

public class UnitControllerBase : MonoBehaviour
{
    public UnitBehaviourBase behaviour = null;
    private ItemManager _itemManager = null;

    public NetworkComponents.NetworkModule Network {
        get {
            if (_network == null) {
                var go = GameObject.Find("NetworkModule");
                if (go != null) {
                    _network = go.GetComponent<NetworkComponents.NetworkModule>();
                }

            }
            return _network;
        }
    }

    private void Awake()
    {
        _itemManager = ItemManager.Manager;
    }

    // Use this for initialization
    void Start()
    {
        Network.RegisterReceiveNotification(PacketId.Coordinates, OnReceiveCharacterCoordinate);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public float GetDirection() {
        return transform.localScale.x;
    }

    public void OnReceiveMovingPacket(PacketId id, byte[] data) {

    }

    public void SendCharacterCoordinate(string charId, int index, List<CharacterCoordinates> coords) {
        if (Network) {
            if (!Network.IsConnected()) return;
            CharacterData data = new CharacterData()
            {
                characterId = charId,
                index = index,
                dataNum = coords.Count,
                coordinates = new CharacterCoordinates[coords.Count]
            };

            for (int i = 0; i < coords.Count; ++i) {
                data.coordinates[i] = coords[i];
            }

            CharacterMovingPacket packet = new CharacterMovingPacket(data);
            int sendSize = Network.SendUnreliable<CharacterData>(packet);

            if (sendSize > 0) {
                //success
            }

        }   
    }

    public void OnReceiveCharacterCoordinate(PacketId id, byte[] data) {
        CharacterMovingPacket packet = new CharacterMovingPacket(data);
        CharacterData charData = packet.GetPacket();

        //check movable state
        if (true) ;

        //update position
    }

    public void onEventHandling(NetEventState state) {
        switch (state.type) {
            case NetEventType.Connect:

                break;
            case NetEventType.Disconnect:

                break;
        }
    }
}
