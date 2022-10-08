using System.Globalization;
using System.Windows.Controls;

namespace Desk.ValidationRules
{
    /// <summary>
    /// Правило проверки пользовательского ввода на пустоту или заполненность
    /// только пробелами.
    /// </summary>
    internal class NullOrWhiteSpaceValidationRule : ValidationRule
    {
        #region Methods

        /// <inheritdoc/>
        public override ValidationResult Validate(object value, 
            CultureInfo cultureInfo)
        {
            var stringValue = value as string;

            return string.IsNullOrWhiteSpace(stringValue)
                ? new ValidationResult(false, 
                    "Value may not be blank or white-spaces")
                : ValidationResult.ValidResult;
        }

        #endregion
    }
}
