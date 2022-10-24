using System.Windows.Media.Media3D;

namespace Wrappers.SolidCreationInfo
{
    internal class RevolutionInfo
    {
        public Point3D StartAxisPoint { get; }

        public Point3D EndAxisPoint { get; }

        public double Angle { get; }

        public RevolutionInfo(Point3D startAxisPoint, Point3D endAxisPoint, 
            double angle)
        {
            StartAxisPoint = startAxisPoint;
            EndAxisPoint = endAxisPoint;
            Angle = angle;
        }
    }
}
