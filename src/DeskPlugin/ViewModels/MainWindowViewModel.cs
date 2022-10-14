using System;
using System.Linq;
using Builder;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Parameters;
using Services.Interfaces;

namespace ViewModels
{
    /// <summary>
    /// Модель представления главного окна плагина.
    /// </summary>
    public class MainWindowViewModel : ObservableObject
    {
        #region Fields

        /// <summary>
        /// Переменная для хранения значения, показывающего, корректны ли введенные
        /// данные.
        /// </summary>
        private bool _isDataValid = true;

        #endregion

        #region Properties

        /// <summary>
        /// Список параметров письменного стола.
        /// </summary>
        public DeskParameters Parameters { get; } = new DeskParameters();

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
        public RelayCommand<DeskParameters> BuildModelCommand { get; }

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
        public MainWindowViewModel(ICadWrapper wrapper)
        {
            var builder = new DeskBuilder(wrapper);
            BuildModelCommand = new RelayCommand<DeskParameters>(builder.BuildDesk);

            SetMinimumParametersCommand = new RelayCommand(() =>
                SetDefaultParameters(parameter => parameter.Value = parameter.Min));
            SetAverageParametersCommand = new RelayCommand(() =>
                SetDefaultParameters(parameter => parameter.Value = 
                    (parameter.Min + parameter.Max) / 2));
            SetMaximumParametersCommand = new RelayCommand(() =>
                SetDefaultParameters(parameter => parameter.Value = parameter.Max));

            // Подписываемся на событие изменения корректности данных, вводимых
            // пользователем, чтобы иметь возможность отслеживать это событие
            // и определять, корректен ли ввод в целом (необходимо для отключения/
            // включения кнопки построения 3D-модели).
            //
            Parameters.DataValidChanged += OnDataValidChanged;

            Parameters.ParametersByGroup.Values
                .ToList()
                .ForEach(parameters => parameters
                    .ToList()
                    .ForEach(parameter => parameter.DataValidChanged +=
                        OnDataValidChanged));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Метод, устанавливающий значения по умолчанию (минимальные, средние или
        /// максимальные) для параметров письменного стола.
        /// </summary>
        /// <param name="action"> Делегат, используемый для передачи
        /// соответствующего метода установки значений по умолчанию.</param>
        private void SetDefaultParameters(Action<DeskParameter> action)
        {
	        // TODO: Почему не var?
			foreach (var parameters in Parameters.ParametersByGroup.Values)
            {
                for (var i = 0; i < parameters.Count; i++)
                {
                    action?.Invoke(parameters[i]);
                    parameters[i].DataValidChanged += OnDataValidChanged;
                }
            }

            OnDataValidChanged(this, EventArgs.Empty);
        }

        #region Event Handlers

        /// <summary>
        /// Обработчик события изменения корректности введенных данных.
        /// </summary>
        /// <param name="sender"> Отправитель события.</param>
        /// <param name="e"> Аргументы события.</param>
        private void OnDataValidChanged(object sender, EventArgs e) => IsDataValid = 
            !Parameters.ParametersByGroup.Values
                .Any(parameters => parameters
                    .Any(parameter => parameter.HasErrors));

        #endregion

        #endregion
    }
}
