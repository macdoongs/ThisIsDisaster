using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudyEffect : MonoBehaviour
{
    public int level = 0;
    public GameObject cloudyObject;

    public void SetLevel(int level)
    {
        this.level = level;
        //이게 필요한가..
    }

    public void SetActive(bool state)
    {
        gameObject.SetActive(state);
        cloudyObject.SetActive(state);
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
