namespace Parameters.Enums.Extensions
{
    /// <summary>
    /// Статический класс, расширяющий перечисление типов ручек ящиков для канцелярии
    /// <see cref="DrawerHandleType"/>.
    /// </summary>
    public static class DrawerHandleTypeExtensions
    {
        /// <summary>
        /// Статический метод, возвращающий тип ручек ящиков для канцелярии
        /// из перечисления <see cref="DeskParameterType"/>.
        /// </summary>
        /// <param name="handleType"> Тип ручек ящиков для канцелярии, выраженный
        /// значением перечисления <see cref="DrawerHandleType"/>.</param>
        /// <returns> Тип ручек ящиков для канцелярии (значение перечисления
        /// <see cref="DeskParameterType"/>).</returns>
        public static DeskParameterType GetHandleDimensionType(
            this DrawerHandleType handleType)
        {
            switch (handleType)
            {
                case DrawerHandleType.Railing:
                {
                    return DeskParameterType.DrawerRailingHandleFastenerDistance;
                }
                case DrawerHandleType.Knob:
                {
                    return DeskParameterType.DrawerKnobHandleBaseDiameter;
                }
                case DrawerHandleType.Grip:
                default:
                {
                    return DeskParameterType.DrawerGripHandleFastenerDistance;
                }
            }
        }
    }
}
