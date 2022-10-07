using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Parameters.Enums;
using Parameters.Enums.Extensions;

namespace Parameters
{
    /// <summary>
    /// Класс для хранения информации об отдельном параметре письменного стола.
    /// </summary>
    public class DeskParameter : ObservableObject, INotifyDataErrorInfo, ICloneable
    {
        #region Fields

        /// <summary>
        /// Текущее значение параметра.
        /// </summary>
        private int _value;

        /// <summary>
        /// Диапазон допустимых значений параметра.
        /// </summary>
        private string _acceptableRange;

        #endregion

        #region Properties

        /// <summary>
        /// Название параметра.
        /// </summary>
        public DeskParameterType Name { get; }

        /// <summary>
        /// Текстовое описание параметра.
        /// </summary>
        public string Description => Name.GetDescription();

        /// <summary>
        /// Минимальное значение параметра.
        /// </summary>
        public int Min { get; }

        /// <summary>
        /// Максимальное значение параметра.
        /// </summary>
        public int Max { get; }

        /// <summary>
        /// Текущее значение параметра.
        /// </summary>
        public int Value
        {
            get => _value;

            set
            {
                if (!SetProperty(ref _value, value))
                {
                    return;
                }

                ValueChanged?.Invoke(this, EventArgs.Empty);
                DataValidChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Диапазон допустимых значений параметра.
        /// </summary>
        public string AcceptableRange
        {
            get => _acceptableRange;
            private set => SetProperty(ref _acceptableRange, value);
        }

        /// <inheritdoc/>
        public bool HasErrors => GetErrors(null).OfType<string>().Any();

        #endregion

        #region Events

        /// <summary>
        /// Событие изменения текущего значения параметра.
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// Событие, уведомляющее об изменении корректности введенных данных.
        /// </summary>
        public event EventHandler DataValidChanged;

        /// <inheritdoc/>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Создает экземпляр <see cref="DeskParameter"/>.
        /// </summary>
        /// <param name="name"> Имя параметра.</param>
        /// <param name="min"> Минимальное значение параметра.</param>
        /// <param name="max"> Максимальное значение параметра.</param>
        /// <param name="value"> Текущее значение параметра.</param>
        public DeskParameter(DeskParameterType name, int min, int max, int value)
        {
            Name = name;
            Min = min;
            Max = max;
            Value = value;

            if (Min >= Max)
            {
                AcceptableRange = "Error";

                return;
            }

            AcceptableRange = Name == DeskParameterType.DrawerNumber 
                ? $"({Min}-{Max} pcs)" 
                : $"({Min}-{Max} mm)";
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (!(obj is DeskParameter parameter))
            {
                return false;
            }

            return parameter.Name == Name && 
                   parameter.Min == Min && 
                   parameter.Max == Max &&
                   parameter.Value == Value && 
                   parameter.AcceptableRange == AcceptableRange && 
                   parameter.HasErrors == HasErrors;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;

	            // TODO: Магическое число
                hash = hash * 23 + Name.GetHashCode();
                hash = hash * 23 + Description.GetHashCode();
                hash = hash * 23 + Min.GetHashCode();
                hash = hash * 23 + Max.GetHashCode();
                hash = hash * 23 + Value.GetHashCode();
                hash = hash * 23 + AcceptableRange.GetHashCode();
                hash = hash * 23 + HasErrors.GetHashCode();

                return hash;
            }
        }

        /// <inheritdoc/>
        public object Clone() => MemberwiseClone();

        #region Validators

        /// <inheritdoc/>
        public IEnumerable GetErrors([CallerMemberName] string propertyName = null)
        {
            if (!string.IsNullOrEmpty(propertyName) && propertyName != nameof(Value))
            {
                yield break;
            }

            if (Value < Min)
            {
                yield return $"Parameter \"{Description}\" must be greater than or equal to {Min}";
            }

            if (Value > Max)
            {
                yield return $"Parameter \"{Description}\" must be less than or equal to {Max}";
            }
        }

        #endregion

        #endregion
    }
}
