using UniTools.Reactive;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TouchBar : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public UnityEvent<float> OnMoveX;
    public UnityEvent<float> OnMoveY;
    public UnityEvent<bool> OnPress;

    private Reactive<Vector2> lastPosition = new();
    private Reactive<bool> isPressed = new();

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed.value = true;
        lastPosition.value = eventData.position;
        OnPress?.Invoke(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed.value = false;
        OnPress?.Invoke(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.position - lastPosition.value;
        lastPosition.value = eventData.position;

        OnMoveX?.Invoke(delta.x);
        OnMoveY?.Invoke(delta.y);
    }
}
