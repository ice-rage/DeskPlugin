using System.Globalization;
using System.Windows.Controls;

namespace Desk.ValidationRules
{
    /// <summary>
    /// Правило проверки пользовательского ввода на целые числа.
    /// </summary>
    public class IntegerValidationRule : ValidationRule
    {
        #region Methods

        /// <inheritdoc/>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var stringValue = value as string;

            return int.TryParse(stringValue, out int _) 
                ? new ValidationResult(true, null)
                : new ValidationResult(false, "Value must be an integer");
        }

        #endregion
    }
}
