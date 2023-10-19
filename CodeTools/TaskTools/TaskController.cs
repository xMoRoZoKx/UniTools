using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UniTools
{
    public class WaitController
    {
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public bool IsPaused { get; private set; }
        public bool IsStopped { get; private set; }

        public async Task WaitForSeconds(float seconds, bool playInEditorMode = false)
        {
            await WaitForMilliseconds((int)(seconds * 1000), playInEditorMode);
        }

        public async Task WaitForMilliseconds(int milliseconds, bool playInEditorMode = false, int accuracy = 10)
        {
            if (!Application.isPlaying && !playInEditorMode || IsStopped)
            {
                Cancel();
                return;
            }

            if (accuracy < 1)
            {
                accuracy = 1;
            }

            long timer = milliseconds;

            while (!IsStopped && timer > 0)
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();

                while (!IsPaused && !IsStopped && timer > 0)
                {
                    await Task.Delay(accuracy);
                    timer -= sw.ElapsedMilliseconds;
                    sw.Restart();
                }

                await Task.Delay(accuracy);
            }
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
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Stop()
        {
            IsPaused = true;
            IsStopped = true;
        }
    }
}