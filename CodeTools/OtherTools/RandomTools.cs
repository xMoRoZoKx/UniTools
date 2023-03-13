using System;
using System.Collections;
using System.Collections.Generic;
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
    }
}