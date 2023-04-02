using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tools
{
    public class Presenter<Data, View> : IDisposable where View : MonoBehaviour
    {
        public List<View> views = new List<View>();

        public Presenter<Data, View> Present(List<Data> list, View prefab, RectTransform container, Action<View, Data> onShow)
        {
            views = container.GetComponentsInChildren<View>().ToList();
            views.ForEach(v => v.SetActive(false));
            for (int i = 0; i < list.Count; i++)
            {
                if (views.Count <= i)
                    views.Add(UnityEngine.Object.Instantiate(prefab, container));
                views[i].SetActive(true);
                onShow?.Invoke(views[i], list[i]);
            }
            return this;
        }


        public void Dispose()
        {
            views.ForEach(view => view.SetActive(false));
            views.Clear();
        }
    }
}