using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BaseEditorRunner : Editor
{
    Serializable2DArrayEditor array2D = new();
    ButtonEditor buttonEditor = new();
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();

        array2D.OnInspectorGUI(target);
        buttonEditor.OnInspectorGUI(targets);

        serializedObject.ApplyModifiedProperties();
    }
}
