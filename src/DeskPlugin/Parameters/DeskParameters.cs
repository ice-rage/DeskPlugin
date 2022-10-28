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
        /// Закрытый словарь только для чтения, в котором ключом является группа
        /// параметров письменного стола, а значением - соответствующая коллекция
        /// параметров, входящих в данную группу. 
        /// </summary>
        private ReadOnlyDictionary<DeskParameterGroupType,
            ObservableCollection<DeskParameter>> _parametersByGroupReadOnly;

        /// <summary>
        /// Тип ножек письменного стола.
        /// </summary>
        private LegType _legType;

        /// <summary>
        /// Тип ручек ящиков для канцелярии.
        /// </summary>
        private DrawerHandleType _handleType;

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

        #endregion

        /// <summary>
        /// Группа параметров и соответствующий ей список параметров.
        /// </summary>
        public Dictionary<DeskParameterGroupType, 
                ObservableCollection<DeskParameter>>
            ParametersByGroup { get; private set; }

        /// <summary>
        /// Тип ножек письменного стола.
        /// </summary>
        public LegType LegType
        {
            get => _legType;

            set
            {
                var previousLegBaseType = _legType.GetLegBaseType();
                SetProperty(ref _legType, value);
                var updatedLegBaseType = _legType.GetLegBaseType();

                UpdateGeneralParameter(DeskParameterGroupType.Legs, 
                    previousLegBaseType, updatedLegBaseType);
            }
        }

        /// <summary>
        /// Тип ручек ящиков для канцелярии.
        /// </summary>
        public DrawerHandleType HandleType
        {
            get => _handleType;

            set
            {
                var previousHandleDimensionType = HandleType
                    .GetHandleMutableParameterType();
                SetProperty(ref _handleType, value);
                var updatedHandleDimensionType = HandleType
                    .GetHandleMutableParameterType();

                UpdateGeneralParameter(DeskParameterGroupType.Drawers,
                    previousHandleDimensionType, updatedHandleDimensionType);
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
            HandleType = DrawerHandleType.Grip;

            SetDefaultParameters();
            CreateParametersByGroupDictionary();

            // Подписываемся на событие изменения текущего значения длины
            // столешницы, т.к. от этого параметра зависит несколько других
            // параметров письменного стола (ширина столешницы и длина ящиков для
            // канцелярии).
            this[DeskParameterGroupType.Worktop, DeskParameterType.WorktopLength]
                .ValueChanged += OnWorktopLengthChanged;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Метод, устанавливающий параметры письменного стола по умолчанию.
        /// </summary>
        private void SetDefaultParameters()
        {
            _parametersByGroupReadOnly = 
                new ReadOnlyDictionary<DeskParameterGroupType, 
                    ObservableCollection<DeskParameter>>(
                    new Dictionary<DeskParameterGroupType, 
                        ObservableCollection<DeskParameter>>
            {
                {
                    DeskParameterGroupType.Worktop,
                    new ObservableCollection<DeskParameter>
                    {
                        new DeskParameter(DeskParameterType.WorktopLength,
                            800, 1200, 1000),
                        new DeskParameter(DeskParameterType.WorktopWidth,
                            500, 750, 625),
                        new DeskParameter(DeskParameterType.WorktopHeight,
                            30, 40, 35)
                    }
                },
                {
                    DeskParameterGroupType.Legs,
                    new ObservableCollection<DeskParameter>
                    {
                        new DeskParameter(DeskParameterType.LegBaseDiameter,
                            50, 70, 60),
                        new DeskParameter(DeskParameterType.LegBaseLength,
                            50, 70, 60),
                        new DeskParameter(DeskParameterType.LegHeight, 690,
                            740, 715)
                    }
                },
                {
                    DeskParameterGroupType.Drawers,
                    new ObservableCollection<DeskParameter>
                    {
                        new DeskParameter(
                            DeskParameterType.DrawerRailingHandleFastenerDistance,
                        70, 90, 80),
                        new DeskParameter(
                            DeskParameterType.DrawerGripHandleFastenerDistance,
                        80, 100, 90),
                        new DeskParameter(
                            DeskParameterType.DrawerKnobHandleBaseDiameter,
                        50, 70, 60),
                        new DeskParameter(DeskParameterType.DrawerNumber, 3,
                        5, 4),
                        new DeskParameter(DeskParameterType.DrawerLength, 250,
                        333, 291)
                    }
                }
            });
        }

        /// <summary>
        /// Метод для создания открытого словаря параметров письменного стола
        /// <see cref="ParametersByGroup"/> на основе закрытого словаря
        /// <see cref="_parametersByGroupReadOnly"/>.
        /// </summary>
        private void CreateParametersByGroupDictionary()
        {
            var legBaseDiameter = DeskParameterType.LegBaseDiameter;
            var legBaseLength = DeskParameterType.LegBaseLength;
            var railingHandleFastenerDistance =
                DeskParameterType.DrawerRailingHandleFastenerDistance;
            var gripHandleFastenerDistance =
                DeskParameterType.DrawerGripHandleFastenerDistance;
            var knobHandleBaseDiameter =
                DeskParameterType.DrawerKnobHandleBaseDiameter;

            var parametersByGroupDeepCopy = new Dictionary<DeskParameterGroupType,
                ObservableCollection<DeskParameter>>();

            foreach (var parametersByGroup 
                in _parametersByGroupReadOnly)
            {
                parametersByGroupDeepCopy.Add(parametersByGroup.Key, 
                    new ObservableCollection<DeskParameter>());

                foreach (var parameter in parametersByGroup.Value)
                {
                    var parameterType = parameter.Name;

                    // Определяем, для какого типа ручек ящиков необходимо добавить
                    // параметр.
                    if (parameterType != legBaseDiameter && 
                        parameterType != legBaseLength &&
                        parameterType != railingHandleFastenerDistance &&
                        parameterType != gripHandleFastenerDistance &&
                        parameterType != knobHandleBaseDiameter ||
                        parameterType == legBaseDiameter && LegType == LegType.Round ||
                        parameterType == legBaseLength && LegType == LegType.Square ||
                        parameterType == railingHandleFastenerDistance && 
                        HandleType == DrawerHandleType.Railing ||
                        parameterType == gripHandleFastenerDistance && 
                        HandleType == DrawerHandleType.Grip ||
                        parameterType == knobHandleBaseDiameter && 
                        HandleType == DrawerHandleType.Knob)
                    {
                        parametersByGroupDeepCopy[parametersByGroup.Key]
                            .Add((DeskParameter)parameter.Clone());
                    }
                }
            }

            ParametersByGroup = new Dictionary<DeskParameterGroupType, 
                ObservableCollection<DeskParameter>>(parametersByGroupDeepCopy);
        }

        /// <summary>
        /// Метод, обновляющий "общие" (связанные с одним и тем же элементом ввода
        /// данных) параметры письменного стола.
        /// </summary>
        /// <param name="parameterGroup"> Группа, к которой относится обновляемый
        /// параметр.</param>
        /// <param name="previousParameterType"> Тип предыдущего параметра.</param>
        /// <param name="updatedParameterType"> Тип обновленного параметра.</param>
        private void UpdateGeneralParameter(
            DeskParameterGroupType parameterGroup, 
            DeskParameterType previousParameterType, 
            DeskParameterType updatedParameterType)
        {
            if (_parametersByGroupReadOnly == null || ParametersByGroup == null)
            {
                return;
            }

            var previousParameter = this[parameterGroup, previousParameterType];
            var updatedParam = _parametersByGroupReadOnly[parameterGroup]
                .First(x => x.Name == updatedParameterType);

            this[parameterGroup, previousParameterType] = updatedParam;
            this[parameterGroup, updatedParameterType].DataValidChanged += 
                DataValidChanged;
            this[parameterGroup, updatedParameterType].Value = 
                previousParameter.Value;
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

            set => ParametersByGroup[firstIndex]
                [ParametersByGroup[firstIndex]
                    .IndexOf(ParametersByGroup[firstIndex]
                    .First(parameter => parameter.Name == secondIndex))] = value;
        }

        #endregion
    }
}
