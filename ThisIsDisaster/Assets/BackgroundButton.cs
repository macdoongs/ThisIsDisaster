using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundButton : MonoBehaviour {

    public GameObject Joystick;

    public void JoystickOn()
    {

        Camera[] cameras = Camera.allCameras;

        Joystick.transform.position = cameras[0].ScreenToWorldPoint(Input.mousePosition);
        Joystick.GetComponentInChildren<GameObject>().transform.position = Joystick.transform.position;

        Joystick.SetActive(true);
        Debug.Log("Background Clicked");
    }

    public void JoystickOff()
    {
        Joystick.SetActive(false);
    }
}
