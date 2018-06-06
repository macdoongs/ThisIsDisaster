using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour{

    public GameObject JoystickGameObject;
    public Color JoystickColor;
    public bool floatingJoystick = true;

    RectTransform joysticRect {
        get { return JoystickGameObject.GetComponent<RectTransform>(); }
    }
    public Transform Stick;

    private Vector3 StickFirstPos;  // 조이스틱의 처음 위치.
    public Vector3 JoyVec;         // 조이스틱의 벡터(방향)
    private float Radius;           // 조이스틱 배경의 반 지름.
    public CanvasScaler canvasScaler;           // 조이스틱 렌더링 캔버스

    public static Joystick Instance
    {
        private set;
        get;
    }


    void Awake()
    {

        if (Instance != null && Instance.gameObject != null)
        {
            GameObject.Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    void Start()
    {
        Radius = GetComponent<RectTransform>().sizeDelta.y * 0.5f;
        StickFirstPos = Stick.transform.position;

        // 캔버스 크기에대한 반지름 조절.
        float Can = transform.parent.GetComponent<RectTransform>().localScale.x;
        Radius *= Can;
    }

    // 드래그
    public void Drag(BaseEventData _Data)
    {
        PointerEventData Data = _Data as PointerEventData;
        Vector3 Pos = Data.position;

        // 조이스틱을 이동시킬 방향을 구함.(오른쪽,왼쪽,위,아래)
        JoyVec = (Pos - StickFirstPos).normalized;
        JoyVec.x *= 2f;
        JoyVec.y *= 2f;


        // 조이스틱의 처음 위치와 현재 내가 터치하고있는 위치의 거리를 구한다.
        float Dis = Vector3.Distance(Pos, StickFirstPos);

        // 거리가 반지름보다 작으면 조이스틱을 현재 터치하고 있는곳으로 이동. 
        if (Dis < Radius)
            Stick.position = StickFirstPos + JoyVec * Dis;
        // 거리가 반지름보다 커지면 조이스틱을 반지름의 크기만큼만 이동.
        else
            Stick.position = StickFirstPos + JoyVec * Radius;
    }


    public void DragEnd()
    {
        Stick.position = StickFirstPos; // 스틱을 원래의 위치로.
        JoyVec = Vector3.zero;          // 방향을 0으로.
    }



    public void JoystickOn()
    {
        if (floatingJoystick)
        {
            Camera[] cameras = Camera.allCameras;
            float input_x_rate = Input.mousePosition.x / Screen.width;
            float input_y_rate = Input.mousePosition.y / Screen.height;

            float x = canvasScaler.referenceResolution.x * input_x_rate;
            float y = canvasScaler.referenceResolution.y * input_y_rate;

            joysticRect.anchoredPosition = new Vector2(x, y);

            Stick.position = JoystickGameObject.transform.position;
            StickFirstPos = Stick.position;
            JoystickGameObject.GetComponentInChildren<Image>().color = JoystickColor;
            Stick.GetComponentInChildren<Image>().color = JoystickColor;
        }
        else
        {
            joysticRect.anchoredPosition = new Vector2(145, 307);
            StickFirstPos = Stick.position;
            JoystickGameObject.GetComponentInChildren<Image>().color = JoystickColor;
            Stick.GetComponentInChildren<Image>().color = JoystickColor;
        }
    }

    public void JoystickOff()
    {
        if (floatingJoystick)
        {
            JoystickGameObject.GetComponentInChildren<Image>().color = Color.clear;
            Stick.GetComponentInChildren<Image>().color = Color.clear;
        }
        else
        {
            joysticRect.anchoredPosition = new Vector2(145, 307);
            StickFirstPos = Stick.position;
            JoystickGameObject.GetComponentInChildren<Image>().color = JoystickColor;
            Stick.GetComponentInChildren<Image>().color = JoystickColor;
        }
    }

    public void FloatingJoystickSetting(bool isFloat)
    {
        floatingJoystick = isFloat;
    }
}
