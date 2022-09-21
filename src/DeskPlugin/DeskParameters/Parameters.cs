using System;
using System.Collections.Generic;
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
        #region PrivateFields

        /// <summary>
        /// Тип ножек письменного стола.
        /// </summary>
        private LegType _legType;

        #endregion

        #region PublicProperties

        /// <summary>
        /// Список параметров письменного стола.
        /// </summary>
        public List<ParameterGroup> ParameterGroups { get; } = new List<ParameterGroup>();

        /// <summary>
        /// Тип ножек письменного стола.
        /// </summary>
        public LegType LegType
        {
            get => _legType;

            set
            {
                if (!ParameterGroups.Any())
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
            ParameterGroups.Clear();

            ParameterGroups.Add(new ParameterGroup(ParameterGroupType.Worktop,
                new List<Parameter>
                {
                    new Parameter(ParameterType.WorktopLength, 800, 1200, 1000),
                    new Parameter(ParameterType.WorktopWidth, 500, 750, 625),
                    new Parameter(ParameterType.WorktopHeight, 30, 40, 35)
                }));

            ParameterGroups.Add(new ParameterGroup(ParameterGroupType.Legs,
                new List<Parameter>
                {
                    LegType == LegType.Round
                        ? new Parameter(ParameterType.LegBaseDiameter, 50, 70, 60)
                        : new Parameter(ParameterType.LegBaseLength, 50, 70, 60),
                    new Parameter(ParameterType.LegHeight, 690, 740, 715)
                }));

            ParameterGroups.Add(new ParameterGroup(ParameterGroupType.Drawers,
                new List<Parameter>
                {
                    new Parameter(ParameterType.DrawerNumber, 3, 5, 4),
                    new Parameter(ParameterType.DrawerLength, 250, 333, 291)
                }));

            // Подписываемся на событие изменения текущего значения длины столешницы, т.к. от этого
            // параметра зависит несколько других параметров письменного стола (ширина столешницы
            // и длина ящиков для канцелярии).
            this[ParameterGroupType.Worktop][ParameterType.WorktopLength].ValueChanged +=
                OnWorktopLengthChanged;

            // Для уведомления модели представления главного окна об изменении корректности каждого
            // параметра используем делегат-"посредник".
            ParameterGroups
                .ForEach(group =>
                {
                    group.Parameters
                        .ToList()
                        .ForEach(parameter => parameter.DataValidChanged += DataValidChanged);
                });
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

            Parameter previousParameter = this[ParameterGroupType.Legs][previousLegBaseType];

            this[ParameterGroupType.Legs][previousLegBaseType] = new Parameter(updatedLegBaseType,
                previousParameter.Min, previousParameter.Max, previousParameter.Value);
            this[ParameterGroupType.Legs][updatedLegBaseType].DataValidChanged += DataValidChanged;
        }

        /// <summary>
        /// Обработчик события изменения значения длины столешницы.
        /// <para>Для зависимых параметров устанавливаются новые ограничения.</para>
        /// </summary>
        /// <param name="sender"> Отправитель события.</param>
        /// <param name="e"> Аргументы события.</param>
        private void OnWorktopLengthChanged(object sender, EventArgs e)
        {
            int worktopLength = this[ParameterGroupType.Worktop][ParameterType.WorktopLength].Value;

            Parameter worktopWidthParameter = this[ParameterGroupType.Worktop]
                [ParameterType.WorktopWidth];
            this[ParameterGroupType.Worktop][ParameterType.WorktopWidth] =
                new Parameter(worktopWidthParameter.Name, worktopLength / 2,
                    worktopWidthParameter.Max, worktopWidthParameter.Value);
            this[ParameterGroupType.Worktop][ParameterType.WorktopWidth].DataValidChanged +=
                DataValidChanged;

            Parameter drawerLengthParameter = this[ParameterGroupType.Drawers]
                [ParameterType.DrawerLength];
            this[ParameterGroupType.Drawers][ParameterType.DrawerLength] =
                new Parameter(drawerLengthParameter.Name, drawerLengthParameter.Min,
                    worktopLength / 3, drawerLengthParameter.Value);
            this[ParameterGroupType.Drawers][ParameterType.DrawerLength].DataValidChanged +=
                DataValidChanged;
        }

        #endregion

        #region Indexers

        /// <summary>
        /// Индексатор, позволяющий получить группу параметров письменного стола по ее наименованию.
        /// </summary>
        /// <param name="index"> Наименование группы параметров, определенный в перечислении
        /// <see cref="ParameterGroupType"/>.</param>
        /// <returns> Объект <see cref="ParameterGroup"/>, который представляет найденную группу
        /// параметров.</returns>
        public ParameterGroup this[ParameterGroupType index]
        {
            get => ParameterGroups.First(group => group.Name == index);

            set => ParameterGroups[ParameterGroups.IndexOf(ParameterGroups.First(group => 
                group.Name == index))] = value;
        }

        #endregion
    }
}
