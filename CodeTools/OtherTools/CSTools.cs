using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace UniTools
{
    public static class CSTools
    {
        public static T To<T>(this System.Object obj)
        {
            if (obj is T result) return result;
            return default;
        }
        public static string SplitCamelCase(this string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }
        public static List<T> GetValuesFrom<T>(this object from, Type[] targetAttributes = default, Type[] ignoreAttributes = default)
        {
            Type objectType = from.GetType();

            List<T> results = new();

            foreach (FieldInfo field in objectType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (targetAttributes != default && !targetAttributes.Any(i => field.IsDefined(i))) continue;
                if (ignoreAttributes != default && ignoreAttributes.Any(i => field.IsDefined(i))) continue;

                var value = field.GetValue(from);
                if (value is T t_value)
                    results.Add(t_value);
            }

            return results;
        }
    }
}
