namespace Parameters.Enums.Extensions
{
    /// <summary>
    /// Статический класс, расширяющий перечисление типов ножек <see cref="LegType"/>.
    /// </summary>
    public static class LegTypeExtension
    {
        #region Methods

        /// <summary>
        /// Статический метод для получения типа основания ножек письменного стола.
        /// </summary>
        /// <param name="legType"> Тип ножек, выраженный значением перечисления
        /// <see cref="LegType"/>.</param>
        /// <returns> Тип основания ножек (значение перечисления
        /// <see cref="DeskParameterType"/>).</returns>
        public static DeskParameterType GetLegBaseType(this LegType legType) => 
            legType == LegType.Round 
                ? DeskParameterType.LegBaseDiameter 
                : DeskParameterType.LegBaseLength;

        #endregion
    }
}
