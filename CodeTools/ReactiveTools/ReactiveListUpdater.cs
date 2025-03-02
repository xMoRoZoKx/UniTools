
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniTools.Reactive;
using UnityEngine;

//public class MergedReactive<T> : Reactive<T[]>, IDisposable
//{
//    private Connections connections = new();

//    public MergedReactive(IReadOnlyReactive<T>[] sources)
//    {
//        connections += sources.Select(source => source.Subscribe(_ => UpdateValues(sources)));
//        UpdateValues(sources);
//    }

//    private void UpdateValues(IReadOnlyReactive<T>[] sources)
//    {
//        value = sources.Select(s => s.Value).ToArray();
//        InvokeEvents();
//    }

//    public void Dispose() => connections.Dispose();
//}
public class MergedReactive<T1, T2> : Reactive<(T1, T2)>, IDisposable
{
    public MergedReactive(IReadOnlyReactive<T1> source1, IReadOnlyReactive<T2> source2)
    {
        connections += source1.SubscribeAndInvoke(lst =>
        {
            value = (source1.Value, source2.Value);
            InvokeEvents();
        });
        connections += source2.SubscribeAndInvoke(lst =>
        {
            value = (source1.Value, source2.Value);
            InvokeEvents();
        });
    }
    private Connections connections = new();
    public void Dispose()
    {
        connections.Dispose();
    }
}
public class ReactiveUpdater<T> : Reactive<T>, IDisposable
{
    public ReactiveUpdater(Func<List<T>, T> func, IReadOnlyReactiveList<T> updater)
    {
        connection = updater.SubscribeAndInvoke(lst =>
        {
            value = func.Invoke(lst);
            InvokeEvents();
        });
    }
    private IDisposable connection;
    public void Dispose()
    {
        connection.Dispose();
    }
}
public class ReactiveUpdaterBool<T> : Reactive<bool>, IDisposable
{
    public ReactiveUpdaterBool(Func<List<T>, bool> func, IReadOnlyReactiveList<T> updater)
    {
        connection = updater.SubscribeAndInvoke(lst =>
        {
            value = func.Invoke(lst);
            InvokeEvents();
        });
    }
    private IDisposable connection;
    public void Dispose()
    {
        connection.Dispose();
    }
}
public class ReactiveListUpdater<T> : ReactiveList<T>, IDisposable
{
    public ReactiveListUpdater(Func<T, bool> func, IReadOnlyReactiveList<T> updater)
    {
        connection = updater.SubscribeForEachAndInvoke((element, eventType, idx) =>
        {
            if (eventType == CollectionEventType.Added && func.Invoke(element)) Insert(idx, element);
            if (eventType == CollectionEventType.Removed) RemoveAt(idx);
            if (eventType == CollectionEventType.Replace)
            {
                if (func.Invoke(element))
                    this[idx] = element;
                else RemoveAt(idx);
            }
        });
    }
    private IDisposable connection;
    public void Dispose()
    {
        connection.Dispose();
    }
}

public class ReactiveListUpdater<T, T2> : ReactiveList<T2>, IDisposable
{
    public ReactiveListUpdater(Func<List<T>, List<T2>> func, IReadOnlyReactiveList<T> updater)
    {
        connection = updater.SubscribeAndInvoke(t => SetValue(func.Invoke(t)));
    }
    public ReactiveListUpdater(Func<T, T2> func, IReadOnlyReactiveList<T> updater)
    {
        connection = updater.SubscribeForEachAndInvoke((element, eventType, idx) =>
        {
            if (eventType == CollectionEventType.Added) Insert(idx, func.Invoke(element));
            if (eventType == CollectionEventType.Removed) RemoveAt(idx);
            if (eventType == CollectionEventType.Replace) this[idx] = func.Invoke(element);
        });
    }
    private IDisposable connection;
    public void Dispose()
    {
        connection.Dispose();
    }
}
