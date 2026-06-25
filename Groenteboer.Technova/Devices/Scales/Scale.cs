using System;

namespace Groenteboer.Technova.Devices.Scales
{
    public abstract class Scale : IScale
    {
        private readonly object _stateLock = new object();

        protected string Name { get; set; } = "Scale";

        public double CurrentWeight { get; protected set; }
        public string CurrentUnit { get; protected set; } = "";
        public ScaleStatus Status { get; protected set; } = ScaleStatus.Disconnected;

        public event EventHandler<ScaleWeightChangedEventArgs> WeightChanged;
        public event EventHandler<ScaleStatusChangedEventArgs> StatusChanged;
        public event EventHandler<ScaleErrorEventArgs> ErrorOccurred;

        public abstract bool IsConnected();
        public abstract void Start();
        public abstract void Stop();

        public virtual void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }

        public override string ToString()
        {
            return Name;
        }

        protected void SetWeight(double weight, string unit)
        {
            lock (_stateLock)
            {
                if (weight == CurrentWeight && unit == CurrentUnit)
                {
                    return;
                }

                CurrentWeight = weight;
                CurrentUnit = unit;
            }

            var handler = WeightChanged;
            if (handler != null)
            {
                handler(this, new ScaleWeightChangedEventArgs(weight, unit));
            }
        }

        protected void SetStatus(ScaleStatus status)
        {
            lock (_stateLock)
            {
                if (Status == status)
                {
                    return;
                }

                Status = status;
            }

            var handler = StatusChanged;
            if (handler != null)
            {
                handler(this, new ScaleStatusChangedEventArgs(status));
            }
        }

        protected void ReportError(string message)
        {
            var handler = ErrorOccurred;
            if (handler != null)
            {
                handler(this, new ScaleErrorEventArgs(message));
            }
        }
    }
}
