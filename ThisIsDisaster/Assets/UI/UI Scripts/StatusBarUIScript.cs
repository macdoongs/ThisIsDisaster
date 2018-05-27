using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBarUIScript : MonoBehaviour {

    public Text PlayerLevel;
    public Text PlayerName;

    public Image HPBar;
    public Text HPText;
    public Image StaminaBar;
    public Text StaminaText;

    public float MaxHP;
    public float HP;

    public float MaxStamina;
    public float Stamina;


    public GameObject DisorderBar;
    public Image[] DisorderBackground = new Image[5];
    public Image[] DisorderImages = new Image[5];

    public Color BackgroundColor;

    public Sprite Mirage;
    public Sprite Injury;
    public Sprite Poisoning;
    public Sprite Thirst;
    public Sprite Hunger;

    public Disorder[] disorders;

    public GameObject DisorderDescPanel;
    public Image DisorderDescImage;
    public Text DisorderDescName;
    public Text DisorderDesc;

    public void Awake()
    {
        Image[] temp = DisorderBar.transform.GetComponentsInChildren<Image>();

        int index1 = 0;
        int index2 = 0;
        for (int i = 0; i < temp.Length; i++)
        {
            if (i == 0)
                continue;

            if (i % 3 == 1)
            {
                DisorderBackground[index1] = temp[i];
                index1++;
            }
            else if(i % 3 == 2)
            {
                DisorderImages[index2] = temp[i];
                index2++;
            }
        }
    }


    public void UpdateDisorder(GameObject PlayerCharacter)
    {
        disorders = PlayerCharacter.GetComponent<CharacterModel>().disorders;
        DefaultDisorder();

        int index = 0;
        foreach (var disorder in disorders)
        {
            if (disorder != null)
            {
                Disorder.DisorderType type = disorder.disorderType;

                if (type.Equals(Disorder.DisorderType.mirage))
                {
                    DisorderImages[index].sprite = Mirage;
                    DisorderImages[index].color = Color.white;
                }
                else if (type.Equals(Disorder.DisorderType.injury))
                {
                    DisorderImages[index].sprite = Injury;
                    DisorderImages[index].color = Color.white;
                }
                else if (type.Equals(Disorder.DisorderType.poisoning))
                {
                    DisorderImages[index].sprite = Poisoning;
                    DisorderImages[index].color = Color.white;
                }
                else if (type.Equals(Disorder.DisorderType.thirst))
                {
                    DisorderImages[index].sprite = Thirst;
                    DisorderImages[index].color = Color.white;
                }
                else
                {
                    DisorderImages[index].sprite = Hunger;
                    DisorderImages[index].color = Color.white;
                }
                DisorderBackground[index].color = BackgroundColor;
            }
            else
            {
                DisorderImages[index].sprite = null;
                DisorderImages[index].color = Color.clear;
                DisorderBackground[index].color = Color.clear;
            }

            index++;

            if (index == 5)
                break;
        }
    }

    public void DefaultDisorder()
    {
        for(int i = 0; i < 5; i++)
        {
            DisorderImages[i].sprite = null;
            DisorderImages[i].color = Color.clear;
            DisorderBackground[i].color = Color.clear;
        }
    }

    public void UpdateStatusBar(GameObject PlayerCharacter)
    {
        UpdateDisorder(PlayerCharacter);
        MaxHP = PlayerCharacter.GetComponent<CharacterModel>().CurrentStats.MaxHealth;
        HP = PlayerCharacter.GetComponent<CharacterModel>().CurrentStats.Health;

        MaxStamina = PlayerCharacter.GetComponent<CharacterModel>().CurrentStats.MaxStamina;
        Stamina = PlayerCharacter.GetComponent<CharacterModel>().CurrentStats.Stamina;

        HPBar.fillAmount = HP / MaxHP;
        HPText.text = HP.ToString();
        StaminaBar.fillAmount = Stamina / MaxStamina;
        StaminaText.text = Stamina.ToString();
    }

    public void SetPlayerInfo(GameObject PlayerCharacter)
    {
        PlayerName.text = PlayerCharacter.GetComponent<CharacterModel>().PlayerName;

        PlayerLevel.text = PlayerCharacter.GetComponent<CharacterModel>().PlayerLevel;

    }


    public void DisorderDescription(int index)
    {
        DefaultDisorderDescription();

        Disorder disorder = disorders[index];
        
        Disorder.DisorderType type = disorder.disorderType;

        if (type.Equals(Disorder.DisorderType.mirage))
        {
            DisorderDescImage.sprite = Mirage;
            DisorderDescImage.color = Color.white;
            DisorderDescName.text = "신기루";
            DisorderDesc.text = "사막에서 발생하는 상태이상." +
                "신기루가 발생하게 되면 스태미너가 더 빨리 소모됩니다." +
                "일정 스태미너 이상이 되면 신기루 현상이 사라지게 되므로 스테미너를 회복하도록 하자.";

        }
        else if (type.Equals(Disorder.DisorderType.injury))
        {
            DisorderDescImage.sprite = Injury;
            DisorderDescImage.color = Color.white;
            DisorderDescName.text = "부상";
            DisorderDesc.text = "부상을 당한 캐릭터는 이동속도가 감소하고 최대체력이 감소합니다." +
                "부상을 당한 동안에는 공격력과 방어력이 낮아집니다.";
        }
        else if (type.Equals(Disorder.DisorderType.poisoning))
        {
            DisorderDescImage.sprite = Poisoning;
            DisorderDescImage.color = Color.white;
            DisorderDescName.text = "식중독";
            DisorderDesc.text = "생고기를 먹을 경우 낮은 확률로 식중독에 걸립니다." +
                "식중독에 걸리면 체력과 스테미너가 감소되며 스테미너회복량이 감소됩니다. ";
        }
        else if (type.Equals(Disorder.DisorderType.thirst))
        {
            DisorderDescImage.sprite = Thirst;
            DisorderDescImage.color = Color.white;
            DisorderDescName.text = "갈증";
            DisorderDesc.text = "갈증상태에 빠지면 스태미너가 감소합니다. ";
        }
        else
        {
            DisorderDescImage.sprite = Hunger;
            DisorderDescImage.color = Color.white;
            DisorderDescName.text = "굶주림";
            DisorderDesc.text = "굶주림을 느끼면 이동속도가 크게 저하되고 스테미너가 감소합니다.";
        }

        DisorderDescPanel.SetActive(true);
    }

    public void DefaultDisorderDescription()
    {
        DisorderDescImage.sprite = null;
        DisorderDescName.text = "";
        DisorderDesc.text = "";
    }
}
