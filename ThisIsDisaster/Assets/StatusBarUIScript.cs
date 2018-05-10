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


    public GameObject CCBar;

    public Sprite[] CCSprites;

    public float MaxHP;
    public float HP;

    public float MaxStamina;
    public float Stamina;


    public void UpdateStatusBar(GameObject PlayerCharacter)
    {
        MaxHP = PlayerCharacter.GetComponent<CharacterModel>().maxHealth;
        HP = PlayerCharacter.GetComponent<CharacterModel>().health;

        MaxStamina = PlayerCharacter.GetComponent<CharacterModel>().maxStamina;
        Stamina = PlayerCharacter.GetComponent<CharacterModel>().stamina;

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
}
