namespace Wrappers.SolidCreationInfo
{
    internal class FilletEdgesInfo
    {
        public double Radius { get; }

        public double StartSetback { get; }

        public double EndSetback { get; }

        public FilletEdgesInfo(double radius, double startSetback, double endSetback)
        {
            Radius = radius;
            StartSetback = startSetback;
            EndSetback = endSetback;
        }
    }
}
