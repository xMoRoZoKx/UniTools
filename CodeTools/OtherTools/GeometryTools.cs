using System;
using UnityEngine;

namespace UniTools
{
    public static class GeometryTools
    {
        public static Vector3 Direction(this Vector3 from, Vector3 to) => (to - from).normalized;
        public static Vector3 WithZ(this Vector3 from, float z) => new Vector3(from.x, from.y, z);
        public static Vector3 WithX(this Vector3 from, float x) => new Vector3(x, from.y, from.z);
        public static Vector3 WithY(this Vector3 from, float y) => new Vector3(from.x, y, from.z);
        public static Vector3 WithZ(this Vector3 from, Func<float, float> zFunc) => new Vector3(from.x, from.y, zFunc.Invoke(from.z));
        public static Vector3 WithX(this Vector3 from, Func<float, float> xFunc) => new Vector3(xFunc.Invoke(from.x), from.y, from.z);
        public static Vector3 WithY(this Vector3 from, Func<float, float> yFunc) => new Vector3(from.x, yFunc.Invoke(from.y), from.z);
        public static Vector2 Direction(this Vector2 from, Vector2 to) => (to - from).normalized;
        public static Vector2 WithX(this Vector2 from, float x) => new Vector2(x, from.y);
        public static Vector2 WithY(this Vector2 from, float y) => new Vector2(from.x, y);
        public static Vector3 ToVector3(this Vector2 vector) => new Vector3(vector.x, vector.y, 0);
        public static Vector2 ToVector2(this Vector3 vector) => new Vector2(vector.x, vector.y);
    }
}