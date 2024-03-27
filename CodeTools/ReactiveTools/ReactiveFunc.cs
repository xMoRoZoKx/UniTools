using System;
using System.Collections;
using System.Collections.Generic;
using UniTools.Reactive;
using UnityEngine;

public class ReactiveFunc<T, T2> : Reactive<T2>
{
    public Func<T, T2> func;
    public IReadOnlyReactive<T> updater;
    public override T2 value { get => func.Invoke(updater.Value); }

    public override IDisposable Subscribe(Action<T2> onChangedEvent)
    {
        return updater.Subscribe(() => onChangedEvent(value));
    }
}
