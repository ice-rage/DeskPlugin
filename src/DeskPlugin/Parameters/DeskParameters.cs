using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Parameters.Enums;
using Parameters.Enums.Extensions;

namespace Parameters
{
    /// <summary>
    /// Класс для хранения параметров письменного стола.
    /// </summary>
    public class DeskParameters : ObservableObject
    {
        #region Fields

        /// <summary>
        /// Тип ножек письменного стола.
        /// </summary>
        private LegType _legType;

        #endregion

        #region Properties

        #region Static Properties

        /// <summary>
        /// Расстояние от края столешницы до ближайшей ножки стола.
        /// </summary>
        public static int DistanceFromWorktopCornerToLeg => 20;

        /// <summary>
        /// Разница в длине между внешним и внутренним пространством ящика для
        /// канцелярии.
        /// </summary>
        public static int OuterInnerDrawerLengthDifference => 40;

        /// <summary>
        /// Разница в ширине между внешним и внутренним пространством ящика для
        /// канцелярии.
        /// </summary>
        public static int OuterInnerDrawerWidthDifference => 40;

        /// <summary>
        /// Разница в высоте между внешним и внутренним пространством ящика для
        /// канцелярии.
        /// </summary>
        public static int OuterInnerDrawerHeightDifference => 20;

        /// <summary>
        /// Разница в длине между ящиком для канцелярии и его дверцей.
        /// </summary>
        public static int DrawerDoorLengthDifference => 40;

        /// <summary>
        /// Ширина дверцы ящика для канцелярии.
        /// </summary>
        public static int DoorWidth => 20;

        /// <summary>
        /// Разница в высоте между ящиком для канцелярии и его дверцей.
        /// </summary>
        public static int DrawerDoorHeightDifference => 20;

        /// <summary>
        /// Высота ручки ящика для канцелярии.
        /// </summary>
        public static int HandleHeight => 20;

        /// <summary>
        /// Разница в длине между внешним и внутренним пространством ручки ящика
        /// для канцелярии.
        /// </summary>
        public static int OuterInnerHandleLengthDifference => 40;

        /// <summary>
        /// Ширина внешнего пространства ручки ящика для канцелярии.
        /// </summary>
        public static int OuterHandleWidth => 40;

        /// <summary>
        /// Ширина внутреннего пространства ручки ящика для канцелярии.
        /// </summary>
        public static int InnerHandleWidth => 20;

        #endregion

        /// <summary>
        /// Группа параметров и соответствующий ей список параметров.
        /// </summary>
        // TODO: Подумать надо, как сделать более читаемо.
        public Dictionary<DeskParameterGroupType,
            ObservableCollection<DeskParameter>>
            ParametersByGroup
        { get; } = new Dictionary<DeskParameterGroupType,
                ObservableCollection<DeskParameter>>();

