using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;
using Parameters;
using Parameters.Enums;
using Parameters.Enums.Extensions;
using Parameters.Static;
using Wrappers.Enums;
using Wrappers.Interfaces;

namespace Builder
{
    /// <summary>
    /// Класс для построения 3D-модели письменного стола.
    /// </summary>
    public class DeskBuilder
    {
        #region Fields

        /// <summary>
        /// Объект класса-оболочки для изолированного вызова методов API
        /// используемой САПР.
        /// </summary>
        private readonly ICadWrapper _wrapper;

        #endregion

        #region Constructors

        /// <summary>
        /// Создает экземпляр <see cref="DeskBuilder"/>.
        /// </summary>
        /// <param name="wrapper"> Объект класса-оболочки.</param>
        public DeskBuilder(ICadWrapper wrapper) => _wrapper = wrapper;

        #endregion

        #region Methods

        /// <summary>
        /// Метод для построения 3D-модели письменного стола.
        /// </summary>
        /// <param name="parameters"> Параметры, необходимые для построения
        /// 3D-модели.</param>
        public void BuildDesk(DeskParameters parameters)
        {
			var worktopLength = parameters[DeskParameterGroupType.Worktop,
                DeskParameterType.WorktopLength].Value;
            var worktopWidth = parameters[DeskParameterGroupType.Worktop,
                DeskParameterType.WorktopWidth].Value;
            var worktopHeight = parameters[DeskParameterGroupType.Worktop,
                DeskParameterType.WorktopHeight].Value;
            var legHeight = parameters[DeskParameterGroupType.Legs,
                DeskParameterType.LegHeight].Value;
            var drawerNumber = parameters[DeskParameterGroupType.Drawers,
                DeskParameterType.DrawerNumber].Value;
            var drawerLength = parameters[DeskParameterGroupType.Drawers,
                DeskParameterType.DrawerLength].Value;
            var drawerHeight = (double)legHeight / drawerNumber;

            var legType = parameters.LegType;
            var legBaseValue = parameters[DeskParameterGroupType.Legs,
                parameters.LegType.GetLegBaseType()].Value;

            var drawerHandleType = parameters.HandleType;
            var drawerHandleDimension =
                parameters[DeskParameterGroupType.Drawers, parameters.HandleType
                    .GetHandleMutableParameterType()].Value;

            BuildWorktop(worktopLength, worktopWidth, worktopHeight);

            BuildLegs(legType, legBaseValue, legHeight, worktopWidth);

            BuildDrawers(
                drawerNumber, 
                drawerLength, 
                drawerHeight, 
                drawerHandleType,
                drawerHandleDimension,
                worktopLength, 
                worktopWidth);

            _wrapper.Build();
        }

        /// <summary>
        /// Метод для построения столешницы.
        /// </summary>
        /// <param name="worktopLength"> Длина столешницы.</param>
        /// <param name="worktopWidth"> Ширина столешницы.</param>
        /// <param name="worktopHeight"> Высота столешницы.</param>
        private void BuildWorktop(int worktopLength, int worktopWidth, 
            int worktopHeight) => _wrapper.BuildCuboid(
            PlaneType.XoY, 
            0, 
            0, 
            worktopLength, 
            worktopWidth, 
            worktopHeight, 
            false, 
            true);

        /// <summary>
        /// Метод для построения ножек письменного стола.
        /// </summary>
        /// <param name="legType"> Тип ножек, описанный в перечислении
        /// <see cref="DeskParameterType"/>.</param>
        /// <param name="legBase"> Значение основания ножек.</param>
        /// <param name="legHeight"> Высота ножек.</param>
        /// <param name="worktopWidth"> Ширина столешницы, необходимая для расчета
        /// положения ножек относительно краев стола.</param>
        private void BuildLegs(LegType legType, int legBase, int legHeight, 
            int worktopWidth)
        {
            // Создаем словари, содержащие в качестве ключа порядковый номер ножки,
            // а в качестве значения - одну из координат ее основания.
            var x = new Dictionary<int, double>();
            var y = new Dictionary<int, double>();

            // Строим основания ножек.
            var legBases = legType == LegType.Round
                ? CreateRoundLegBases(legBase, worktopWidth, x, y)
                : CreateSquareLegBases(legBase, worktopWidth, x, y);

            // Выполняем операцию выдавливания для основания каждой ножки.
            foreach (var leg in legBases)
            {
                _wrapper.Extrude(leg, legHeight, isDirectionPositive: false);
            }
        }

