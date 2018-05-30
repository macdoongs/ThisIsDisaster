using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour {
    public Timer _lifeClock = new Timer();

    public float LifeTime = 1f;

    private void OnEnable()
    {
        if (LifeTime > 0f) {
            SetClock(LifeTime);
        }
    }

    public void SetClock(float time) {
        LifeTime = time;
        _lifeClock.StartTimer(LifeTime);
    }

    private void Update()
    {
        if (_lifeClock.RunTimer()) {
            Destroy(gameObject);
        }
    }
}
