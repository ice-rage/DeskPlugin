using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using DeskParameters.Enums;
using DeskParameters.Enums.Extensions;

namespace DeskParameters
{
    /// <summary>
    /// Класс для хранения параметров письменного стола.
    /// </summary>
    public class Parameters : ObservableObject
    {
        #region Fields

        /// <summary>
        /// Тип ножек письменного стола.
        /// </summary>
        private LegType _legType;

        #endregion

        #region Properties

        #region Static

        /// <summary>
        /// Расстояние от края столешницы до некоторого объекта (ножек стола, углов ящиков и т.п.).
        /// </summary>
        public static int DistanceFromWorktopCorner => 20;

        /// <summary>
        /// Разница в ширине между столешницей и ящиком для канцелярии.
        /// </summary>
        public static int WorktopDrawerWidthDifference => 40;

        /// <summary>
        /// Разница в длине между внешним и внутренним пространством ящика для канцелярии.
        /// </summary>
        public static int OuterInnerDrawerLengthDifference => 40;

        /// <summary>
        /// Разница в ширине между внешним и внутренним пространством ящика для канцелярии.
        /// </summary>
        public static int OuterInnerDrawerWidthDifference => 40;

        /// <summary>
        /// Разница в высоте между внешним и внутренним пространством ящика для канцелярии.
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
        /// Разница в длине между внешним и внутренним пространством ручки ящика для канцелярии.
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
        public Dictionary<ParameterGroupType, ObservableCollection<Parameter>> ParametersByGroup
        { get; } = new Dictionary<ParameterGroupType, ObservableCollection<Parameter>>();

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
        /// Событие, уведомляющее об изменении корректности данных, введенных пользователем.
        /// </summary>
        public event EventHandler DataValidChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Создает экземпляр класса <see cref="Parameters"/>.
        /// </summary>
        public Parameters()
        {
            LegType = LegType.Round;
            ParametersByGroup.Clear();

            ParametersByGroup.Add(ParameterGroupType.Worktop, new ObservableCollection<Parameter>
            {
                new Parameter(ParameterType.WorktopLength, 800, 1200, 1000),
                new Parameter(ParameterType.WorktopWidth, 500, 750, 625),
                new Parameter(ParameterType.WorktopHeight, 30, 40, 35)
            });

            ParametersByGroup.Add(ParameterGroupType.Legs, new ObservableCollection<Parameter>
            {
                LegType == LegType.Round
                    ? new Parameter(ParameterType.LegBaseDiameter, 50, 70, 60)
                    : new Parameter(ParameterType.LegBaseLength, 50, 70, 60),
                new Parameter(ParameterType.LegHeight, 690, 740, 715)
            });

            ParametersByGroup.Add(ParameterGroupType.Drawers, new ObservableCollection<Parameter>
            {
                new Parameter(ParameterType.DrawerNumber, 3, 5, 4),
                new Parameter(ParameterType.DrawerLength, 250, 333, 291)
            });

            // Подписываемся на событие изменения текущего значения длины столешницы, т.к. от этого
            // параметра зависит несколько других параметров письменного стола (ширина столешницы
            // и длина ящиков для канцелярии).
            this[ParameterGroupType.Worktop, ParameterType.WorktopLength].ValueChanged +=
                OnWorktopLengthChanged;

            // Для уведомления модели представления главного окна об изменении корректности каждого
            // параметра используем делегат-"посредник".
            ParametersByGroup.Values
                .ToList()
                .ForEach(parameters => parameters
                    .ToList()
                    .ForEach(parameter => parameter.DataValidChanged += DataValidChanged));

            string value = Enum.GetNames(typeof(ParameterGroupType))[0];
        }

        #endregion

        #region Methods

        /// <summary>
        /// Обновляет параметр, хранящий размер основания ножек письменного стола, в зависимости от
        /// их типа.
        /// </summary>
        /// <param name="legType"> Тип ножек письменного стола.</param>
        private void UpdateLegBaseParameter(LegType legType)
        {
            ParameterType previousLegBaseType = LegType.GetLegBaseType();
            SetProperty(ref _legType, legType);
            ParameterType updatedLegBaseType = LegType.GetLegBaseType();

            Parameter previousParameter = this[ParameterGroupType.Legs, previousLegBaseType];

            this[ParameterGroupType.Legs, previousLegBaseType] = new Parameter(updatedLegBaseType,
                previousParameter.Min, previousParameter.Max, previousParameter.Value);
            this[ParameterGroupType.Legs, updatedLegBaseType].DataValidChanged += DataValidChanged;
        }

        /// <summary>
        /// Обработчик события изменения значения длины столешницы.
        /// <para>Для зависимых параметров устанавливаются новые ограничения.</para>
        /// </summary>
        /// <param name="sender"> Отправитель события.</param>
        /// <param name="e"> Аргументы события.</param>
        private void OnWorktopLengthChanged(object sender, EventArgs e)
        {
            int worktopLength = this[ParameterGroupType.Worktop, ParameterType.WorktopLength].Value;

            Parameter worktopWidthParameter = this[ParameterGroupType.Worktop, 
                ParameterType.WorktopWidth];
            this[ParameterGroupType.Worktop, ParameterType.WorktopWidth] =
                new Parameter(worktopWidthParameter.Name, worktopLength / 2, 
                    worktopWidthParameter.Max, worktopWidthParameter.Value);
            this[ParameterGroupType.Worktop, ParameterType.WorktopWidth].DataValidChanged +=
                DataValidChanged;

            Parameter drawerLengthParameter = this[ParameterGroupType.Drawers, 
                ParameterType.DrawerLength];
            this[ParameterGroupType.Drawers, ParameterType.DrawerLength] =
                new Parameter(drawerLengthParameter.Name, drawerLengthParameter.Min,
                    worktopLength / 3, drawerLengthParameter.Value);
            this[ParameterGroupType.Drawers, ParameterType.DrawerLength].DataValidChanged +=
                DataValidChanged;
        }

        #endregion

        #region Indexers

        /// <summary>
        /// Индексатор, позволяющий получить группу параметров и соответствующий список параметров
        /// по указанной группе параметров./>
        /// </summary>
        /// <param name="index"> Группа параметров.</param>
        /// <returns> Пара "ключ-значение", образованная группой параметров и соответствующим
        /// списком параметров.</returns>
        public KeyValuePair<ParameterGroupType, ObservableCollection<Parameter>>
            this[ParameterGroupType index] => new KeyValuePair<ParameterGroupType,
            ObservableCollection<Parameter>>(index, ParametersByGroup[index]);

        /// <summary>
        /// Индексатор, позволяющий получить группу параметров письменного стола по ее наименованию.
        /// </summary>
        /// <param name="firstIndex"> Группа параметров.</param>
        /// <param name="secondIndex"> Тип определенного параметра.</param>
        /// <returns> Объект <see cref="Parameter"/>, который представляет найденную группу
        /// параметров.</returns>
        public Parameter this[ParameterGroupType firstIndex, ParameterType secondIndex]
        {
            get => ParametersByGroup[firstIndex].First(parameter => parameter.Name == secondIndex);

            set => ParametersByGroup[firstIndex][ParametersByGroup[firstIndex]
                .IndexOf(ParametersByGroup[firstIndex]
                    .First(parameter => parameter.Name == secondIndex))] = value;
        }

        #endregion
    }
}
