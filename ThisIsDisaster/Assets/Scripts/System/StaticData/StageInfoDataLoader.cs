using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GameStaticData
{
    public class StageInfoDataLoader : StaticDataLoader {
        const string _xmlPath = "xml/StageInfo";

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
                //init
            }
            catch { return false; }
            return true;
        }

        void LoadClimateInfo(XmlNode node, List<StageGenerator.ClimateInfo> list) {

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

        
    }
}
