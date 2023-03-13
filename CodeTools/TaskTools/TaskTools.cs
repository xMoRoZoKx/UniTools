using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
namespace Tools
{
    public static class TaskTools
    {
        public static readonly TaskController taskController = new TaskController();
        public static Task WaitForSeconds(float value, bool playInEditorMode = true) => WaitForMilliseconds((long)value * 1000);
        public static Task WaitForMilliseconds(long value, bool playInEditorMode = true) => taskController.WaitForMilliseconds(value);
        public static async void Wait(float time, Action waitEvent)
        {
            await WaitForSeconds(time, false);
            waitEvent.Invoke();
        }
    }
}