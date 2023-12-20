using System;
using System.Collections;
using System.Collections.Generic;
using UniTools.PlayerPrefs;
using UniTools.Reactive;
using UnityEngine;

public interface IAutoSaver
{
    // public IDisposable OnDataUpdate(Action<string, T> onUpdate);
    public void Save();
}
