using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudEffect : MonoBehaviour
{
    public int level = 0;
    public GameObject cloudObject;

    public void SetLevel(int level)
    {
        this.level = level;
    }

    public void SetActive(bool state)
    {
        gameObject.SetActive(state);
        cloudObject.SetActive(state);
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
