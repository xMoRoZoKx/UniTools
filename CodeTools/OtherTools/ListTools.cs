using System;
using System.Collections.Generic;
using System.Linq;
using UniTools.Reactive;
using UnityEngine;
using static Unity.VisualScripting.Member;

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
        public static T GetRandom<T>(this IEnumerable<T> list, Predicate<T> match)
        {
            if (list.Count() == 0) return default;

            List<T> newList = new();
            foreach (var item in list)
            {
                if (match.Invoke(item)) newList.Add(item);
            }
            var idx = UnityEngine.Random.Range(0, newList.Count());
            return newList.HasIndex(idx) ? newList[idx] : default;
        }
        public static T GetRandom<T>(this IEnumerable<T> list)
        {
            if (list.Count() == 0) return default;
            return list.ElementAt(UnityEngine.Random.Range(0, list.Count()));
        }
        public static T GetRandom<T>(this IEnumerable<T> list, Func<T, float> weight)
        {
            var items = list.ToList();
            if (items.Count == 0)
                return default;

            float minWeight = items.Min(weight);
            float offset = minWeight < 0 ? -minWeight + 1 : 0;

            float totalWeight = items.Sum(item => weight(item) + offset);
            if (totalWeight <= 0)
                throw new InvalidOperationException("Weight exception");

            float randomValue = (float)(new System.Random().NextDouble() * totalWeight);

            foreach (var item in items)
            {
                randomValue -= weight(item) + offset;
                if (randomValue <= 0)
                    return item;
            }

            return default;
        }

        public static List<T> SortWith<T, TKey>(this List<T> list, Func<T, TKey> keySelector) => list.OrderByDescending(keySelector)
                  .ThenBy(item => list.IndexOf(item))
                  .ToList();
        public static void ForEach<T>(this IReadOnlyList<T> list, Action<T> action)
        {
            foreach (var t in list)
            {
                action?.Invoke(t);
            }
        }
        public static void ForEachWithIndexes<T>(this IReadOnlyList<T> list, Action<T, int> action)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (action == null) throw new ArgumentNullException(nameof(action));

            for (int i = 0; i < list.Count; i++)
            {
                action.Invoke(list[i], i);
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
            return CreateResizedList(list.ToList(), size, i => defaultValue);
        }
        public static List<T> CreateResizedList<T>(this IEnumerable<T> source, int size, Func<int, T> getValue)
        {
            var list = source.ToList();
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
                    list.Add(getValue.Invoke(i));
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
        public static Presenter<Data, View> Present<Data, View>(this IEnumerable<Data> list, View prefab, RectTransform container, Action<View, Data> onShow) where View : Component
        {
           return Present(list, prefab, container, (view, data, idx) => onShow?.Invoke(view, data));
        }

        public static Presenter<Data, View> Present<Data, View>(this IEnumerable<Data> list, View prefab, RectTransform container, Action<View, Data, int> onShow) where View : Component
        {
            var presenter = new Presenter<Data, View>();
            if (list is IReadOnlyReactiveList<Data> reactiveList)
            {
                presenter.connections += reactiveList.Subscribe(data => presenter.Present(data, prefab, container, onShow));
            }
            presenter.Present(list, prefab, container, onShow);
            return presenter;
        }
        public static IReadOnlyReactiveList<T> FindAllReactive<T>(this IReadOnlyReactiveList<T> source, Func<T, bool> predicate)
        {
            return new ReactiveListUpdater<T>(val => predicate.Invoke(val), source);
        }
        public static IReadOnlyReactive<T> FindReactive<T>(this IReadOnlyReactiveList<T> source, Func<T, bool> predicate)
        {
            return new ReactiveUpdater<T>(lst => lst.Find(element => predicate.Invoke(element)), source);
        }
        public static IReadOnlyReactiveList<TResult> SelectReactive<TSource, TResult>(this IReadOnlyReactiveList<TSource> source, Func<TSource, TResult> selector)
        {
            return new ReactiveListUpdater<TSource, TResult>(val => selector.Invoke(val), source);
        }
        public static IReadOnlyReactiveList<T> ResizeReactive<T>(this IReadOnlyReactiveList<T> list, int size, T defaultValue = default)
        {
            return new ReactiveListUpdater<T, T>(lst => CreateResizedList(list, size, i => defaultValue), list);
        }
        public static IReadOnlyReactive<bool> AnyReactive<T>(this IReadOnlyReactiveList<T> source, Func<T, bool> predicate)
        {
            return new ReactiveUpdaterBool<T>(lst => lst.Any(element => predicate.Invoke(element)), source);
        }
        public static IReadOnlyReactive<bool> AllReactive<T>(this IReadOnlyReactiveList<T> source, Func<T, bool> predicate)
        {
            return new ReactiveUpdaterBool<T>(lst => lst.All(element => predicate.Invoke(element)), source);
        }
        public static IReadOnlyReactiveList<T> ConcatReactive<T>(this IEnumerable<T> source, IReadOnlyReactiveList<T> updater, Func<IEnumerable<T>, IEnumerable<T>, List<T>> predicate)
        {
            return new ReactiveListUpdater<T, T>(lst => predicate.Invoke(source, lst), updater);
        }
        //public static IReadOnlyReactive<T[]> MergeReactive<T>(params IReadOnlyReactive<T>[] sources)
        //{
        //    return new MergedReactive<T>(sources);
        //}

        public static IReadOnlyReactive<(T1, T2)> MergeReactive<T1, T2>(this IReadOnlyReactive<T1> source1, IReadOnlyReactive<T2> source2)
        {
            return new MergedReactive<T1, T2>(source1, source2);
        }

    }
}