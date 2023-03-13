using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.CodeTools;
using UITools;
using UnityEngine;

namespace Tools
{
    public static class ListTools
    {
        public static T GetRandom<T>(this List<T> list)
        {
            if (list.Count == 0) return default;
            return list[UnityEngine.Random.Range(0, list.Count())];
        }
        public static List<T> GetRandoms<T>(this List<T> list, int count)
        {
            if (list.Count == 0) return default;
            if (count > list.Count) count = list.Count;
            return list.ToList().Shuffle().GetRange(0, count);
        }
        public static List<T> Shuffle<T>(this List<T> list)
        {
            if (list.Count == 0) return default;
            for (int i = 0; i < list.Count; i++)
            {
                int j = UnityEngine.Random.Range(i, list.Count);
                if (j != i)
                {
                    var temp = list[j];
                    list[j] = list[i];
                    list[i] = temp;
                }
            }
            return list;
        }
        public static List<T> Resize<T>(this List<T> list, int size)
        {
            for (int i = 0; i < list.Count + size; i++)
            {
                list.Add(default);
            }
            if (list.Count > size) list = list.GetRange(0, size);
            return list;
        }
        public static bool AddIfNotContains<T>(this List<T> list, T element)
        {
            if (!list.Contains(element))
            {
                list.Add(element);
                return true;
            }
            return false;
        }
        public static Presenter<Data, View> Present<Data, View>(this List<Data> list, View prefab, RectTransform container, Action<View, Data> onShow) where View : MonoBehaviour
        {
            var presenter = new Presenter<Data, View>();
            if (list is ReactiveList<Data> reactiveList)
            {
                reactiveList.Subscribe(data => presenter.Present(data, prefab, container, onShow));
            }
            presenter.Present(list, prefab, container, onShow);
            return presenter;
        }
    }
}