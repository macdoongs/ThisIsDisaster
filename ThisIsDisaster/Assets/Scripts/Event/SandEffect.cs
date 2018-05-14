using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandEffect : MonoBehaviour
{
    public int level = 0;
    public GameObject sandObject;

    public void SetLevel(int level)
    {
        this.level = level;
        //눈의 세기를 바꿔 준다
    }

    public void SetActive(bool state)
    {
        gameObject.SetActive(state);
        sandObject.SetActive(state);
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
