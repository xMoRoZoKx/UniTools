using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
