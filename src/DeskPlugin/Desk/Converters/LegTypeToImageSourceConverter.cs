using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using DeskParameters.Enums;

namespace Desk.Converters
{
    /// <summary>
    /// Конвертер для преобразования выбранного типа ножек письменного стола в соответствующее
    /// изображение его 3D-модели.
    /// </summary>
    [ValueConversion(typeof(LegType), typeof(string))]
    [MarkupExtensionReturnType(typeof(LegTypeToImageSourceConverter))]
    public class LegTypeToImageSourceConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            switch (value)
            {
                case LegType.Round:
                {
                    return "../../Resources/DeskWithRoundLegs.png";
                }
                case LegType.Square:
                {
                    return "../../Resources/DeskWithSquareLegs.png";
                }
                default:
                {
                    return null;
                }
            }
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}