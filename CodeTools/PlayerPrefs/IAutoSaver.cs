using System;
using System.Collections;
using System.Collections.Generic;
using Tools.PlayerPrefs;
using Tools.Reactive;
using UnityEngine;

public interface IAutoSaver<T> 
{
    // public IDisposable OnDataUpdate(Action<string, T> onUpdate);
    public void Save();
}
