using System;
using CommunityToolkit.Mvvm.ComponentModel;
using DeskParameters.Enums;
using DeskParameters.Enums.Extensions;

namespace DeskParameters
{
    /// <summary>
    /// Класс <see cref="Parameter"/> хранит информацию о параметре письменного стола.
    /// </summary>
    public class Parameter : ObservableObject, ICloneable
    {
        #region PrivateFields

        /// <summary>
        /// Текущее значение параметра.
        /// </summary>
        private int _value;

        /// <summary>
        /// Диапазон допустимых значений параметра.
        /// </summary>
        private string _acceptableRange;

        /// <summary>
        /// Хранит значение, показывающее, корректны ли введенные данные.
        /// </summary>
        private bool _isDataValid = true;

        #endregion

        #region PublicProperties

        /// <summary>
        /// Название параметра.
        /// </summary>
        public ParameterType Name { get; }

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

        /// <summary>
        /// Проверяет, корректны ли введенные данные.
        /// </summary>
        public bool IsDataValid
        {
            get => _isDataValid;

            set
            {
                if (!SetProperty(ref _isDataValid, value))
                {
                    return;
                }

                DataValidChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Событие, уведомляющее об изменении корректности введенных данных.
        /// </summary>
        public event EventHandler DataValidChanged;

        /// <summary>
        /// Событие изменения текущего значения параметра.
        /// </summary>
        public event EventHandler ValueChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Создает экземпляр класса <see cref="Parameter"/>.
        /// </summary>
        /// <param name="name"> Имя параметра.</param>
        /// <param name="min"> Минимальное значение параметра.</param>
        /// <param name="max"> Максимальное значение параметра.</param>
        /// <param name="value"> Текущее значение параметра.</param>
        public Parameter(ParameterType name, int min, int max, int value)
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

            AcceptableRange = Name == ParameterType.DrawerNumber 
                ? $"({Min}-{Max} pcs)" 
                : $"({Min}-{Max} mm)";
        }

        #region Methods

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (!(obj is Parameter parameter))
            {
                return false;
            }

            return parameter.Name == Name && parameter.Min == Min && parameter.Max == Max &&
                   parameter.Value == Value;
        }

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(Name, Description, Min, Max, Value,
            AcceptableRange, IsDataValid);

        /// <inheritdoc/>
        public object Clone() => MemberwiseClone();

        #endregion

        #endregion
    }
}
