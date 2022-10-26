namespace Parameters.Enums.Extensions
{
    /// <summary>
    /// Статический класс, расширяющий перечисление типов ручек ящиков для канцелярии
    /// <see cref="DrawerHandleType"/>.
    /// </summary>
    public static class DrawerHandleTypeExtension
    {
        /// <summary>
        /// Статический метод, возвращающий тип изменяемого параметра ручки ящика
        /// для канцелярии из перечисления <see cref="DeskParameterType"/>.
        /// </summary>
        /// <param name="handleType"> Тип ручки ящика для канцелярии, выраженный
        /// значением перечисления <see cref="DrawerHandleType"/>.</param>
        /// <returns> Тип изменяемого параметра ручки ящика для канцелярии (значение
        /// перечисления <see cref="DeskParameterType"/>).</returns>
        public static DeskParameterType GetHandleMutableParameterType(
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
