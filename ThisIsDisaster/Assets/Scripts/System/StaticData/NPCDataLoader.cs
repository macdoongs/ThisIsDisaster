using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using NPC;

namespace GameStaticData
{
    public class NPCDataLoader : StaticDataLoader
    {
        public const string _npcXmlFilePath = "xml/NPCData";

        protected override bool Load()
        {
            if (!base.Load()) return false;

            try
            {
                XmlDocument doc = GetDocument();
                XmlNodeList nodeList = doc.SelectNodes("root/npc");
                List<NPCTypeInfo> npcInfos = new List<NPCTypeInfo>();
                foreach (XmlNode npcNode in nodeList) {
                    LoadNPCInfo(npcNode, npcInfos);
                }

                NPCManager.Manager.SetNpcInfos(npcInfos);

            }
            catch {
                return false;
            }

            return true;
        }

        void LoadNPCInfo(XmlNode node, List<NPCTypeInfo> list) {
            int id = 0;
            string name = "";
            string script = "";
            string prefab = "";
            NPCTypeInfo newInfo = new NPCTypeInfo();

            try {
                XmlNode idNode = node.Attributes.GetNamedItem("id");
                XmlNode nameNode = node.Attributes.GetNamedItem("name");
                XmlNode maxHpNode = node.Attributes.GetNamedItem("maxHp");
                XmlNode speedNode = node.Attributes.GetNamedItem("speed");
                XmlNode defenseNode = node.Attributes.GetNamedItem("defense");
                XmlNode damageNode = node.Attributes.GetNamedItem("damage");
                XmlNode scriptNode = node.Attributes.GetNamedItem("script");
                XmlNode prefabNode = node.Attributes.GetNamedItem("prefab");

                if (idNode != null) {
                    id = (int)float.Parse(idNode.InnerText.Trim());
                }

                if (nameNode != null) {
                    name = nameNode.InnerText.Trim();
                }

                if (maxHpNode != null) {
                    newInfo.stats.Add("maxHp", float.Parse(maxHpNode.InnerText.Trim()));
                }
                if (speedNode != null)
                {
                    newInfo.stats.Add("speed", float.Parse(maxHpNode.InnerText.Trim()));
                }
                if (defenseNode != null)
                {
                    newInfo.stats.Add("defense", float.Parse(maxHpNode.InnerText.Trim()));
                }
                if (damageNode != null)
                {
                    newInfo.stats.Add("damage", float.Parse(maxHpNode.InnerText.Trim()));
                }
                if (scriptNode != null) {
                    script = scriptNode.InnerText.Trim();
                }
                if (prefabNode != null) {
                    prefab = prefabNode.InnerText.Trim();
                }
                newInfo.Id = id;
                newInfo.Name = name;
                newInfo.Script = script;
                newInfo.Prefab = prefab;
            }
            catch {
                return;
            }
            list.Add(newInfo);
        }

    }
}