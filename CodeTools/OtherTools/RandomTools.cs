using System;
using UnityEngine;

namespace Tools
{
    public static class RandomTools
    {
        public static void InvokWithChance(Action action, int chance)
        {
            if (chance == 0) return;
            if (UnityEngine.Random.Range(0, 100) <= chance) action?.Invoke();
        }

        public static Vector3 GetRandomPointInRange(Vector3 centre, float range, bool useX = true, bool useY = true, bool useZ = true)
        {
            return new Vector3(centre.x + (useX ? GetRandom() : 0), centre.y + (useY ? GetRandom() : 0), centre.z + (useZ ? GetRandom() : 0));

            float GetRandom() => UnityEngine.Random.Range(-range, range);
        }

    }
}