        /// <summary>
        /// Тип ножек письменного стола.
        /// </summary>
        public LegType LegType
        {
            get => _legType;

            set
            {
                if (!ParametersByGroup.Any())
                {
                    SetProperty(ref _legType, value);

                    return;
                }

                UpdateLegBaseParameter(value);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Событие, уведомляющее об изменении корректности данных, введенных
        /// пользователем.
        /// </summary>
        public event EventHandler DataValidChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Создает экземпляр <see cref="DeskParameters"/>.
        /// </summary>
        public DeskParameters()
        {
            LegType = LegType.Round;
            ParametersByGroup.Clear();

            ParametersByGroup.Add(DeskParameterGroupType.Worktop,
                new ObservableCollection<DeskParameter>
                { 
                    // TODO: длина строк
                    new DeskParameter(DeskParameterType.WorktopLength, 800,
                        1200, 1000), 
                    // TODO: длина строк
                    new DeskParameter(DeskParameterType.WorktopWidth, 500,
                        750, 625), 
                    // TODO: длина строк и т.д. Должно быть примерно 80-85 длина стоки
                    new DeskParameter(DeskParameterType.WorktopHeight, 30,
                        40, 35)
                });

            ParametersByGroup.Add(DeskParameterGroupType.Legs,
                new ObservableCollection<DeskParameter>
                {
                    LegType == LegType.Round
                        ? new DeskParameter(DeskParameterType.LegBaseDiameter,
                            50, 70, 60)
                        : new DeskParameter(DeskParameterType.LegBaseLength,
                            50, 70, 60),
                    new DeskParameter(DeskParameterType.LegHeight, 690,
                        740, 715)
                });

            ParametersByGroup.Add(DeskParameterGroupType.Drawers,
                new ObservableCollection<DeskParameter>
                {
                    new DeskParameter(DeskParameterType.DrawerNumber, 3,
                        5, 4),
                    new DeskParameter(DeskParameterType.DrawerLength, 250,
                        333, 291)
                });

            // Подписываемся на событие изменения текущего значения длины
            // столешницы, т.к. от этого параметра зависит несколько других
            // параметров письменного стола (ширина столешницы и длина ящиков для
            // канцелярии).
            this[DeskParameterGroupType.Worktop, DeskParameterType.WorktopLength]
                    .ValueChanged += OnWorktopLengthChanged;

            // Для уведомления модели представления главного окна об изменении
            // корректности каждого параметра используем делегат-"посредник".
            ParametersByGroup.Values
                .ToList()
                .ForEach(parameters => parameters
                    .ToList()
                    .ForEach(parameter => parameter.DataValidChanged +=
                        DataValidChanged));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Обновляет параметр, хранящий размер основания ножек письменного стола,
        /// в зависимости от их типа.
        /// </summary>
        /// <param name="legType"> Тип ножек письменного стола.</param>
        private void UpdateLegBaseParameter(LegType legType)
        {
            // TODO: Почему не var? И так дальше по классу
            var previousLegBaseType = LegType.GetLegBaseType();
            SetProperty(ref _legType, legType);
            var updatedLegBaseType = LegType.GetLegBaseType();

            var previousDeskParameter = this[DeskParameterGroupType.Legs,
                previousLegBaseType];

            this[DeskParameterGroupType.Legs, previousLegBaseType] =
                new DeskParameter(updatedLegBaseType, previousDeskParameter.Min,
                    previousDeskParameter.Max, previousDeskParameter.Value);
            this[DeskParameterGroupType.Legs, updatedLegBaseType].DataValidChanged +=
                DataValidChanged;
        }

        /// <summary>
        /// Обработчик события изменения значения длины столешницы.
        /// <para> Для зависимых параметров устанавливаются новые диапазоны
        /// допустимых значений.</para>
        /// </summary>
        /// <param name="sender"> Отправитель события.</param>
        /// <param name="e"> Аргументы события.</param>
        private void OnWorktopLengthChanged(object sender, EventArgs e)
        {
            var worktopLength = this[DeskParameterGroupType.Worktop,
                DeskParameterType.WorktopLength].Value;

            var worktopWidthDeskParameter = this[DeskParameterGroupType.Worktop,
                DeskParameterType.WorktopWidth];
            this[DeskParameterGroupType.Worktop, DeskParameterType.WorktopWidth] =
                new DeskParameter(worktopWidthDeskParameter.Name,
                    worktopLength / 2, worktopWidthDeskParameter.Max,
                    worktopWidthDeskParameter.Value);
            this[DeskParameterGroupType.Worktop, DeskParameterType.WorktopWidth]
                    .DataValidChanged += DataValidChanged;

            var drawerLengthDeskParameter = this[DeskParameterGroupType.Drawers,
                DeskParameterType.DrawerLength];
            this[DeskParameterGroupType.Drawers, DeskParameterType.DrawerLength] =
                new DeskParameter(drawerLengthDeskParameter.Name,
                    drawerLengthDeskParameter.Min, worktopLength / 3,
                    drawerLengthDeskParameter.Value);
            this[DeskParameterGroupType.Drawers, DeskParameterType.DrawerLength]
                    .DataValidChanged += DataValidChanged;
        }

        #endregion

        #region Indexers

        /// <summary>
        /// Индексатор, позволяющий получить группу параметров и соответствующий
        /// список параметров по указанной группе параметров./>
        /// </summary>
        /// <param name="index"> Группа параметров.</param>
        /// <returns> Пара "ключ-значение", которая содержит группу параметров
        /// и соответствующий список параметров.</returns>
        public KeyValuePair<DeskParameterGroupType,
                ObservableCollection<DeskParameter>>
            this[DeskParameterGroupType index] =>
                new KeyValuePair<DeskParameterGroupType,
                    ObservableCollection<DeskParameter>>(index,
                        ParametersByGroup[index]);

        /// <summary>
        /// Индексатор, позволяющий получить параметр письменного стола по его
        /// группе и типу.
        /// </summary>
        /// <param name="firstIndex"> Группа параметров.</param>
        /// <param name="secondIndex"> Тип определенного параметра.</param>
        /// <returns> Найденный параметр.</returns>
        public DeskParameter this[DeskParameterGroupType firstIndex, 
            DeskParameterType secondIndex]
        {
            get => ParametersByGroup[firstIndex].First(parameter =>
                parameter.Name == secondIndex);

            set => ParametersByGroup[firstIndex][ParametersByGroup[firstIndex]
                .IndexOf(ParametersByGroup[firstIndex]
                    .First(parameter => parameter.Name == secondIndex))] = value;
        }

        #endregion
    }
}
