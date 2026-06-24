using Groenteboer.Technova.Devices.Scales.Dymo;
using Groenteboer.Technova.Devices.Scales.Mock;

namespace Groenteboer.Technova.Devices.Scales
{
    public static class ScaleFactory
    {
        public static IScale CreateDefault()
        {
            var dymoM10 = CreateDymoM10Scale();

            if (dymoM10.IsConnected())
            {
                return dymoM10;
            }

            return CreateTestScale();
        }

        public static DymoM10Scale CreateDymoM10Scale()
        {
            return new DymoM10Scale();
        }

        public static TestScale CreateTestScale()
        {
            return new TestScale();
        }
    }
}
