namespace Parameters.Enums.Extensions
{
    /// <summary>
    /// Класс, расширяющий перечисление типов ножек письменного стола <see cref="LegType"/>.
    /// </summary>
    public static class LegTypeExtension
    {
        #region Methods

        /// <summary>
        /// Метод для получения типа основания ножек, хранящегося в перечислении
        /// <see cref="DeskParameterType"/>.
        /// </summary>
        /// <param name="legType"> Тип ножек, выраженный значением перечисления
        /// <see cref="LegType"/>.</param>
        /// <returns> Тип основания ножек (значение перечисления <see cref="DeskParameterType"/>).
        /// </returns>
        public static DeskParameterType GetLegBaseType(this LegType legType) => 
            legType == LegType.Round 
                ? DeskParameterType.LegBaseDiameter 
                : DeskParameterType.LegBaseLength;

        #endregion
    }
}
