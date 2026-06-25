using System;

namespace Groenteboer.Technova.Devices.Scales
{
    public class ScaleStatusChangedEventArgs : EventArgs
    {
        public ScaleStatus Status { get; }

        public ScaleStatusChangedEventArgs(ScaleStatus status)
        {
            Status = status;
        }

        public override string ToString()
        {
            return Status.ToString();
        }
    }
}
