using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class MultiButtonAction
{
    public string name;
    public UnityEvent onClick = new UnityEvent();
}
public class MultiButton : Selectable, IPointerClickHandler, IEventSystemHandler
{
    [SerializeField] private List<MultiButtonAction> actions;
    [SerializeField] private TextMeshProUGUI buttonName;
    private int currentActionIdx = 0;
    public void AddAction(string name, Action onClick)
    {
        var action = new MultiButtonAction() { name = name };
        action.onClick.AddListener(() => onClick?.Invoke());
        
        actions.Add(action);
    }

    public void Set(int idx)
    {
        currentActionIdx = 0;
        OnClick();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }
    private void OnClick()
    {
        var currentAction = actions[currentActionIdx];

        buttonName.text = currentAction.name;

        currentAction.onClick?.Invoke();

        currentActionIdx++;
        if (currentActionIdx >= actions.Count) currentActionIdx = 0;
    }
}
