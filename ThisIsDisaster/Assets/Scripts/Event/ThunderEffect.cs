using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* 
    이벤트 발생 시 비오게
    시간 약간 지나면 화면 어두워지게
    20초 지나면 낙뢰 멈추고
    이벤트 끝나면 비 점점 안오게
*/
public class ThunderEffect : MonoBehaviour {

    public const int MaxLevel = 8;
    [Range(0, MaxLevel)]
    public int level = 0;

    public void SetLevel(int level)
    {
        this.level = level;
    }

    public void SetActive(bool state)
    {
        gameObject.SetActive(state);
    }


    public delegate void EndEvent();

    EndEvent _endEvent = null;


    public void SetEndEvent(EndEvent e)
    {
        this._endEvent = e;
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

