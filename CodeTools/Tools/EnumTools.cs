using System;

namespace UniTools
{
    public static class EnumTools
    {
        public static T[] GetValues<T>() where T : Enum => (T[])Enum.GetValues(typeof(T));
        public static T ToEnum<T>(this string str) where T : Enum
        {
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                if (e.ToString() == str) return (T)e;
            }
            return default;
        }
        public static bool TryConvertToEnum<T>(this string str) where T : Enum
        {
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                if (e.ToString() == str) return true;
            }
            return false;
        }
        public static T GetRandomValue<T>() where T : Enum
        {
            var enums = Enum.GetValues(typeof(T));
            return (T)enums.GetValue(UnityEngine.Random.Range(0, enums.Length));
        }
    }
}