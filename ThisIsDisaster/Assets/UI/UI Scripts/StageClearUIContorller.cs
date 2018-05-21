using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageClearUIContorller : MonoBehaviour {

    public GameObject StageClearPanel;

    public bool isStageCleared;
    public int deadCount;
    public int disasterCount;
    public float EXP;

    public int Rank = 0;

    public Image[] CheckBoxes;
    public Image StageClearCheck;
    public Image DeadCountCheck;
    public Image DisasterCountCheck;

    public Text DeadCountText;
    public Text DisasterCountText;
    public Image EXPImage;

    public Sprite CheckTrue;
    public Sprite CheckFalse;

    public Sprite Star_Y;
    public Sprite Star_G;

    public Image[] Stars = new Image[3];

    public void Start()
    {
        CheckBoxes = new Image[] { StageClearCheck, DeadCountCheck, DisasterCountCheck };

        SetStageClearUI();
    }

    public void Update()
    {
        SetStageClearUI();
    }

    public void SetStageClearUI()
    {
        Rank = 0;

        SetCheckBox(isStageCleared, StageClearCheck);
        DeadCountText.text = deadCount.ToString();
        SetCheckBox(deadCount < 2, DeadCountCheck);
        DisasterCountText.text = disasterCount.ToString();
        SetCheckBox(disasterCount > 5, DisasterCountCheck);

        EXPImage.fillAmount = EXP;

        for(int i = 0; i < Rank; i++)
        {
            Stars[i].sprite = Star_Y;
        }

        for(int j = Rank; j < 3; j++)
        {
            Stars[j].sprite = Star_G;
        }
    }

    public void SetCheckBox(bool isCheck, Image CheckBox)
    {
        if (isCheck)
        {
            CheckBox.sprite = CheckTrue;
            CheckBox.color = Color.green;
            Rank++;
        }
        else
        {
            CheckBox.sprite = CheckFalse;
            CheckBox.color = Color.red;
        }
    }


}
