using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniTools
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
        private List<View> _views = new List<View>();
        private List<(View, Data)> _data = new List<(View, Data)>();
        public IReadOnlyList<View> views => _views;
        public IReadOnlyList<(View, Data)> data => _data;

        public Connections connections = new Connections();

        public Presenter<Data, View> Present(IEnumerable<Data> list, View prefab, RectTransform container, Action<View, Data, int> onShow, bool useIgnoreElements = true)
        {
            _views = container.GetComponentsInChildren<View>(true).ToList();

            if (!useIgnoreElements) _views.RemoveAll(v => v.GetComponent<PresenterIgnore>());

            _views.ForEach(v => v.SetActive(false));

            _data.Clear();

            for (int i = 0; i < list.Count(); i++)
            {
                if (_views.Count <= i)
                    _views.Add(UnityEngine.Object.Instantiate(prefab, container));

                _views[i].SetActive(true);
                onShow?.Invoke(_views[i], list.ElementAt(i), i);
                _data.Add((_views[i], list.ElementAt(i)));
            }
            return this;
        }


        public void Dispose()
        {
            connections.DisconnectAll();

            _views.ForEach(view => view.SetActive(false));
            _views.Clear();
        }
    }
}