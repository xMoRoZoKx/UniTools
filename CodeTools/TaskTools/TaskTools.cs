using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Tools
{
    public static class TaskTools
    {
        public static readonly TaskController taskController = new TaskController();
        public static Task WaitForSeconds(float value, bool playInEditorMode = false) => WaitForMilliseconds((long)(value * 1000), playInEditorMode);
        public static Task WaitForMilliseconds(long value, bool playInEditorMode = false) => taskController.WaitForMilliseconds(value, playInEditorMode);
        public static async Task Wait(this Component component, long seconds, Action waitEvent)
        {
            await WaitForSeconds(seconds, false);
            if(component == null) return;
            waitEvent?.Invoke();
        }
        public static async void Wait(float time, Action waitEvent, bool playInEditorMode = false)
        {
            await WaitForSeconds(time, playInEditorMode);
            waitEvent.Invoke();
        }
    }
}