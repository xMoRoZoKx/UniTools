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
    }
}
