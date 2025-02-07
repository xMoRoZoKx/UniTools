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
        public static bool IsNullOrEmpty(this string value)
        {
            return value == null || value.Length == 0;
        }

        public static T To<T>(this System.Object obj)
        {
            if (obj is T result) return result;
            else Debug.LogError("Cast error");
            return default;
        }
        public static string ToCamelCase(this string input) => SplitCamelCase(input);
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
        public static Type GetElementType(this IEnumerable ienumerable) => ienumerable.GetType().GetElementType();
        public static Type GetGenericType(this object generic, int index) => generic.GetType().GetTypeInfo().GenericTypeArguments[index];
        public static Type GetEnumerableType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return type.GetGenericArguments()[0];

            var iface = (from i in type.GetInterfaces()
                         where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                         select i).FirstOrDefault();

            if (iface == null)
                throw new ArgumentException("Does not represent an enumerable type.", "type");

            return GetEnumerableType(iface);
        }
    }
}
