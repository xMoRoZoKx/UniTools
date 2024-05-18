using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UniTools
{
    public static class TaskTools
    {
        public static async Task DelaySeconds(float secondsDelay) => await Delay((int)(secondsDelay * 1000));
        public static async Task Delay(int millisecondsDelay)
        {
            var tcs = new TaskCompletionSource<bool>();
            await Task.Delay(millisecondsDelay);

            tcs.SetResult(true);
            await tcs.Task;
        }
        public static readonly WaitController taskController = new WaitController();
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
            if(!Application.isPlaying) return;
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