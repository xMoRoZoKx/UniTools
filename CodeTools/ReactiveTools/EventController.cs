using System;
using System.Collections.Generic;

public interface IEventStream<T>
{
    public IDisposable Subscribe(Action<T> action);
}
[System.Serializable]
public class EventStream<T> : IEventStream<T>, IDisposable
{
    List<(Action<T>, bool)> actions = new List<(Action<T>, bool)>();
    public IDisposable Subscribe(Action<T> action)
    {
        var item = (action, false);
        actions.Add(item);
        return new DisposableEvent(() => actions.Remove(item));
    }
    public IDisposable SubscribeOnce(Action<T> action)
    {
        var item = (action, true);
        actions.Add(item);
        return new DisposableEvent(() => actions.Remove(item));
    }
    public void Invoke(T value)
    {
        var count = actions.Count;
        for (int i = 0; i < count; i++)
        {
            actions[i].Item1?.Invoke(value);
        }
        actions.RemoveAll(a => a.Item2);
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
public interface IEventStream
{
    public IDisposable Subscribe(Action action);
}
[System.Serializable]
public class EventStream : IEventStream, IDisposable
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
