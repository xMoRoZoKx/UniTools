using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Tools
{
    public static class TaskTools
    {
        public static readonly TaskController taskController = new TaskController();
        public static Task WaitForSeconds(float value, bool playInEditorMode = false) => WaitForMilliseconds((int)(value * 1000), playInEditorMode);
        public static Task WaitForMilliseconds(int value, bool playInEditorMode = false) => taskController.WaitForMilliseconds(value, playInEditorMode);
        public static async void Wait(this Component component, float seconds, Action waitEvent)
        {
            await WaitForSeconds(seconds, false);
            if (component == null) return;
            waitEvent?.Invoke();
        }
        public static async void Wait(float time, Action waitEvent, bool playInEditorMode = false)
        {
            await WaitForSeconds(time, playInEditorMode);
            waitEvent.Invoke();
        }
        // public static Task Wait(AsyncOperation operation)
        // {
        //     if(operation == null) return WaitForSeconds(0);
        //     return Task.Run(() =>
        //     {
        //         while (!operation.isDone)
        //         {
        //             Thread.Sleep(TimeSpan.FromMilliseconds(100));
        //         }
        //     });
        // }
    }
}