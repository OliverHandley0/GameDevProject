using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class SimpleJoystick : MonoBehaviour,
    IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    // background area
    public RectTransform background;  
    // handle element
    public RectTransform handle;

    // current input value
    public Vector2 InputDirection { get; private set; }

    void Start()
    {
        // init
        if (background == null) background = GetComponent<RectTransform>();
        if (handle == null)
            Debug.LogError("Handle missing.");  
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // start drag
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // reset
        InputDirection = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // get local pos
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, eventData.position,
            eventData.pressEventCamera, out localPoint);

        // normalize coords
        localPoint.x /= background.sizeDelta.x;
        localPoint.y /= background.sizeDelta.y;

        Vector2 rawDir = new Vector2(localPoint.x * 2f, localPoint.y * 2f);
        // clamp
        InputDirection = rawDir.magnitude > 1f ? rawDir.normalized : rawDir;

        // move handle
        handle.anchoredPosition = InputDirection * (background.sizeDelta.x / 3f);
    }
}