        /// <summary>
        /// Метод для создания круглых оснований ножек письменного стола (если
        /// выбран тип ножек <see cref="LegType.Round"/>).
        /// </summary>
        /// <param name="baseDiameter"> Диаметр основания ножек.</param>
        /// <param name="worktopWidth"> Ширина столешницы, необходимая для расчета
        /// положения ножек относительно краев стола.</param>
        /// <param name="x"> Словарь, содержащий в качестве ключа порядковый номер
        /// ножки, а в качестве значения - x-координату центра окружности основания
        /// ножки.</param>
        /// <param name="y"> Словарь, содержащий в качестве ключа порядковый номер
        /// ножки, а в качестве значения - y-координату центра окружности основания
        /// ножки.</param>
        /// <returns> Перечисление круглых оснований ножек.</returns>
        private IEnumerable<object> CreateRoundLegBases(int baseDiameter, 
            int worktopWidth, IDictionary<int, double> x, IDictionary<int, double> y)
        {
            var roundLegBases = new List<object>();

            // В каждый словарь оснований добавляем координату центра окружности
            // основания соответствующей ножки.
            var baseCenter = DeskParameters.DistanceFromWorktopCornerToLeg + 
                baseDiameter / 2;
            x.Add(0, baseCenter);
            y.Add(0, baseCenter);

            x.Add(1, baseCenter);
            y.Add(1, worktopWidth - baseCenter);

            // Создаем окружности основания ножек и добавляем их в список.
            for (var i = 0; i < x.Count; i++)
            {
                var circle = _wrapper.CreateCircle(PlaneType.XoY, baseDiameter, 
                    x[i], y[i]);
                roundLegBases.Add(circle);
            }

            return roundLegBases;
        }

        /// <summary>
        /// Метод для создания квадратных оснований ножек письменного стола (если
        /// выбран тип ножек
        /// <see cref="LegType.Square"/>).
        /// </summary>
        /// <param name="baseLength"> Длина основания ножек.</param>
        /// <param name="worktopWidth"> Ширина столешницы, необходимая для расчета
        /// положения ножек относительно краев стола.</param>
        /// <param name="x"> Словарь, содержащий в качестве ключа порядковый номер
        /// ножки, а в качестве значения - x-координату левого нижнего угла квадрата
        /// основания ножки.</param>
        /// <param name="y"> Словарь, содержащий в качестве ключа порядковый номер
        /// ножки, а в качестве значения - y-координату левого нижнего угла квадрата
        /// основания ножки.</param>
        /// <returns> Перечисление квадратных оснований ножек.</returns>
        private IEnumerable<object> CreateSquareLegBases(int baseLength, 
            int worktopWidth, IDictionary<int, double> x, IDictionary<int, double> y)
        {
            var squareLegBases = new List<object>();

            // В каждый словарь оснований добавляем координату левого нижнего угла
            // квадрата основания соответствующей ножки.
            //
            x.Add(0, DeskParameters.DistanceFromWorktopCornerToLeg);
            y.Add(0, DeskParameters.DistanceFromWorktopCornerToLeg);

            x.Add(1, DeskParameters.DistanceFromWorktopCornerToLeg);
            y.Add(1, worktopWidth - DeskParameters.DistanceFromWorktopCornerToLeg - 
                     baseLength);

            // Создаем квадраты оснований ножек и добавляем их в список.
            for (var i = 0; i < x.Count; i++)
            {
                var square = _wrapper.CreateRectangle(PlaneType.XoY, x[i], y[i], 
                    baseLength, baseLength);
                squareLegBases.Add(square);
            }

            return squareLegBases;
        }

