using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniTools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropView : ConnectableMonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform itemView;
    public EventStream onBeginDrag = new EventStream();
    public EventStream<IEnumerable<GameObject>> onDrop = new EventStream<IEnumerable<GameObject>>();
    public bool dragable = false;
    public Canvas canvas { get; protected set; }
    public ScrollRect scrollRect { get; protected set; }
    Vector2 startPos;
    Transform originParent;
    public bool canDrag => dragable && itemView != null && canvas != null;
    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;

        itemView.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag)
        {
            if (scrollRect)
            {
                eventData.pointerDrag = scrollRect.gameObject;
                EventSystem.current.SetSelectedGameObject(scrollRect.gameObject);

                scrollRect.OnInitializePotentialDrag(eventData);
                scrollRect.OnBeginDrag(eventData);
            }
            return;
        }
        if (!canvas)
        {
            Debug.LogError("Canvas not seted");
        }

        originParent ??= itemView.parent;

        startPos = itemView.anchoredPosition;
        itemView.SetParent(canvas.transform);

        SetMaskable(false);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag) return;

        List<RaycastResult> raycastResults = new();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        onDrop.Invoke(raycastResults.Select(result => result.gameObject));

        itemView.SetParent(originParent);
        itemView.anchoredPosition = startPos;

        SetMaskable(true);
    }

    public void SetMaskable(bool value)
    {
        GetComponentsInChildren<Image>().ForEach(img => img.maskable = value);
        GetComponentsInChildren<TMP_Text>().ForEach(txt => txt.maskable = value);
    }
}
