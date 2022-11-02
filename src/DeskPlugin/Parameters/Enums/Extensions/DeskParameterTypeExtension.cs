using System.ComponentModel;
using System.Linq;

namespace Parameters.Enums.Extensions
{
    /// <summary>
    /// Статический класс, расширяющий перечисление типов параметров
    /// <see cref="DeskParameterType"/>.
    /// </summary>
    public static class DeskParameterTypeExtension
    {
        #region Methods

        /// <summary>
        /// Статический метод для получения текстового описания параметра письменного
        /// стола.
        /// </summary>
        /// <param name="parameter"> Параметр, описание которого необходимо
        /// получить.</param>
        /// <returns> Текстовое описание параметра.</returns>
        public static string GetDescription(this DeskParameterType parameter)
        {
            var description = parameter.ToString();
            var field = parameter.GetType().GetField(parameter.ToString());

            if (field.GetCustomAttributes(typeof(DescriptionAttribute), false) 
                    is DescriptionAttribute[] attributes && attributes.Any())
            {
                description = attributes.First().Description;
            }

            return description;
        }

        #endregion
    }
}
