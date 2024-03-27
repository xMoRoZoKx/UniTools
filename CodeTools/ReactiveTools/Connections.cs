using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Connections : IDisposable
{
    private List<IDisposable> connects = new();
    public static Connections operator +(Connections connections, IDisposable dispos)
    {
        connections.connects.Add(dispos);
        return connections;
    }
    public void Add(Action action)
    {
        connects.Add(new DisposableAction(action));
    }
    public void DisconnectAll()
    {
        connects.ForEach(c => c?.Dispose());
        connects.Clear();
    }
    public void Dispose()
    {
        DisconnectAll();
    }
}
