using System;
using System.Threading;
using System.Threading.Tasks;

namespace Groenteboer.Technova.Devices.Scales.Mock
{
    public abstract class TimeRandomizedScale : Scale
    {
        private CancellationTokenSource _cts;
        private Task _simulationTask;
        private readonly object _lifecycleLock = new object();
        private readonly Random _random = new Random();

        protected TimeRandomizedScale(string name)
        {
            Name = name;
        }

        public override bool IsConnected()
        {
            return true;
        }

        public override void Start()
        {
            lock (_lifecycleLock)
            {
                if (_simulationTask != null && !_simulationTask.IsCompleted)
                {
                    return;
                }

                if (_cts != null)
                {
                    _cts.Dispose();
                }

                _cts = new CancellationTokenSource();

                SetStatus(ScaleStatus.Ready);

                CancellationToken token = _cts.Token;

                _simulationTask = Task.Run(delegate
                {
                    return RunSimulation(token);
                });
            }
        }

        public override void Stop()
        {
            lock (_lifecycleLock)
            {
                CancellationTokenSource cts = _cts;

                if (cts != null)
                {
                    cts.Cancel();
                }

                _cts = null;

                if (cts != null && _simulationTask != null)
                {
                    _simulationTask.ContinueWith(delegate
                    {
                        cts.Dispose();
                    });
                }
            }

            SetStatus(ScaleStatus.Disconnected);
        }

        private async Task RunSimulation(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    int targetWeight = _random.Next(1, 1001);

                    await MoveToWeight(targetWeight, token);

                    await Task.Delay(5000, token);
                }
            }
            catch (TaskCanceledException)
            {
                // Simulatie gestopt.
            }
        }

        private async Task MoveToWeight(int targetWeight, CancellationToken token)
        {
            double currentWeight = 0;

            SetWeight(currentWeight, "g");

            int steps = _random.Next(5, 12);

            for (int i = 1; i <= steps; i++)
            {
                double progress = (double)i / steps;
                double weight = targetWeight * progress;

                weight += _random.NextDouble() * 40 - 20;

                if (weight < 0)
                {
                    weight = 0;
                }

                if (i == steps)
                {
                    weight = targetWeight;
                }

                SetWeight(Math.Round(weight, 0), "g");

                await Task.Delay(_random.Next(200, 700), token);
            }
        }
    }
}
