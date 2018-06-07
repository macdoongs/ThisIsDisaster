using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SettingScript : MonoBehaviour {

    public Toggle JoystickSettingToggle;

    public void JoystickSetting()
    {
        Joystick.Instance.floatingJoystick = !JoystickSettingToggle.isOn;
        Joystick.Instance.JoystickOff();
    }
}
