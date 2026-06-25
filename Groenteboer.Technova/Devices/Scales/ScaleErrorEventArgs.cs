using System;

namespace Groenteboer.Technova.Devices.Scales
{
    public class ScaleErrorEventArgs : EventArgs
    {
        public string Message { get; }

        public ScaleErrorEventArgs(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}