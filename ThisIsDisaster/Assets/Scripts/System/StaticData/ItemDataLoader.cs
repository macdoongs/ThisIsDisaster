using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Xml;

namespace GameStaticData {
    

    public class ItemDataLoader : StaticDataLoader
    {
        public const string _itemXmlFilePath = "xml/ItemData";
        const char _tagDiv = ',';

        static ItemRareness ParseRare(string rare) {
            switch (rare) {
                case "low": return ItemRareness.Low;
                case "mid": return ItemRareness.Middle;
                case "high": return ItemRareness.High;
                default: return ItemRareness.Middle;
            }
        }

        protected override bool Load()
        {
            if (!base.Load()) return false;

            try
            {
                XmlDocument doc = GetDocument();
                XmlNodeList nodeList = doc.SelectNodes("root/item");
                List<ItemTypeInfo> itemInfos = new List<ItemTypeInfo>();
                foreach (XmlNode itemNode in nodeList) {
                    LoadItemData(itemNode, itemInfos);
                }                 
                ItemManager.Manager.InitTypeInfoList(itemInfos);

            }
            catch (Exception e) {
    
                return false;
            }
            return true;
        }

        void LoadItemData(XmlNode itemNode, List<ItemTypeInfo> list) {
            long id = -1;
            string name = "";
            string typeText = "";
            int maxCount = 0;
            string description = "";
            List<string> tags = new List<string>();
            ItemType type = ItemType.Etc;
            Dictionary<string, float> statDic = new Dictionary<string, float>();
            ItemRareness rareness = ItemRareness.Middle;

            XmlNode idNode = itemNode.Attributes.GetNamedItem("id");
            if (idNode!=null) {
                id = long.Parse(idNode.InnerText.Trim());
            }

            XmlNode nameNode = itemNode.Attributes.GetNamedItem("name");
            if (nameNode != null) {
                name = nameNode.InnerText.Trim();
            }

            XmlNode typeNode = itemNode.Attributes.GetNamedItem("type");
            if (typeNode != null) {
                typeText = typeNode.InnerText.Trim();

                type = ItemTypeInfo.ParseType(typeText);
            }

            XmlNode countNode = itemNode.Attributes.GetNamedItem("max");
            if (countNode != null) {
                maxCount = int.Parse(countNode.InnerText.Trim());
            }

            XmlNode rareNode = itemNode.Attributes.GetNamedItem("rareness");
            if (rareNode != null) {
                string rareText = rareNode.InnerText.Trim();
                rareness = ParseRare(rareText);

            }

            ///태그를 안쓸것 같음       
            XmlNode tagNode = itemNode.Attributes.GetNamedItem("tag");
            if (tagNode != null) {
                string tagOriginText = tagNode.InnerText.Trim();

                string[] splitted = tagOriginText.Split(_tagDiv);
                foreach (string tagUnit in splitted) {
                    if (String.IsNullOrEmpty(tagUnit)) continue;
                    tags.Add(tagUnit);
                }
            }

            XmlNode descNode = itemNode.Attributes.GetNamedItem("description");
            if(descNode != null)
            {
                description = descNode.InnerText.Trim();
            }


            foreach (XmlNode statNode in itemNode.ChildNodes) {
                string statName = "";
                float statValue = 0f;

                statName = statNode.Name.Trim();
                statValue = float.Parse(statNode.InnerText.Trim());

                if (string.IsNullOrEmpty(statName)) continue;
                statDic.Add(statName, statValue);
            }

            ItemTypeInfo newInfo = new ItemTypeInfo(id, name, maxCount, type, description , tags.ToArray());
            newInfo.stats = statDic;
            newInfo.rareness = rareness;


            XmlNode spriteNode = itemNode.Attributes.GetNamedItem("sprite");
            if (spriteNode != null)
            {
                string src = spriteNode.InnerText.Trim();
                newInfo.spriteSrc = src;
            }

            list.Add(newInfo);
        }
    }
}