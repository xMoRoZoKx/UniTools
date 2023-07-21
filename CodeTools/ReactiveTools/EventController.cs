using System;
using System.Collections.Generic;

public class EventStream<T> : IDisposable
{
    List<Action<T>> actions = new List<Action<T>>();
    public IDisposable Subscribe(Action<T> action)
    {
        actions.Add(action);
        return new DisposableEvent(() => actions.Remove(action));
    }
    public void Invoke(T value)
    {
        var count = actions.Count;
        for (int i = 0; i < count; i++)
        {
            actions[i]?.Invoke(value);
        }
    }
    public void DisonnectAll() => actions.Clear();

    public void Dispose()
    {
        DisonnectAll();
    }

    public static EventStream<T> operator +(EventStream<T> a, Action<T> b)
    {
        a.Subscribe(b);
        return a;
    }
    private class DisposableEvent : IDisposable
    {
        public DisposableEvent(Action onDispose)
        {
            _onDispose = onDispose;
        }
        private Action _onDispose;
        public void Dispose()
        {
            _onDispose?.Invoke();
        }
    }
}
public class EventStream : IDisposable
{
    List<Action> actions = new List<Action>();
    public IDisposable Subscribe(Action action)
    {
        actions.Add(action);
        return new DisposableEvent(() => actions.Remove(action));
    }
    public void Invoke()
    {
        var count = actions.Count;
        for (int i = 0; i < count; i++)
        {
            actions[i]?.Invoke();
        }
    }
    public void DisonnectAll() => actions.Clear();

    public void Dispose()
    {
        DisonnectAll();
    }

    public static EventStream operator +(EventStream a, Action b)
    {
        a.Subscribe(b);
        return a;
    }
    private class DisposableEvent : IDisposable
    {
        public DisposableEvent(Action onDispose)
        {
            _onDispose = onDispose;
        }
        private Action _onDispose;
        public void Dispose()
        {
            _onDispose?.Invoke();
        }
    }
}
