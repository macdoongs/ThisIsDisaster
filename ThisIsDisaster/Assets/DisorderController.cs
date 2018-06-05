using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisorderController : MonoBehaviour {

    public GameObject PlayerCharacter;

    private float TimeLeft = 60.0f;
    private float nextTime = 0.0f;

    private float TimeLeftMirage = 30.0f;
    private float nextTimeMirage = 0.0f;
    private bool timerToken = true;

    public static DisorderController Instance
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


    }

    private void Start()
    {

    }

    void Update () {
        
        if (PlayerCharacter == null)
        {
            try
            {
                PlayerCharacter = GameManager.CurrentGameManager.GetLocalPlayer().gameObject;
            }
            catch {

            }
        }
        if (PlayerCharacter == null) return;

        //신기루에 걸린 후 30초 후에 자동으로 신기루 해제
        if (!PlayerCharacter.GetComponent<CharacterModel>().ContainDisorder(Disorder.DisorderType.mirage) )
        {
            if (timerToken)
            {
                nextTimeMirage = Time.time + TimeLeftMirage;
                timerToken = false;
            }
            if (Time.time > nextTimeMirage)
            {
                PlayerCharacter.GetComponent<CharacterModel>().RecoverDisorder(Disorder.DisorderType.mirage);
                timerToken = true;
            }
        }


        //60초마다 상태이상 조건을 체크해 상태이상에 걸림
        if (Time.time > nextTime)
        {
            nextTime = Time.time + TimeLeft;
            MakeDisorder();
        }

    }

    void MakeDisorder()
    {
        //스테이지가 사막이면 신기루를 만듬
        //if(stage == desert)

        MakeDisorderByProbability(Disorder.DisorderType.mirage, 25);


        //이미 갈증이나 굶주림에 걸린 경우
        if (!PlayerCharacter.GetComponent<CharacterModel>().ContainDisorder(Disorder.DisorderType.thirst)
            || !PlayerCharacter.GetComponent<CharacterModel>().ContainDisorder(Disorder.DisorderType.hunger))
        {
        }        
        else
        {
            System.Random random = new System.Random();
            int x = random.Next(1, 100);
            if (x < 50)
            {
                MakeDisorderByProbability(Disorder.DisorderType.thirst, 25);
            }
            else
            {
                MakeDisorderByProbability(Disorder.DisorderType.hunger, 25);
            }
        }
    }


    bool CheckDisorderTriggerCondition(Disorder.DisorderType type)
    {
        return true;
    }
    
    public void MakeDisorderByProbability(Disorder.DisorderType type, int TriggerCondition)
    {
        System.Random random = new System.Random();
        int x = random.Next(1, 100);
        if (x < TriggerCondition)
        {
            if (PlayerCharacter.GetComponent<CharacterModel>().ContainDisorder(type))
            {
                PlayerCharacter.GetComponent<CharacterModel>().GetDisorder(type);
                InGameUIScript.Instance.DisorderNotice(type);
            }
        }
    }
}
