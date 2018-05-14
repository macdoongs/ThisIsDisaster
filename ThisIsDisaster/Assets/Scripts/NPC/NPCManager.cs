using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using NPC;

public class NPCManager : MonoBehaviour, IObserver
{
    public NPCManager Manager {
        get; private set;
    }

    private void Awake()
    {
        Manager = this;
    }

    private void OnDestroy()
    {
        
    }

    public void OnNotice(string notice, params object[] param)
    {
        //currently empty
    }

    public void ObserveNotices()
    {
    }

    public void RemoveNotices()
    {
    }
}