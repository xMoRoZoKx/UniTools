using System;
using System.Threading.Tasks;
namespace Tools
{
    public static class TaskTools
    {
        public static readonly TaskController taskController = new TaskController();
        public static Task WaitForSeconds(float value, bool playInEditorMode = true) => WaitForMilliseconds((long)value * 1000);
        public static Task WaitForMilliseconds(long value, bool playInEditorMode = true) => taskController.WaitForMilliseconds(value);
        public static async void Wait(float time, Action waitEvent, bool playInEditorMode = false)
        {
            await WaitForSeconds(time, playInEditorMode);
            waitEvent.Invoke();
        }
    }
}