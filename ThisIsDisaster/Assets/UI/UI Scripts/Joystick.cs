using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler,IPointerDownHandler{

    public Image bgImg;
    public Image joystickImg;
    private Vector3 inputVector;

    public static Joystick Instance
    {
        private set;
        get;
    }

    private void Awake()
    {

        if (Instance != null && Instance.gameObject != null)
        {
            GameObject.Destroy(gameObject);
            return;
        }
        Instance = this;
    }
	
    public virtual void OnDrag(PointerEventData pad)
    {
        Vector2 pos;
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImg.rectTransform, pad.position , pad.pressEventCamera, out pos))
        {
            pos.x = (pos.x / bgImg.rectTransform.sizeDelta.x);
            pos.y = (pos.y / bgImg.rectTransform.sizeDelta.y);

            inputVector = new Vector3(pos.x * 2 + 1, pos.y * 2 - 1, 0);

            joystickImg.rectTransform.anchoredPosition = new Vector3(inputVector.x * (bgImg.rectTransform.sizeDelta.x / 3),
                inputVector.y * (bgImg.rectTransform.sizeDelta.y / 3));
        }
    }

    public virtual void OnPointerDown(PointerEventData ped)
    {
        OnDrag(ped);
    }

    public virtual void OnPointerUp(PointerEventData ped)
    {
        inputVector = Vector3.zero;
        joystickImg.rectTransform.anchoredPosition = Vector3.zero;
    }

    public float GetHorizontalValue()
    {
        return inputVector.x;
    }

    public float GetVerticalValue()
    {
        return inputVector.y;
    }




}
