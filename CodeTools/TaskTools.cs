using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class TaskTools
{
    public class TaskWaiting
    {
        public bool IsPaused { get; private set; }
        public bool IsStopped { get; private set; }
        public bool PlayInEditorMode = false;
        static CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        static CancellationToken token = cancelTokenSource.Token;
        public Task WaitForSeconds(float seconds)
        {
            return WaitForMilliseconds((long)seconds * 1000, 100);
        }
        public Task WaitForMilliseconds(long milliseconds, int accuracy = 1)
        {
            if (!Application.isPlaying)
            {
                cancelTokenSource.Cancel();
                cancelTokenSource.Dispose();
                return null;
            }
            return Task.Run(() =>
            {
                var startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                if (accuracy < 1) accuracy = 1;
                long timer = milliseconds;
                while (!IsStopped && timer > 0)
                {
                    long timeToWait = 0;
                    while (!IsPaused && !IsStopped && timer > 0)
                    {
                        timeToWait = DateTimeOffset.Now.ToUnixTimeMilliseconds();// temp problem
                        Thread.Sleep(TimeSpan.FromMilliseconds(accuracy));
                        timer -= DateTimeOffset.Now.ToUnixTimeMilliseconds() - timeToWait;//
                    }

                    if (IsStopped || timer <= 0) break;
                    Thread.Sleep(accuracy);
                }
            }, token);
        }
        public void Pause()
        {
            IsPaused = true;
        }
        public void Resume()
        {
            IsPaused = false;
        }
        public void Stop()
        {
            IsPaused = true;
            IsStopped = true;
        }
    }
    static CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
    static CancellationToken token = cancelTokenSource.Token;
    public static Task WaitForSeconds(float value, bool playInEditorMode = true)
    {
        return WaitForMilliseconds((long)value * 1000);
    }

    public static Task WaitForMilliseconds(long value, bool playInEditorMode = true)
    {
        if (!Application.isPlaying)
        {
            cancelTokenSource.Cancel();
            cancelTokenSource.Dispose();
            return null;
        }
        if (!playInEditorMode)
            return Task.Delay(TimeSpan.FromMilliseconds(value));
        else
            return Task.Delay(TimeSpan.FromMilliseconds(value), token);
    }
    public static async void Wait(float time, Action waitEvent)
    {
        await WaitForSeconds(time, false);
        waitEvent.Invoke();
    }
}