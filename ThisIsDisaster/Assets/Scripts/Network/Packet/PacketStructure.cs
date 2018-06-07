using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using UnityEngine;

namespace NetworkComponents
{
    public enum PacketId {
        GameSyncInfo,
        CharacterData,
        ItemData,
        Coordinates,

        //Maching Relate
        MatchingData,
        MatchingRequest,
        MatchingResponse,
        MatchingReady,
        SessionInfo,
        SessionInfoSync,
        SessionInfoSyncReflection,
        StartSessionNotify,
        StageStartTime,

        //InGmae
        PlayerStateInfo,
        PlayerItemInfo,
        PlayerAnimTrigger,

        GameServerRequest,
        Max//dummy End
    }

    public struct PacketHeader {
        public PacketId packetId;
        public int packetSender;
    }

    public struct GameSyncData {
        public int serverVersion;
        public string accountName;
        public int accountId;
        public int stageGenSeed;

        public const int MAX_ACCOUNT_LENGTH = 128;
    }

    public struct CharacterCoordinates {
        public float x;
        public float y;
        public float z;

        public CharacterCoordinates(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3 GetVector() {
            return new Vector3(x, y, z);
        }

        public static CharacterCoordinates SetFromVector(Vector3 v) {
            return new CharacterCoordinates(v.x, v.y, v.z);
        }

        public static CharacterCoordinates Lerp(CharacterCoordinates a, CharacterCoordinates b, float rate) {
            CharacterCoordinates c;
            c.x = Mathf.Lerp(a.x, b.x, rate);
            c.y = Mathf.Lerp(a.y, b.y, rate);
            c.z = Mathf.Lerp(a.z, b.z, rate);
            return c;
        }

        public override string ToString()
        {
            return "Coordinate [" + x + "," + y + "," + z + "]";
        }
    }

    public struct CharacterData {
        //public string characterId;
        public int index;
        public int dataNum;
        public CharacterCoordinates[] coordinates;
        
        public const int MAX_CHAR_ID = 64;

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Index : {0}\tDataNum : {1}", index, dataNum);
            builder.AppendLine();
            int count = 0;
            foreach (var coord in coordinates) {
                builder.AppendFormat("\tind:{0} ({1},{2},{3})", count, coord.x, coord.y, coord.z);
            }
            return builder.ToString();
        }
    }

    public struct SessionSyncInfo {
        public int accountId;
        public int serverPort;
        public int ipLength;
        public string ip;
        
    }

    public struct SessionSyncInfoReflection {
        public int nodeIndex;
        public bool isConnection;
        public int nodeAccountId;
        public int nodeServerPort;
        public int ipLength;
        public string nodeIp;
    }

    public struct StartSessionNotice {
        public int sessionId;
        public int stageRandomSeed;
    }

    public struct MatchingRequest {
        public int accountId;
        public int accountLength;
        public string accountName;
    }

    public struct MatchingResponse {
        public bool connectionState;
        public int nodeIndex;
        public int sessionIndex;
    }

    public struct GameServerRequest {
        public GameServerRequestType request;
    }

    public struct StageStartTime {
        public DateTime startTime;
    }

    public struct PlayerStateInfo {
        public int accountId;
        public int playerHp;
        public bool isPlayerDead;
    }

    public struct PlayerItemInfo {
        public int accountId;
        public int itemId;
        public bool isAcquire;
    }

    public struct PlayerAnimTrigger {
        public int playerId;
        public int animKeyLength;
        public string animKey;
        
    }
}
