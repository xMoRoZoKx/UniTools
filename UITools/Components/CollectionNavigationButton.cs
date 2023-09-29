using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionNavigationButton : ConnectableMonoBehaviour
{
    public TextMeshProUGUI massage;
    public Button button;
    public RectTransform defaultBG, selectedBG;
    public void Show(string massage, Action onClick = null)
    {
        this.massage.text = massage;
        
        connections.Dispose();

        connections += button.SubscribeWithSound(() => onClick?.Invoke());
    }
    public void Highlight(bool value)
    {
        defaultBG.SetActive(!value);
        selectedBG.SetActive(value);
    }
}
