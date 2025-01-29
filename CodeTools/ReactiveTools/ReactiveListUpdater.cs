using System;
using System.Collections;
using System.Collections.Generic;
using UniTools.Reactive;
using UnityEngine;

public class ReactiveListUpdater<T, T2> : ReactiveList<T2>, IDisposable
{
    public ReactiveListUpdater(Func<List<T>, List<T2>> func, IReadOnlyReactiveList<T> updater)
    {
        connection = updater.SubscribeAndInvoke(t => SetValue(func.Invoke(t)));
    }
    private IDisposable connection;
    public void Dispose()
    {
        connection.Dispose();
    }
}
