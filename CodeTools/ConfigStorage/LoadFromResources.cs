using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
sealed class LoadFromResources : Attribute
{
    public string Patch { get; private set; }
    public LoadFromResources(string patch = "")
    {
        Patch = patch;
    }
}