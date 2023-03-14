using System;
using System.Collections.Generic;

public class EventController<T>
{
    List<Action<T>> actions = new List<Action<T>>();
    public void Subscribe(Action<T> action)
    {
        actions.Add(action);
    }
    public void Invoke(T value)
    {
        actions.ForEach(a => a.Invoke(value));
    } 
    public static EventController<T> operator +(EventController<T> a, Action<T> b)
    {
        a.Subscribe(b);
        return a;
    }
}
