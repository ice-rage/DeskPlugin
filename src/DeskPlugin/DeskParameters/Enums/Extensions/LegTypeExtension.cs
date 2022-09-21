namespace DeskParameters.Enums.Extensions
{
    /// <summary>
    /// Класс, расширяющий перечисление типов ножек письменного стола <see cref="LegType"/>.
    /// </summary>
    public static class LegTypeExtension
    {
        #region Methods

        /// <summary>
        /// Метод для получения типа основания ножек, хранящегося в перечислении
        /// <see cref="ParameterType"/>.
        /// </summary>
        /// <param name="legType">Тип ножек, выраженный значением перечисления
        /// <see cref="LegType"/>.</param>
        /// <returns>Тип основания ножек (значение перечисления <see cref="ParameterType"/>).
        /// </returns>
        public static ParameterType GetLegBaseType(this LegType legType) => legType == LegType.Round
            ? ParameterType.LegBaseDiameter
            : ParameterType.LegBaseLength;

        #endregion
    }
}
