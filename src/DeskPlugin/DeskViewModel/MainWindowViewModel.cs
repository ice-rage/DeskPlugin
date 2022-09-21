using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeskParameters;

namespace DeskViewModel
{
    /// <summary>
    /// Модель представления главного окна плагина.
    /// </summary>
    public class MainWindowViewModel : ObservableObject
    {
        #region PrivateFields

        /// <summary>
        /// Переменная для хранения значения, показывающего, корректны ли введенные данные.
        /// </summary>
        private bool _isDataValid = true;

        #endregion

        #region PublicProperties

        /// <summary>
        /// Список параметров письменного стола.
        /// </summary>
        public Parameters Parameters { get; } = new Parameters();

        /// <summary>
        /// Проверяет, корректны ли введенные данные.
        /// </summary>
        public bool IsDataValid
        {
            get => _isDataValid;

            set => SetProperty(ref _isDataValid, value);
        }

        #region Commands

        /// <summary>
        /// Команда для построения 3D-модели письменного стола в AutoCAD.
        /// </summary>
        public RelayCommand<Parameters> BuildModelCommand { get; }

        /// <summary>
        /// Команда, задающая минимальные значения параметров.
        /// </summary>
        public RelayCommand SetMinimumParametersCommand { get; }

        /// <summary>
        /// Команда, задающая средние значения параметров.
        /// </summary>
        public RelayCommand SetAverageParametersCommand { get; }

        /// <summary>
        /// Команда, задающая максимальные значения параметров.
        /// </summary>
        public RelayCommand SetMaximumParametersCommand { get; }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Создает экземпляр класса <see cref="MainWindowViewModel"/>.
        /// </summary>
        public MainWindowViewModel()
        {
            // Подписываемся на событие изменения корректности данных, вводимых пользователем,
            // чтобы иметь возможность отслеживать это событие и определять, корректен ли ввод
            // в целом (необходимо для отключения/включения кнопки построения 3D-модели).
            //
            Parameters.DataValidChanged += OnDataValidChanged;

            Parameters.ParameterGroups
                .ForEach(group => group.Parameters
                    .ToList()
                    .ForEach(parameter => parameter.DataValidChanged += OnDataValidChanged));
        }

        #endregion

        #region Methods

        #region EventHandlers

        /// <summary>
        /// Обработчик события изменения корректности введенных данных.
        /// </summary>
        /// <param name="sender"> Отправитель события.</param>
        /// <param name="e"> Аргументы события.</param>
        private void OnDataValidChanged(object sender, EventArgs e) => IsDataValid = !Parameters
            .ParameterGroups
            .Any(group => group.Parameters
                .Any(parameter => parameter.HasErrors));

        #endregion

        #endregion
    }
}
