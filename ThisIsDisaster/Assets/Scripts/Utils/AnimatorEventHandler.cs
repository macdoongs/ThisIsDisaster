using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEventHandler : MonoBehaviour {

    public delegate void OnEventCalled();

    private OnEventCalled onEventCalled;

    public void SetEventCalled(OnEventCalled called) {
        this.onEventCalled = called;
    }

    public void OnEventCall() {
        if (onEventCalled != null) {
            onEventCalled();
        }
    }
}
