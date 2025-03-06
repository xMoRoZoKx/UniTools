using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniTools.Reactive;
using UniTools;
using UnityEngine;

public static class ReactiveLinq
{

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
        return new ReactiveListUpdater<T, T>(lst => lst.CreateResizedList(size, i => defaultValue), list);
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

    public static IReadOnlyReactive<(T1, T2)> MergeReactive<T1, T2>(this IReadOnlyReactive<T1> source1, IReadOnlyReactive<T2> source2)
    {
        return new MergedReactive<T1, T2>(source1, source2);
    }
}
