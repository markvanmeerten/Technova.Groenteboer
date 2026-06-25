using System;

namespace Groenteboer.Technova.Devices.Scales
{
    public interface IScale : IDisposable
    {
        double CurrentWeight { get; }
        string CurrentUnit { get; }
        ScaleStatus Status { get; }

        event EventHandler<ScaleWeightChangedEventArgs> WeightChanged;
        event EventHandler<ScaleStatusChangedEventArgs> StatusChanged;
        event EventHandler<ScaleErrorEventArgs> ErrorOccurred;

        bool IsConnected();

        void Start();
        void Stop();
    }
}
