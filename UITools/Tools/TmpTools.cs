using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UniTools.Reactive;

public static class TmpTools
{
    public static void SetValue(this TMP_Dropdown dropdown, string val) => dropdown.value = dropdown.options.FindIndex(option => option.text == val);
    public static void SetOptionsFromEnum<T>(this TMP_Dropdown dropdown) where T : Enum => dropdown.SetOptionsFromEnum(typeof(T));
    public static void SetOptionsFromEnum(this TMP_Dropdown dropdown, Type enumType)
    {
        var options = new List<string>();
        foreach (var era in Enum.GetValues(enumType))
        {
            options.Add(era.ToString());
        }
        dropdown.SetOptionsFromString(options.ToArray());
    }
    public static void SetOptionsFromString(this TMP_Dropdown dropdown, string[] names)
    {
        var options = new List<TMP_Dropdown.OptionData>();
        foreach (var optName in names)
        {
            options.Add(new TMP_Dropdown.OptionData() { text = optName.ToString() });
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
    }
    public static IDisposable SetTextReactive<T>(this TMP_Text tmpText, IReadOnlyReactive<T> reactive, Func<T, string> textFunc = null)
    {
        return reactive?.SubscribeAndInvoke(val => tmpText.text = textFunc == null ? val?.ToString() : textFunc?.Invoke(val));
    }
}
