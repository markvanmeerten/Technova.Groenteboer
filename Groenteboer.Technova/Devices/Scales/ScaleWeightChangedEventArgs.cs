namespace Groenteboer.Technova.Devices.Scales
{
    public class ScaleWeightChangedEventArgs : EventArgs
    {
        public double Weight { get; }
        public string Unit { get; }

        public ScaleWeightChangedEventArgs(double weight, string unit)
        {
            Weight = weight;
            Unit = unit;
        }

        public override string ToString()
        {
            return $"{Weight:0.###} {Unit}";
        }
    }
}
