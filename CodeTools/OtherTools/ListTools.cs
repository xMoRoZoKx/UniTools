using System;
using System.Collections.Generic;
using System.Linq;
using UniTools.Reactive;
using UnityEngine;

namespace UniTools
{
    public static class ListTools
    {
        public static ReactiveList<T> ToReactiveList<T>(this IEnumerable<T> source)
        {
            var reactiveList = new ReactiveList<T>();
            foreach (var s in source)
            {
                reactiveList.Add(s);
            }
            return reactiveList;
        }
        public static T GetRandom<T>(this IReadOnlyList<T> list, Predicate<T> match)
        {
            if (list.Count == 0) return default;

            List<T> newList = new();
            foreach (var item in list)
            {
                if (match.Invoke(item)) newList.Add(item);
            }
            var idx = UnityEngine.Random.Range(0, newList.Count());
            return newList.HasIndex(idx) ? newList[idx] : default;
        }
        public static T GetRandom<T>(this IReadOnlyList<T> list)
        {
            if (list.Count == 0) return default;
            return list[UnityEngine.Random.Range(0, list.Count())];
        }
        public static void ForEach<T>(this IReadOnlyList<T> list, Action<T> action)
        {
            foreach (var t in list)
            {
                action?.Invoke(t);
            }
        }
        public static void ForEach<T>(this T[] arr, Action<T> action)
        {
            if (arr == null) return;
            for (int i = 0; i < arr.Length; i++)
                action(arr[i]);
        }
        public static List<T> GetRandoms<T>(this IReadOnlyList<T> list, int count)
        {
            if (list.Count == 0) return default;
            if (count > list.Count) count = list.Count;
            return list.ToList().Shuffle().GetRange(0, count);
        }
        public static T Remove<T>(this IList<T> list, Predicate<T> match)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (match.Invoke(list[i]))
                {
                    var item = list[i];
                    list.Remove(item);
                    return item;
                }
            }
            return default;
        }
        public static T Last<T>(this IReadOnlyList<T> list) => list[list.Count - 1];
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
        public static List<T> Resize<T>(this List<T> list, int size, T defaultValue = default)
        {
            int startCount = list.Count;
            if (startCount > size)
            {
                for (int i = startCount - 1; i > size - 1; i--)
                {
                    list.RemoveAt(i);
                }
            }
            else
            {
                for (int i = 0; i < size - startCount; i++)
                {
                    list.Add(defaultValue);
                }
            }
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
        public static bool HasIndex<T>(this List<T> list, int idx)
        {
            return idx < list.Count && idx >= 0;
        }
        public static Presenter<Data, View> Present<Data, View>(this IReadOnlyList<Data> list, View prefab, RectTransform container, Action<View, Data> onShow) where View : Component
        {
            var presenter = new Presenter<Data, View>();
            if (list is ReactiveList<Data> reactiveList)
            {
                presenter.connections += reactiveList.Subscribe(data => presenter.Present(data, prefab, container, onShow));
            }
            presenter.Present(list, prefab, container, onShow);
            return presenter;
        }
    }
}