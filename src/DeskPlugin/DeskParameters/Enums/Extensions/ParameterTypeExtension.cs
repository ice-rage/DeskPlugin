﻿using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace DeskParameters.Enums.Extensions
{
    /// <summary>
    /// Класс, расширяющий перечисление типов параметров <see cref="ParameterType"/>.
    /// </summary>
    public static class ParameterTypeExtension
    {
        /// <summary>
        /// Метод для получения описания параметра письменного стола.
        /// </summary>
        /// <param name="parameter"> Параметр, описание которого необходимо получить.
        /// </param>
        /// <returns> Строка, содержащая описание параметра.</returns>
        public static string GetDescription(this ParameterType parameter)
        {
            var description = parameter.ToString();
            FieldInfo fieldInfo = parameter.GetType().GetField(parameter.ToString());

            if (fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) 
                is DescriptionAttribute[] attributes && attributes.Any())
            {
                description = attributes.First().Description;
            }

            return description;
        }
    }
}
