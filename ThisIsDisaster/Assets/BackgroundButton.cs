using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundButton : MonoBehaviour {

    public GameObject Joystick;
    
    public void JoystickOn()
    {
        if (Joystick.GetComponent<Joystick>().floatingJoystick)
        {
            Camera[] cameras = Camera.allCameras;

            Joystick.transform.position = cameras[0].ScreenToWorldPoint(Input.mousePosition);
            Joystick.GetComponentInChildren<GameObject>().transform.position = Joystick.transform.position;

            Joystick.SetActive(true);
        }
        else
        {
            Joystick.transform.position = new Vector2(142, 262);
            Joystick.SetActive(true);
        }
    }

    public void JoystickOff()
    {
        Joystick.SetActive(false);
    }
}
