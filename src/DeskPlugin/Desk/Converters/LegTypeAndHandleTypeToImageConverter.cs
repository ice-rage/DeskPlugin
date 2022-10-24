using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using Parameters.Enums;

namespace Desk.Converters
{
    /// <summary>
    /// Конвертер для преобразования выбранного типа ножек письменного стола
    /// в объект <see cref="BitmapImage"/>, который содержит путь к соответствующему
    /// изображению 3D-модели письменного стола.
    /// </summary>
    [ValueConversion(typeof(Enum[]), typeof(BitmapImage))]
    [MarkupExtensionReturnType(typeof(LegTypeAndHandleTypeToImageConverter))]
    internal class LegTypeAndHandleTypeToImageConverter : MarkupExtension, 
        IMultiValueConverter
    {
        #region Fields

        /// <summary>
        /// Текущий рабочий каталог программы.
        /// </summary>
        private readonly string _outputDirectory = Directory.GetCurrentDirectory();

        #endregion

        #region Methods

        #region IMultiValueConverter Implementation

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public object Convert(
            object[] values,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            LegType legType;
            DrawerHandleType handleType;

            switch (values[0])
            {
                case LegType _ when values[1] is DrawerHandleType:
                {
                    legType = (LegType)values[0];
                    handleType = (DrawerHandleType)values[1];
                    break;
                }
                case DrawerHandleType _ when values[1] is LegType:
                {
                    legType = (LegType)values[1];
                    handleType = (DrawerHandleType)values[0];
                    break;
                }
                default:
                {
                    return null;
                }
            }

            switch (legType)
            {
                case LegType.Round:
                {
                    return ConvertToImage(nameof(LegType.Round), handleType);
                }
                case LegType.Square:
                {
                    return ConvertToImage(nameof(LegType.Square), handleType);
                }
                default:
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public object[] ConvertBack(
            object value,
            Type[] targetTypes,
            object parameter,
            CultureInfo culture) => throw new NotSupportedException(
            "Back conversion is not supported");

        #endregion

        #region MarkupExtension Overriding

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override object ProvideValue(IServiceProvider serviceProvider) => this;
        
        #endregion

        #endregion

        /// <summary>
        /// Метод, преобразующий значения перечислений <see cref="LegType"/> и
        /// <see cref="DrawerHandleType"/> в объект <see cref="BitmapImage"/>,
        /// содержащий относительный путь к изображению 3D-модели письменного стола.
        /// </summary>
        /// <param name="legType"> Тип ножек стола.</param>
        /// <param name="handleType"> Тип ручек ящиков для канцелярии.</param>
        /// <returns></returns>
        private BitmapImage ConvertToImage(string legType, DrawerHandleType handleType)
        {
            if (Enum.IsDefined(typeof(LegType), legType) && 
                Enum.IsDefined(typeof(DrawerHandleType), handleType))
            {
                return new BitmapImage(new Uri(Path.Combine(_outputDirectory,
                    $@"Resources\{legType}Legs{handleType}Handles.png")));
            }

            return null;
        }
    }
}