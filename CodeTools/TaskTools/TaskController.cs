using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Tools
{
    public class TaskController
    {
        public bool IsPaused { get; private set; }
        public bool IsStopped { get; private set; }
        public bool PlayInEditorMode = false;
        static CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        static CancellationToken token = cancelTokenSource.Token;
        public Task WaitForSeconds(float seconds, bool playInEditorMode = false)
        {
            return WaitForMilliseconds((long)seconds * 1000, playInEditorMode);
        }
        public Task WaitForMilliseconds(long milliseconds, bool playInEditorMode = false, int accuracy = 1)
        {
            if ((!Application.isPlaying && !playInEditorMode) || IsStopped)
            {
                Cancel();
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
        public void Cancel()
        {
            cancelTokenSource.Cancel();
            cancelTokenSource.Dispose();
        }
        public void Stop()
        {
            IsPaused = true;
            IsStopped = true;
        }
    }
}