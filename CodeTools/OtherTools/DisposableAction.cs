using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSCodeEditor;

public class DisposableAction : IDisposable
{
    Action _action;
    public DisposableAction(Action action)
    {
        _action = action;
    }
    public void Dispose()
    {
        _action?.Invoke();
    }
}
