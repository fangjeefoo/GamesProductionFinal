using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class LongPressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool pointerDown;
    private bool longClick;
    private bool clickable;
    private float pointerDownTime = 0f;

    [Tooltip("Need to hold how long on button")]
    public float holdTime;
    public UnityEvent onLongClick;

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDown = true;
        if (eventData.selectedObject)
            clickable = eventData.selectedObject.GetComponent<Button>().interactable;
        else
            clickable = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {        
        pointerDownTime = 0f;
        pointerDown = false;
        if (longClick && clickable)
            GameManager.gm.DisableLongClick();
        else if(clickable)
            GameManager.gm.OnClick();
    }

    // Update is called once per frame
    void Update()
    {
        if (pointerDown)
        {
            pointerDownTime += Time.deltaTime;
            if (pointerDownTime >= holdTime)
            {
                longClick = true;
                if(clickable)
                    onLongClick.Invoke();
            }
            else
                longClick = false;
        }            
    }
}
