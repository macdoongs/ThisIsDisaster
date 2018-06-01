using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Xml;

namespace GameStaticData
{
    public class RecipeDataLoader : StaticDataLoader
    {

        public const string _recipeXmlFilePath = "xml/MixtureRecipe";
        const char _tagDiv = ',';

        protected override bool Load()
        {
            if (!base.Load()) return false;

            try
            {
                XmlDocument doc = GetDocument();
                XmlNodeList nodeList = doc.SelectNodes("root/recipe");
                List<MixtureRecipe> recipeInfos = new List<MixtureRecipe>();
                foreach (XmlNode recipeNode in nodeList)
                {
                    LoadRecipeData(recipeNode, recipeInfos);
                }

               ItemManager.Manager.InitRecipeList(recipeInfos);

            }
            catch (Exception e)
            {

                return false;
            }
            return true;
        }

        void LoadRecipeData(XmlNode RecipeNode, List<MixtureRecipe> list)
        {
            long Resultid = -1;


            List<long> materialID = new List<long>();
            List<int> mateiralNum = new List<int>();

            Dictionary<long, int> materialDic = new Dictionary<long, int>();

            XmlNode RecipeidNode = RecipeNode.Attributes.GetNamedItem("resultID");
            if (RecipeidNode != null)
            {
                Resultid = long.Parse(RecipeidNode.InnerText.Trim());
            }

            XmlNodeList matNode = RecipeNode.SelectNodes("material");


            foreach (XmlNode materialNode in matNode)
            {
                long matID = 0;
                int matNum = 0;

                XmlNode matIDNode = materialNode.Attributes.GetNamedItem("id");
                if(matIDNode != null)
                {
                    matID = long.Parse(matIDNode.InnerText.Trim());
                }

                XmlNode matNumNode = materialNode.Attributes.GetNamedItem("num");
                if (matNumNode != null)
                {
                    matNum = int.Parse(matNumNode.InnerText.Trim());
                }

                if (matID == 0) continue;
                materialDic.Add(matID, matNum);

            }

            MixtureRecipe newRecipe = new MixtureRecipe(Resultid, materialDic);

            list.Add(newRecipe);
        }
    }
}