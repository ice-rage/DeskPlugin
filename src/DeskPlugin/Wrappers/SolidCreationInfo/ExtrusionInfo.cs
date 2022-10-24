namespace Wrappers.SolidCreationInfo
{
    internal class ExtrusionInfo
    {
        public double Height { get; }

        public double TaperAngle { get; }

        public bool IsExtrusionCuttingOut { get; }

        public bool IsDirectionPositive { get; }

        public ExtrusionInfo(
            double height,
            double taperAngle,
            bool isExtrusionCuttingOut, 
            bool isDirectionPositive)
        {
            Height = height;
            TaperAngle = taperAngle;
            IsExtrusionCuttingOut = isExtrusionCuttingOut;
            IsDirectionPositive = isDirectionPositive;
        }
    }
}
