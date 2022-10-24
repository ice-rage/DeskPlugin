namespace Parameters.Enums.Extensions
{
    public static class DrawerHandleTypeExtensions
    {
        public static DeskParameterType GetHandleDimensionType(
            this DrawerHandleType handleType)
        {
            switch (handleType)
            {
                case DrawerHandleType.Railing:
                {
                    return DeskParameterType.DrawerRailingHandleFastenerDistance;
                }

                case DrawerHandleType.Grip:
                {
                    return DeskParameterType.DrawerGripHandleFastenerDistance;
                }

                case DrawerHandleType.Knob:
                default:
                {
                    return DeskParameterType.DrawerKnobHandleBaseDiameter;
                }
            }
        }
    }
}
