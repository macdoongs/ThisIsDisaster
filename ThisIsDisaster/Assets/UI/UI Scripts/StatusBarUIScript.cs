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

    public GameObject DisorderDescriptionTotalPanel;
    public Image DisorderDescImage;
    public GameObject DisorderDescPanelTitle;

    public Image DisorderDescPanel;
    public Text DisorderDescName;
    public Text DisorderDesc;
    public Text DisorderRecoveryCondition;

    public static StatusBarUIScript Instance
    {
        private set;
        get;
    }

    public void Awake()
    {
        if (Instance != null && Instance.gameObject != null)
        {
            GameObject.Destroy(gameObject);
            return;
        }
        Instance = this;

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

        bool TitleToken = false;

        int index = 0;
        foreach (var disorder in disorders)
        {


            if (disorder != null)
            {
                TitleToken = true;
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

        DisorderDescPanelTitle.SetActive(TitleToken);
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
        HPText.text = ((int)HP).ToString();
        StaminaBar.fillAmount = Stamina / MaxStamina;
        StaminaText.text = Stamina.ToString();
    }

    public void SetPlayerInfo(GameObject PlayerCharacter)
    {
        PlayerName.text = PlayerCharacter.GetComponent<CharacterModel>().PlayerName;

        PlayerLevel.text = PlayerCharacter.GetComponent<CharacterModel>().PlayerLevel.ToString();

    }


    public void DisorderDescription(int index)
    {
        DefaultDisorderDescription();

        Disorder disorder = disorders[index];
        if(disorder == null)
        {
            return;
        }

        Disorder.DisorderType type = disorder.disorderType;

        if (type.Equals(Disorder.DisorderType.mirage))
        {
            DisorderDescImage.sprite = Mirage;
            DisorderDescImage.color = Color.white;
            DisorderDescName.text = "신기루";
            DisorderDesc.text = "사막에서 발생하는 상태이상.\n신기루로 인해 바뀐 지형을 지나가면 체력이 감소하고 몬스터를 사냥하면 스테미너가 감소합니다.";
            DisorderRecoveryCondition.text = "신기루 발생 후 30초 후에 자동 종료";
        }
        else if (type.Equals(Disorder.DisorderType.injury))
        {
            DisorderDescImage.sprite = Injury;
            DisorderDescImage.color = Color.white;
            DisorderDescName.text = "부상";
            DisorderDesc.text = "부상을 당한 캐릭터는 이동속도가 감소하고 최대체력이 감소합니다.\n또한 부상을 당한 동안에는 공격력과 방어력이 낮아집니다.";
            DisorderRecoveryCondition.text = "아이템 '구급상자' 로 부상 회복";
        }
        else if (type.Equals(Disorder.DisorderType.poisoning))
        {
            DisorderDescImage.sprite = Poisoning;
            DisorderDescImage.color = Color.white;
            DisorderDescName.text = "식중독";
            DisorderDesc.text = "식중독에 걸리면 체력과 스태미나가 감소되며 스태미나 회복량이 감소합니다. ";
            DisorderRecoveryCondition.text = "아이템 '약' 으로 부상 회복";
        }
        else if (type.Equals(Disorder.DisorderType.thirst))
        {
            DisorderDescImage.sprite = Thirst;
            DisorderDescImage.color = Color.white;
            DisorderDescName.text = "갈증";
            DisorderDesc.text = "갈증 상태에 빠지면 스태미나가 감소합니다.";
            DisorderRecoveryCondition.text = "아이템 '물'을 소모해 갈증 회복";
        }
        else
        {
            DisorderDescImage.sprite = Hunger;
            DisorderDescImage.color = Color.white;
            DisorderDescName.text = "굶주림";
            DisorderDesc.text = "굶주림을 느끼면 이동속도가 크게 저하되고 스테미너와 기본 공격력이 감소합니다.";
            DisorderRecoveryCondition.text = "아이템 '생고기','음식'을 소모해 굶주림 회복";
        }

        float y = 0f;

        foreach(var text in DisorderDescPanel.GetComponentsInChildren<Text>())
        {
            y += text.GetComponent<RectTransform>().rect.height;
        }
                
   //     Debug.Log("DisorderDesc Height : " + y.ToString());
   //     DisorderDescPanel.rectTransform.sizeDelta = new Vector2(DisorderDescPanel.rectTransform.sizeDelta.x, y + 30); 
     //   DisorderDescPanel.GetComponent<BoxCollider2D>().size = new Vector2(DisorderDescPanel.rectTransform.sizeDelta.x, y + 30);

        DisorderDescriptionTotalPanel.SetActive(true);
    }

    public void DefaultDisorderDescription()
    {
        DisorderDescImage.sprite = null;
        DisorderDescName.text = "";
        DisorderDesc.text = "";
    }
}
