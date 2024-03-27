using System;
using System.Collections;
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
                    var objects = Resources.LoadAll(loadAttribute.Patch, field.FieldType.GetEnumerableType());

                    Array arr = Array.CreateInstance(field.FieldType.GetEnumerableType(), objects.Length);
                    Array.Copy(objects, arr, objects.Length);

                    // var secondListType = typeof(List<>).MakeGenericType(field.FieldType.GetEnumerableType());
                    // var result = Activator.CreateInstance(secondListType);
                    // IList list = (IList)result;
                    // list.AddRange(arr);

                    field.SetValue(this, arr);//Convert.ChangeType(value, field.FieldType)
                }
                else
                {
                    var values = Resources.LoadAll(loadAttribute.Patch, field.FieldType);
                    if (values.Length == 0)
                    {
                        Debug.LogError($"you dont have any {field.FieldType} by patch \"Resources/{loadAttribute.Patch}\"");
                        continue;
                    }
                    field.SetValue(this, values[0]);
                }
            }
        }
    }
}
