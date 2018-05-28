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
        MatchingRequest,
        MatchingResponse,
        SessionInfoSync,
        SessionInfoSyncReflection,
        StartSessionNotify,

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
}
