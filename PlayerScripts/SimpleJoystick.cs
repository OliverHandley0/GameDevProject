using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class SimpleJoystick : MonoBehaviour,
    IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public RectTransform background;
    public RectTransform handle;
    public Vector2 InputDirection { get; private set; }

    void Start()
    {
        if (background == null) background = GetComponent<RectTransform>();
        if (handle == null)
            Debug.LogError("SimpleJoystick: assign the Handle RectTransform.");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        InputDirection = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, eventData.position, eventData.pressEventCamera, out pos);

        pos.x /= background.sizeDelta.x;
        pos.y /= background.sizeDelta.y;

        Vector2 dir = new Vector2(pos.x * 2f, pos.y * 2f);
        InputDirection = dir.magnitude > 1f ? dir.normalized : dir;

        handle.anchoredPosition = new Vector2(
            InputDirection.x * (background.sizeDelta.x / 3f),
            InputDirection.y * (background.sizeDelta.y / 3f)
        );
    }
}
