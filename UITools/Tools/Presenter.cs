using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Presenter<Data, View> : IDisposable where View : MonoBehaviour
{
    private List<View> views = new List<View>();

    public List<View> Present(List<Data> list, View prefab, RectTransform container, Action<View, Data> onShow)
    {
        views.ForEach(v => v.SetActive(false));
        for (int i = 0; i < list.Count; i++)
        {
            if (views.Count <= i)
                views.Add(UnityEngine.Object.Instantiate(prefab, container));
            views[i].SetActive(true);
            onShow?.Invoke(views[i], list[i]);
        }
        return views.GetRange(0, list.Count);
    }


    public void Dispose()
    {
        views.ForEach(view => UnityEngine.Object.Destroy(view.gameObject));
        views.Clear();
    }
}
