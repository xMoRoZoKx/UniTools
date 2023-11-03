using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UniTools
{
    public static class CSTools
    {
        public static T To<T>(this Object obj)
        {
            if (obj is T result) return result;
            return default;
        }
        public static string SplitCamelCase(this string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }
    }
}
