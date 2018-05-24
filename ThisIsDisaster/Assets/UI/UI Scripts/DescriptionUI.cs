using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionUI : MonoBehaviour {

    public GameObject DescriptionPanel;

    public Image DescriptionItemImage;
    public Text DescriptionItemName;
    public Text DescriptionItemDescription;
    public Text[] DescriptionItemStats = new Text[3];
    public Text[] DescriptionItemStatAmount = new Text[3];
    public Text DescriptionItemRightButton;
    public Text DescriptionItemLeftButton;
    public GameObject DescriptionItemRegisterButton;

    // 아이템 슬롯 설명
    public DescriptionUI(GameObject Panel, Image Image, Text Name, Text Desc, Text[] Stats,
        Text[] StatAmount, Text LeftButton, Text RightButton, GameObject RegisterButton)
    {
        DescriptionPanel = Panel;
        DescriptionItemImage = Image;
        DescriptionItemName = Name;
        DescriptionItemDescription = Desc;
        DescriptionItemStats = Stats;
        DescriptionItemStatAmount = StatAmount;
        DescriptionItemRightButton = RightButton;
        DescriptionItemLeftButton = LeftButton;
        DescriptionItemRegisterButton = RegisterButton;
    }

    // 프리뷰 아이템 설명
    public DescriptionUI(GameObject Panel, Image Image, Text Name, Text Desc, Text[] Stats, Text[] StatAmount, Text LeftButton, Text RightButton)
    {
        DescriptionPanel = Panel;
        DescriptionItemImage = Image;
        DescriptionItemName = Name;
        DescriptionItemDescription = Desc;
        DescriptionItemStats = Stats;
        DescriptionItemStatAmount = StatAmount;
        DescriptionItemRightButton = RightButton;
        DescriptionItemLeftButton = LeftButton;
    }

    //아이템 교체 설명
    public DescriptionUI(Image Image, Text Name, Text[] Stats, Text[] StatAmount)
    {
        DescriptionItemImage = Image;
        DescriptionItemName = Name;
        DescriptionItemDescription = null;
        DescriptionItemStats = Stats;
        DescriptionItemStatAmount = StatAmount;
    }


    public void SetDescription(ItemModel item)
    {
        if(item == null)
        {
            return ;
        }

        ItemType type = item.metaInfo.itemType;

        if(DescriptionItemRegisterButton != null)
        {
            if (type.Equals(ItemType.Etc) || type.Equals(ItemType.Tool_Use))
            {
                DescriptionItemRegisterButton.SetActive(true);
                DescriptionItemRightButton.text = "사용";
            }
            else if(type.Equals(ItemType.Normal))
            {
                if (DescriptionItemRegisterButton != null)
                    DescriptionItemRegisterButton.SetActive(false);
                DescriptionItemRightButton.text = "조합";
            }
            else
            {
                if (DescriptionItemRegisterButton != null)
                    DescriptionItemRegisterButton.SetActive(false);
                DescriptionItemRightButton.text = "착용";
            }
        }


        string src = item.metaInfo.spriteSrc;
        Sprite s = Resources.Load<Sprite>(src);
        DescriptionItemImage.sprite = s;


        DescriptionItemImage.color = Color.white;
        DescriptionItemImage.preserveAspect = true;

        DescriptionItemName.text = item.metaInfo.Name;

        if(item.metaInfo.description != null && DescriptionItemDescription !=null)
        {
            DescriptionItemDescription.text = item.metaInfo.description;
        }
        

        Dictionary<string, float> stats = GetItemStats(item);

        int i = 0;
        foreach (var key in stats)
        {
            DescriptionItemStats[i].text = key.Key;
            DescriptionItemStatAmount[i].text = key.Value.ToString();
            i++;
        }
    }

    public void ClearDescription()
    {
        if(DescriptionPanel != null)
        {
            DescriptionPanel.SetActive(false);
        }

        DescriptionItemImage.sprite = null;
        DescriptionItemImage.color = Color.clear;

        DescriptionItemName.text = "";

        for (int i = 0; i < 3; i++)
        {
            DescriptionItemStats[i].text = "";
            DescriptionItemStatAmount[i].text = "";
        }
    }

    public Dictionary<string, float> GetItemStats(ItemModel item)
    {
        // 순서 : HP , Stamina , Damage , Defense 

        Dictionary<string, float> result = new Dictionary<string, float>();

        if (item.GetHealth() != 0f)
        {
            result.Add("HP", item.GetHealth());
        }

        if (item.GetStamina() != 0f)
        {
            result.Add("Stamina", item.GetStamina());
        }
        if (item.GetDamage() != 0f)
        {
            result.Add("Damage", item.GetDamage());
        }
        if (item.GetDefense() != 0f)
        {
            result.Add("Defense", item.GetDefense());
        }
        if (item.GetSize() != 0)
        {
            if (item.metaInfo.itemType.Equals(ItemType.Backpack))
                result.Add("Bag Size", item.GetSize());
            else
                result.Add("Bottle Size", item.GetSize());
        }
        if(item.GetStaminaRegen() != 0)
        {
            result.Add("Stamina Regen", item.GetStaminaRegen());
        }
        if(item.GetHealthRegen() != 0)
        {
            result.Add("Health Regen", item.GetHealthRegen());
        }
        

        return result;
    }
}
