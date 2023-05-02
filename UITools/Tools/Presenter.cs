using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tools
{
    public class SimplePresenter<View> : IDisposable where View : Component
    {
        public List<View> views = new List<View>();

        public SimplePresenter<View> Present(int count, View prefab, RectTransform container, Action<View> onShow = null)
        {
            views = container.GetComponentsInChildren<View>().ToList();
            views.RemoveAll(v => v.GetComponent<PresenterIgnore>());
            views.ForEach(v => v.SetActive(false));
            for (int i = 0; i <= count; i++)
            {
                if (views.Count <= i)
                    views.Add(UnityEngine.Object.Instantiate(prefab, container));
                views[i].SetActive(true);
                onShow?.Invoke(views[i]);
            }
            return this;
        }


        public void Dispose()
        {
            views.ForEach(view => view.SetActive(false));
            views.Clear();
        }
    }
    public class Presenter<Data, View> : IDisposable where View : Component
    {
        public List<View> views { private set; get; } = new List<View>();
        public Connections connections = new Connections();
        public Presenter<Data, View> Present(IReadOnlyList<Data> list, View prefab, RectTransform container, Action<View, Data> onShow)
        {
            views = container.GetComponentsInChildren<View>().ToList();
            views.RemoveAll(v => v.GetComponent<PresenterIgnore>());
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
            connections.DisconnectAll();
            views.ForEach(view => view.SetActive(false));
            views.Clear();
        }
    }
}