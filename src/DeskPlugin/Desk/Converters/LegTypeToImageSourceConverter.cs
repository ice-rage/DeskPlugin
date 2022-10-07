using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Parameters.Enums;

namespace Desk.Converters
{
    /// <summary>
    /// Конвертер для преобразования выбранного типа ножек письменного стола в соответствующее
    /// изображение его 3D-модели.
    /// </summary>
    [ValueConversion(typeof(LegType), typeof(string))]
    [MarkupExtensionReturnType(typeof(LegTypeToImageSourceConverter))]
    internal class LegTypeToImageSourceConverter : IValueConverter
    {
        #region Methods

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            switch (value)
            {
                case LegType.Round:
                {
						// TODO: Лучше сделать относительный путь.
						// Копировать всегда изображения при сборке.
						// Получиться примерно такой путь Resources/DeskWithSquareLegs.png
						return "../../Resources/DeskWithRoundLegs.png";
                }
                case LegType.Square:
                {
	                // TODO: Лучше сделать относительный путь.
	                // Копировать всегда изображения при сборке.
	                // Получиться примерно такой путь Resources/DeskWithSquareLegs.png
                    return "../../Resources/DeskWithSquareLegs.png";
                }
                default:
                {
                    return null;
                }
            }
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, 
            CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        #endregion
    }
}