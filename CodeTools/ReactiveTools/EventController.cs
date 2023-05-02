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
        actions.ForEach(a => a.Invoke(value));
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
