using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Xml;
using Environment;

namespace GameStaticData
{
    public class EnvironmentDataLoader : StaticDataLoader
    {
        public const string _envXmlFilePath = "xml/EnvironmentData";

        protected override bool Load()
        {
            if (!base.Load()) return false;

            try
            {
                XmlDocument doc = GameStaticDataLoader.Loader.GetXmlDocuments(_envXmlFilePath);
                XmlNodeList nodeList = doc.SelectNodes("root/environment");
                List<EnvironmentTypeInfo> envInfos = new List<EnvironmentTypeInfo>();
                foreach (XmlNode envNode in nodeList) {
                    LoadEnvInfo(envNode, envInfos);
                }
                EnvironmentManager.Manager.InitInfoList(envInfos);
            }
            catch {
                return false;
            }
            return true;
        }

        void LoadEnvInfo(XmlNode node, List<EnvironmentTypeInfo> list) {
            int id = 0;
            string name = "";
            string prefab = "";
            string script = "";
            EnvironmentTypeInfo newInfo = new EnvironmentTypeInfo();

            try
            {

                XmlNode idNode = node.Attributes.GetNamedItem("id");
                XmlNode nameNode = node.Attributes.GetNamedItem("name");
                XmlNode prefabNode = node.Attributes.GetNamedItem("prefab");
                XmlNode scriptNode = node.Attributes.GetNamedItem("script");

                if (idNode != null)
                {
                    id = (int)float.Parse(idNode.InnerText.Trim());
                }
                if (nameNode != null) {
                    name = nameNode.InnerText.Trim();
                }
                if (prefabNode != null) {
                    prefab = prefabNode.InnerText.Trim();
                }
                if (scriptNode != null) {
                    script = scriptNode.InnerText.Trim();
                }

                newInfo.Id = id;
                newInfo.Name = name;
                newInfo.Prefab = prefab;
                newInfo.Script = script;
            }
            catch {
                return;
            }
            list.Add(newInfo);
        }
    }
}