        /// <summary>
        /// Метод, который строит ящики для канцелярии письменного стола.
        /// </summary>
        /// <param name="drawerNumber"> Количество ящиков для канцелярии.</param>
        /// <param name="drawerLength"> Длина ящиков для канцелярии.</param>
        /// <param name="drawerHeight"> Высота ящиков для канцелярии.</param>
        /// <param name="handleType"> Тип ручек ящика для канцелярии.</param>
        /// <param name="handleDimension"> Числовой параметр ручки ящика
        /// для канцелярии (расстояние между крепежными элементами для ручек типа
        /// <see cref="DrawerHandleType.Grip"/> и <see cref="DrawerHandleType.Railing"/>
        /// либо диаметр основания для ручек типа <see cref="DrawerHandleType.Knob"/>.
        /// </param>
        /// <param name="worktopLength"> Длина столешницы, необходимая для расчета
        /// размеров и положения ящиков для канцелярии, их дверц и ручек.</param>
        /// <param name="worktopWidth"> Ширина столешницы, необходимая для расчета
        /// размеров и положения ящиков для канцелярии, их дверц и ручек.</param>
        private void BuildDrawers(
            int drawerNumber,
            int drawerLength,
            double drawerHeight,
            DrawerHandleType handleType,
            int handleDimension,
            int worktopLength,
            int worktopWidth)
        {
            // Вычисляем все размерности, необходимые для построения ящиков.
            //
            var drawerX = worktopLength - drawerLength;

            var holeX = drawerX + DeskParameters.OuterInnerDrawerLengthDifference / 2;
            var holeLength = drawerLength - 
                DeskParameters.OuterInnerDrawerLengthDifference;
            var holeWidth = worktopWidth - 
                DeskParameters.OuterInnerDrawerWidthDifference;
            var holeHeight = drawerHeight - 
                DeskParameters.OuterInnerDrawerHeightDifference;

            var doorX = drawerX + DeskParameters.DrawerDoorLengthDifference / 2;
            var doorLength = drawerLength - DeskParameters.DrawerDoorLengthDifference;
            var doorHeight = drawerHeight - DeskParameters.DrawerDoorHeightDifference;

            for (var i = 0; i < drawerNumber; i++)
            {
                var drawerZ = drawerHeight * i;

                // Строим ящик для канцелярии.
                _wrapper.BuildCuboid(
                    PlaneType.XoZ,
                    drawerX,
                    drawerZ,
                    drawerLength,
                    drawerHeight,
                    worktopWidth,
                    false,
                    true);

                // Вырезаем отверстие (внутреннее пространство) в ящике.
                _wrapper.BuildCuboid(
                    PlaneType.XoZ,
                    holeX,
                    drawerZ,
                    holeLength,
                    holeHeight,
                    holeWidth,
                    true,
                    true);

                // Строим дверцу ящика.
                _wrapper.BuildCuboid(
                    PlaneType.XoZ,
                    doorX,
                    drawerZ,
                    doorLength,
                    doorHeight,
                    DeskParameters.DoorWidth,
                    false,
                    true);

                // Строим ручку ящика в зависимости от выбранного типа.
                //
                switch (handleType)
                {
                    case DrawerHandleType.Railing:
                    {
                        BuildRailingDrawerHandle(
                            worktopLength,
                            drawerLength,
                            drawerHeight,
                            drawerZ, 
                            handleDimension);
                        break;
                    }
                    case DrawerHandleType.Knob:
                    {
                        BuildKnobDrawerHandle(
                            worktopLength,
                            drawerLength,
                            drawerHeight,
                            drawerZ,
                            handleDimension);
                        break;
                    }
                    case DrawerHandleType.Grip:
                    default:
                    {
                        BuildGripDrawerHandle(
                            worktopLength,
                            drawerLength,
                            drawerHeight,
                            drawerZ,
                            handleDimension);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Метод, выполняющий построение ручки-рейлинг ящика для канцелярии.
        /// </summary>
        /// <param name="worktopLength"> Длина столешницы.</param>
        /// <param name="drawerLength"> Длина ящика.</param>
        /// <param name="drawerHeight"> Высота ящика.</param>
        /// <param name="locationAlongZ"> Координата левого нижнего угла ящика
        /// по оси Z.</param>
        /// <param name="distanceBetweenFasteners"> Расстояние между крепежными
        /// элементами ручки-рейлинг.</param>
        private void BuildRailingDrawerHandle(
            int worktopLength,
            int drawerLength,
            double drawerHeight,
            double locationAlongZ,
            int distanceBetweenFasteners)
        {
            // Вычисляем все размерности, необходимые для построения ручки-рейлинг.
            //
            var railingZ = locationAlongZ + drawerHeight / 2 - 
                (double)DrawerHandleDimensions.HandleHeight / 2;

            var crossbeamLength = distanceBetweenFasteners + 
                DrawerHandleDimensions.RailingCrossbeamFastenersLengthDifference;
            var crossbeamX = worktopLength - drawerLength / 2;
            var crossbeamRotationAxis = new Vector3D(0, 0, 1);
            var crossbeamHalfLength = (double)crossbeamLength / 2;
            var crossbeamRotationPoint = new Point3D(crossbeamX, -crossbeamHalfLength, 
                railingZ);

            var leftFasteningLegX = crossbeamX - distanceBetweenFasteners / 2;
            var rightFasteningLegX = crossbeamX + distanceBetweenFasteners / 2;

            // Строим поперечную перекладину ручки, поворачивая ее таким образом, 
            // чтобы она приняла горизонтальное положение в пространстве.
            //
            _wrapper.BuildCylinder(
                PlaneType.XoZ,
                DrawerHandleDimensions.RailingCrossbeamDiameter,
                crossbeamX,
                railingZ,
                crossbeamRotationAxis,
                DrawerHandleDimensions.RailingCrossbeamRotationAngleInDegrees,
                crossbeamRotationPoint,
                crossbeamLength);

            // Строим левую крепежную ножку.
            _wrapper.BuildCylinder(
                PlaneType.XoZ,
                DrawerHandleDimensions.RailingFasteningLegDiameter,
                leftFasteningLegX,
                railingZ,
                crossbeamHalfLength,
                false);

            // Строим правую крепежную ножку.
            _wrapper.BuildCylinder(
                PlaneType.XoZ,
                DrawerHandleDimensions.RailingFasteningLegDiameter,
                rightFasteningLegX,
                railingZ,
                crossbeamHalfLength,
                false);
        }

        /// <summary>
        /// Метод, выполняющий построение ручки-скобы ящика для канцелярии.
        /// </summary>
        /// <param name="worktopLength"> Длина столешницы.</param>
        /// <param name="drawerLength"> Длина ящика.</param>
        /// <param name="drawerHeight"> Высота ящика.</param>
        /// <param name="locationAlongZ"> Координата левого нижнего угла ящика
        /// по оси Z.</param>
        /// <param name="distanceBetweenFasteners"> Расстояние между крепежными
        /// элементами ручки-скобы.</param>
        private void BuildGripDrawerHandle(
            int worktopLength,
            int drawerLength,
            double drawerHeight,
            double locationAlongZ,
            int distanceBetweenFasteners)
        {
            // Вычисляем все размерности, необходимые для построения ручки-скобы.
            //
            var gripX = worktopLength - drawerLength / 2 - distanceBetweenFasteners / 2;
            var gripZ = locationAlongZ + drawerHeight / 2 - 
                (double)DrawerHandleDimensions.HandleHeight / 2;

            var outerGripX = gripX - 
                DrawerHandleDimensions.GripOuterInnerHandleLengthDifference / 2;
            var outerGripLength = distanceBetweenFasteners + 
                DrawerHandleDimensions.GripOuterInnerHandleLengthDifference;
            var outerGripWidth = (double)distanceBetweenFasteners / 2;

            var innerGripWidth = outerGripWidth - 
                DeskParameters.OuterInnerDrawerHeightDifference;

            // Строим внешнее пространство ручки-скобы.
            _wrapper.BuildCuboid(
                PlaneType.XoZ,
                outerGripX,
                gripZ,
                outerGripLength,
                DrawerHandleDimensions.HandleHeight,
                outerGripWidth,
                false, 
                false);

            // Строим внутреннее пространство ручки-скобы, вырезая его из внешнего 
            // пространства.
            _wrapper.BuildCuboid(
                PlaneType.XoZ,
                gripX,
                gripZ,
                distanceBetweenFasteners,
                DrawerHandleDimensions.HandleHeight,
                innerGripWidth,
                true,
                false);
        }

        /// <summary>
        /// Метод, выполняющий построение ручки-кнопки ящика для канцелярии.
        /// </summary>
        /// <param name="worktopLength"> Длина столешницы.</param>
        /// <param name="drawerLength"> Длина ящика.</param>
        /// <param name="drawerHeight"> Высота ящика.</param>
        /// <param name="locationAlongZ"> Координата левого нижнего угла ящика
        /// по оси Z.</param>
        /// <param name="baseDiameter"> Диаметр основания ручки-кнопки.</param>
        private void BuildKnobDrawerHandle(
            int worktopLength,
            int drawerLength,
            double drawerHeight,
            double locationAlongZ,
            int baseDiameter)
        {
            // Вычисляем все размерности, необходимые для построения ручки-кнопки.
            //
            var knobX = worktopLength - drawerLength / 2;
            var knobZ = locationAlongZ + drawerHeight / 2 - 
                (double)DrawerHandleDimensions.HandleHeight / 2;

            var knobLegX = knobX - baseDiameter / 2.5;
            var knobLegY = -(DrawerHandleDimensions.KnobHandleBaseWidth + 
                DrawerHandleDimensions.KnobHandleLegWidth);
            var knobLegSketchBulgeByVertex = new Dictionary<Point, double>
            {
                {
                    new Point(knobLegX, knobLegY),
                    DrawerHandleDimensions.KnobHandleLegBulgeAngleInDegrees
                },
                {
                    new Point(knobLegX, -DrawerHandleDimensions.KnobHandleBaseWidth),
                    0.0
                },
                {
                    new Point(knobX, -DrawerHandleDimensions.KnobHandleBaseWidth),
                    0.0
                },
                {
                    new Point(knobX, knobLegY), 0.0
                }
            };

            var knobLegDisplacementStartPoint = new Point3D(knobX,
                -DrawerHandleDimensions.KnobHandleBaseWidth, 0);
            var knobLegDisplacementEndPoint = new Point3D(knobX,
                -DrawerHandleDimensions.KnobHandleBaseWidth, -knobZ);
            var knobLegRevolutionAxisStartPoint = new Point3D(knobX,
                -DrawerHandleDimensions.KnobHandleBaseWidth, -knobZ);
            var knobLegRevolutionAxisEndPoint = new Point3D(knobX, knobLegY, -knobZ);

            var knobCapBaseDiameter = baseDiameter + 
                DrawerHandleDimensions.KnobHandleCapBaseHandleBaseDiameterDifference;
            var knobCapBaseY = -(DrawerHandleDimensions.KnobHandleBaseWidth + 
                DrawerHandleDimensions.KnobHandleLegWidth);
            var knobCapBaseDisplacementStartPoint = new Point3D(knobX, 0, knobZ);
            var knobCapBaseDisplacementEndPoint = new Point3D(knobX, knobCapBaseY, 
                knobZ);

            var knobCapDiameter = baseDiameter + 
                DrawerHandleDimensions.KnobHandleCapHandleBaseDiameterDifference;
            var knobCapY = -(DrawerHandleDimensions.KnobHandleBaseWidth + 
                DrawerHandleDimensions.KnobHandleLegWidth +
                DrawerHandleDimensions.KnobHandleCapBaseWidth);
            var knobCapDisplacementStartPoint = new Point3D(knobX, 0, knobZ);
            var knobCapDisplacementEndPoint = new Point3D(knobX, knobCapY, knobZ);

            // Строим основание ручки-кнопки, закругляя его ребра.
            _wrapper.BuildCylinder(PlaneType.XoZ,
                baseDiameter,
                knobX,
                knobZ, 
                DrawerHandleDimensions.KnobHandleBaseWidth, 
                false,
                DrawerHandleDimensions.KnobHandleBaseFilletEdgeRadius,
                DrawerHandleDimensions.KnobHandleBaseFilletEdgeStartSetback,
                DrawerHandleDimensions.KnobHandleBaseFilletEdgeEndSetback);

            // Строим ножку с изгибом, соединяющую основание всей ручки-кнопки
            // с основанием шляпки.
            _wrapper.BuildComplexObjectByRevolution(
                PlaneType.XoY,
                knobLegSketchBulgeByVertex,
                knobLegDisplacementStartPoint,
                knobLegDisplacementEndPoint,
                knobLegRevolutionAxisStartPoint,
                knobLegRevolutionAxisEndPoint);

            // Строим основание шляпки с закругленными ребрами.
            _wrapper.BuildCylinder(
                PlaneType.XoZ,
                knobCapBaseDiameter,
                knobX,
                knobZ,
                knobCapBaseDisplacementStartPoint,
                knobCapBaseDisplacementEndPoint,
                DrawerHandleDimensions.KnobHandleCapBaseWidth,
                DrawerHandleDimensions.KnobHandleCapBaseFilletEdgeRadius,
                DrawerHandleDimensions.KnobHandleCapBaseFilletEdgeStartSetback,
                DrawerHandleDimensions.KnobHandleCapBaseFilletEdgeEndSetback);

            // Строим шляпку, закругляя ее ребра.
            _wrapper.BuildCylinder(
                PlaneType.XoZ,
                knobCapDiameter,
                knobX,
                knobZ,
                knobCapDisplacementStartPoint,
                knobCapDisplacementEndPoint,
                DrawerHandleDimensions.KnobHandleCapWidth,
                DrawerHandleDimensions.KnobHandleCapFilletEdgeRadius,
                DrawerHandleDimensions.KnobHandleCapFilletEdgeStartSetback,
                DrawerHandleDimensions.KnobHandleCapFilletEdgeEndSetback);
        }

        #endregion
    }
}
