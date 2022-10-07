using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Parameters.Enums.Extensions
{
    /// <summary>
    /// Класс, расширяющий перечисление типов параметров <see cref="DeskParameterType"/>.
    /// </summary>
    public static class DeskParameterTypeExtension
    {
        #region Methods

        /// <summary>
        /// Метод для получения описания параметра письменного стола.
        /// </summary>
        /// <param name="parameter"> Параметр, описание которого необходимо получить.
        /// </param>
        /// <returns> Строковое описание параметра.</returns>
        public static string GetDescription(this DeskParameterType parameter)
        {
            var description = parameter.ToString();
            FieldInfo field = parameter.GetType().GetField(parameter.ToString());

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
