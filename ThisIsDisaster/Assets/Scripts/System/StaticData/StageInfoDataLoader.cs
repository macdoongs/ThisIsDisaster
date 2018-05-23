using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GameStaticData
{
    public class StageInfoDataLoader : StaticDataLoader {
        public const string _xmlPath = "xml/StageInfo";

        protected override bool Load()
        {
            if (!base.Load()) return false;
            try {
                XmlDocument doc = GameStaticDataLoader.Loader.GetXmlDocuments(_xmlPath);
                XmlNode commonNode = doc.SelectSingleNode("common");
                XmlNodeList nodeList = doc.SelectNodes("root/climate");

                List<StageGenerator.ClimateInfo> climateInfos = new List<StageGenerator.ClimateInfo>();
                foreach (XmlNode climateNode in nodeList) {
                    LoadClimateInfo(climateNode, climateInfos);
                }
                StageGenerator.Instance.InitClimateType(climateInfos);
                //init
            }
            catch { return false; }
            return true;
        }

        void LoadClimateInfo(XmlNode node, List<StageGenerator.ClimateInfo> list) {
            StageGenerator.ClimateInfo newClimate = new StageGenerator.ClimateInfo();

            try
            {
                string type = node.Attributes.GetNamedItem("type").InnerText.Trim();
                ClimateType climateType = ClimateType.Island;
                if (!ParseClimate(type, out climateType)) {
                    return;
                }
                newClimate.climateType = climateType;

                XmlNode eventsNode = node.SelectSingleNode("events");
                if (eventsNode != null)
                {
                    XmlNodeList eventList = eventsNode.SelectNodes("event");
                    foreach (XmlNode e in eventList)
                    {
                        string weatherType = e.InnerText.Trim();
                        WeatherType weather = ParseEventType(weatherType);
                        newClimate.weatherList.Add(weather);
                    }
                }

                XmlNode mapNode = node.SelectSingleNode("map");
                if (mapNode != null)
                {
                    int height = (int)float.Parse(
                        mapNode.Attributes.GetNamedItem("height").InnerText.Trim());

                    XmlNode zeroTypeNode = mapNode.SelectSingleNode("zero");
                    XmlNodeList tileNode = mapNode.SelectNodes("tile");

                    StageGenerator.ZeroTileType zeroTileType = ParseZeroTileType(zeroTypeNode.InnerText.Trim());
                    newClimate.zeroTileType = zeroTileType;

                    foreach (XmlNode tile in tileNode)
                    {
                        int level = (int)float.Parse(tile.Attributes.GetNamedItem("level").InnerText.Trim());
                        string src = tile.InnerText.Trim();

                        newClimate.tileSpriteSrc.Add(level, src);
                    }
                }

                XmlNodeList itemNodeList = node.SelectNodes("items/item");
                if (itemNodeList != null)
                {
                    foreach (XmlNode itemNode in itemNodeList)
                    {
                        int id = (int)float.Parse(itemNode.InnerText.Trim());
                        newClimate.uniqueGenItemList.Add(id);
                    }
                }

                XmlNodeList envNodeList = node.SelectNodes("environments/environment");
                if (envNodeList != null)
                {
                    foreach (XmlNode envNode in envNodeList)
                    {
                        int height = (int)float.Parse(envNode.Attributes.GetNamedItem("height").InnerText);
                        int id = (int)float.Parse(envNode.InnerText.Trim());
                        int min = (int)float.Parse(envNode.Attributes.GetNamedItem("min").InnerText);
                        int max = (int)float.Parse(envNode.Attributes.GetNamedItem("max").InnerText);
                        var newInfo = new StageGenerator.ClimateInfo.EnvInfo(height, min, max, id);
                        newClimate.envInfoList.Add(newInfo);
                    }
                }

                XmlNodeList npcNodeList = node.SelectNodes("npcs/npc");
                if (npcNodeList != null)
                {
                    foreach (XmlNode npcNode in npcNodeList)
                    {
                        int id = (int)float.Parse(npcNode.InnerText.Trim());
                        int max = (int)float.Parse(npcNode.Attributes.GetNamedItem("max").InnerText);
                        var newInfo = new StageGenerator.ClimateInfo.NpcInfo(max, id);
                        newClimate.npcInfoList.Add(newInfo);
                    }
                }
            }
            catch (Exception e){
#if UNITY_EDITOR
                UnityEngine.Debug.LogError(e);
#endif
                return;
            }

            list.Add(newClimate);
        }

        bool ParseClimate(string parsed, out ClimateType type)
        {
            switch (parsed.ToLower().Trim())
            {
                case "island": type = ClimateType.Island; return true;
                case "forest": type = ClimateType.Forest; return true;
                case "desert": type = ClimateType.Desert; return true;
                case "polar": type = ClimateType.Polar; return true;
                default: type = ClimateType.Island; return false;
            }
        }

        StageGenerator.ZeroTileType ParseZeroTileType(string parsed) {
            switch (parsed.ToLower().Trim())
            {
                case "slow": return StageGenerator.ZeroTileType.Slow;
                case "dead": return StageGenerator.ZeroTileType.Dead;
                default: return StageGenerator.ZeroTileType.None; 

            }
        }
        
        public static WeatherType ParseEventType(string eventString)
        {
            WeatherType output = WeatherType.None;
            switch (eventString.ToLower())
            {
                case "cyclone": output = WeatherType.Cyclone; break;
                case "flood": output = WeatherType.Flood; break;
                case "yellowdust": output = WeatherType.Yellowdust; break;
                case "drought": output = WeatherType.Drought; break;
                case "fire": output = WeatherType.Fire; break;
                case "earthquake": output = WeatherType.Earthquake; break;
                case "lightning": output = WeatherType.Lightning; break;
                case "landsliding": output = WeatherType.Landsliding; break;
                case "heavysnow": output = WeatherType.Heavysnow; break;
            }
            return output;
        }
    }
}
