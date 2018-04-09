using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Xml;

namespace GameStaticData {
    public class ItemDataLoader : StaticDataLoader
    {
        const string _itemXmlFilePath = "xml/ItemData";
        const char _tagDiv = ',';

        protected override bool Load()
        {
            if (!base.Load()) return false;

            try
            {
                XmlDocument doc = GameStaticDataLoader.Loader.GetXmlDocuments(_itemXmlFilePath);
                XmlNodeList nodeList = doc.SelectNodes("root/item");
                List<ItemTypeInfo> itemInfos = new List<ItemTypeInfo>();
                foreach (XmlNode itemNode in nodeList) {
                    LoadItemData(itemNode, itemInfos);
                }
                ItemManager.Manager.InitTypeInfoList(itemInfos);

            }
            catch (Exception e) {
                Debug.LogError(e);
                return false;
            }
            return true;
        }

        void LoadItemData(XmlNode itemNode, List<ItemTypeInfo> list) {
            long id = -1;
            string name = "";
            string typeText = "";
            int maxCount = 0;
            List<string> tags = new List<string>();
            ItemType type = ItemType.EXPENDABLES;

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

            XmlNode tagNode = itemNode.Attributes.GetNamedItem("tag");
            if (tagNode != null) {
                string tagOriginText = tagNode.InnerText.Trim();

                string[] splitted = tagOriginText.Split(_tagDiv);
                foreach (string tagUnit in splitted) {
                    if (String.IsNullOrEmpty(tagUnit)) continue;
                    tags.Add(tagUnit);
                }
            }

            ItemTypeInfo newInfo = new ItemTypeInfo(id, name, maxCount, type, tags.ToArray());
            list.Add(newInfo);
        }
    }
}