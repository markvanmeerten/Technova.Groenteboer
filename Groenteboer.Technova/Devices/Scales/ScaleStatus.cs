namespace Groenteboer.Technova.Devices.Scales
{
    public enum ScaleStatus
    {
        Disconnected, // No USB connection
        Standby,      // Connected but scale is sleeping/off
        Ready         // Connected and operational
    }
}