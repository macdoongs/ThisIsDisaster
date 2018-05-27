using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEffect : MonoBehaviour {
 
    public GameObject fireObject;
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
        fireObject.SetActive(state);
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

