using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UniTools;
using UnityEngine;

public class ConfigStorage
{
    public ConfigStorage()
    {
        LoadResources();
    }
    protected void LoadResources()
    {
        Type objectType = this.GetType();

        foreach (FieldInfo field in objectType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            var loadAttribute = field.GetCustomAttribute<LoadFromResources>();
            if (loadAttribute != null)
            {
                if (typeof(IEnumerable).IsAssignableFrom(field.FieldType))
                {
                    var elementType = field.FieldType.GetEnumerableType();
                    var objects = Resources.LoadAll(loadAttribute.Patch, elementType);

                    if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        var listType = typeof(List<>).MakeGenericType(elementType);
                        var list = Activator.CreateInstance(listType) as IList;

                        foreach (var obj in objects)
                        {
                            list.Add(obj);
                        }

                        field.SetValue(this, list);
                    }
                    else if (field.FieldType.IsArray)
                    {
                        Array arr = Array.CreateInstance(elementType, objects.Length);
                        Array.Copy(objects, arr, objects.Length);
                        field.SetValue(this, arr);
                    }
                    else
                    {
                        Debug.LogError($"Field {field.Name} of type {field.FieldType} is not supported for loading from Resources.");
                    }
                }
            }
        }
    }
}
