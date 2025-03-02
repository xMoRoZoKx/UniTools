using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ButtonEditor 
{
    public void OnInspectorGUI(UnityEngine.Object[] targets)
    {
        foreach (var target in targets)
        {
            var mis = target.GetType().GetMethods().Where(m => m.GetCustomAttributes().Any(a => a.GetType() == typeof(EditorButtonAttribute)));
            if (mis != null)
            {
                foreach (var mi in mis)
                {
                    if (mi != null)
                    {
                        var attribute = (EditorButtonAttribute)mi.GetCustomAttribute(typeof(EditorButtonAttribute));
                        if (GUILayout.Button(attribute.name))
                        {
                            mi.Invoke(target, null);
                        }
                    }
                }
            }
        }
    }
}