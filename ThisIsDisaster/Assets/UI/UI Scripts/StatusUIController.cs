using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUIController : MonoBehaviour {

    public GameObject StatusUI;

    public Text PlayerLevel;
    public Text PlayerName;

    //상태창 텍스트
    public Text[] StatusPane;
    public Text Health;
    public Text Stamina;
    public Text Defense;
    public Text Damage;
    public Image[] Disorders;

    public Sprite Mirage;
    public Sprite Injury;
    public Sprite Poisoning;
    public Sprite Thirst;
    public Sprite Hunger;


    public Disorder[] disorders;

    public static StatusUIController Instance
    {
        private set;
        get;
    }

    private void Awake()
    {
        if (Instance != null && Instance.gameObject != null)
        {
            GameObject.Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {

    }

    public StatusUIController()
    {
        StatusPane = new Text[] { Health, Stamina, Defense, Damage };
    }

    //캐릭터 모델로부터 스텟을 불러와 상태창에 반영
    public void GetStatus(GameObject PlayerCharacter)
    {
        string _maxHealth = PlayerCharacter.GetComponent<CharacterModel>().CurrentStats.MaxHealth.ToString();
        string _health = ((int)PlayerCharacter.GetComponent<CharacterModel>().CurrentStats.Health).ToString("F1");
        string _healthInUI = _health + " / " + _maxHealth;

        string _maxStamina = PlayerCharacter.GetComponent<CharacterModel>().CurrentStats.MaxStamina.ToString();
        string _stamina = ((int)PlayerCharacter.GetComponent<CharacterModel>().CurrentStats.Stamina).ToString("F1");
        string _staminaInUI = _stamina + " / " + _maxStamina;


        Health.GetComponent<Text>().text = _healthInUI;
        Stamina.GetComponent<Text>().text = _staminaInUI;
        Damage.GetComponent<Text>().text =
                PlayerCharacter.GetComponent<CharacterModel>().CurrentStats.Damage.ToString();
        Defense.GetComponent<Text>().text =
            PlayerCharacter.GetComponent<CharacterModel>().CurrentStats.Defense.ToString();

        GetDisorders(PlayerCharacter);
    }

    public void SetPlayerInfo(GameObject PlayerCharacter)
    {
        PlayerName.text = PlayerCharacter.GetComponent<CharacterModel>().PlayerName;

        PlayerLevel.text = PlayerCharacter.GetComponent<CharacterModel>().PlayerLevel.ToString();

    }

    public void GetDisorders(GameObject PlayerCharacter)
    {
        disorders = PlayerCharacter.GetComponent<CharacterModel>().disorders;

        int i = 0;

        foreach(var disorder in disorders)
        {
            if(disorder != null)
            {
                SetDisorderImage(disorder, Disorders[i]);
            }
            else
            {
                Disorders[i].sprite = null;
                Disorders[i].color = Color.clear;
            }
            i++;
        }
    }

    public void SetDisorderImage(Disorder disorder, Image image)
    {
        Disorder.DisorderType type = disorder.disorderType;

        if (type.Equals(Disorder.DisorderType.mirage))
        {
            image.sprite = Mirage;
            image.color = Color.white;
        }
        else if (type.Equals(Disorder.DisorderType.injury))
        {
            image.sprite = Injury;
            image.color = Color.white;
        }
        else if (type.Equals(Disorder.DisorderType.poisoning))
        {
            image.sprite = Poisoning;
            image.color = Color.white;
        }
        else if (type.Equals(Disorder.DisorderType.thirst))
        {
            image.sprite = Thirst;
            image.color = Color.white;
        }
        else
        {
            image.sprite = Hunger;
            image.color = Color.white;
        }
    }


    public void Close()
    {
        StatusUI.SetActive(false);
    }
}
