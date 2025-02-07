using System;
using System.Collections;
using System.Collections.Generic;
using UniTools.Reactive;
using UnityEngine;

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
