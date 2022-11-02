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

        /// <summary>
        /// Параметры, необходимые для построения 3D-модели.
        /// </summary>
        private DeskParameters _parameters;

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
        /// Метод для построения 3D-модели.
        /// </summary>
        /// <param name="parameters"> </param>
        public void BuildDesk(DeskParameters parameters)
        {
            _parameters = parameters;
			var worktopLength = _parameters[DeskParameterGroupType.Worktop,
                DeskParameterType.WorktopLength].Value;
            var worktopWidth = _parameters[DeskParameterGroupType.Worktop,
                DeskParameterType.WorktopWidth].Value;
            
            var legHeight = _parameters[DeskParameterGroupType.Legs,
                DeskParameterType.LegHeight].Value;
            var drawerNumber = _parameters[DeskParameterGroupType.Drawers,
                DeskParameterType.DrawerNumber].Value;
            var drawerHeight = (double)legHeight / drawerNumber;
            
            BuildWorktop(worktopLength, worktopWidth);

            BuildLegs(legHeight, worktopWidth);

            BuildDrawers(drawerNumber, drawerHeight, worktopLength, worktopWidth);

            _wrapper.Build();
        }

        /// <summary>
        /// Метод для построения столешницы.
        /// </summary>
        /// <param name="worktopLength"> Длина столешницы.</param>
        /// <param name="worktopWidth"> Ширина столешницы.</param>
        private void BuildWorktop(int worktopLength, int worktopWidth)
        {
            // Получаем высоту столешницы.
            var worktopHeight = _parameters[DeskParameterGroupType.Worktop,
                DeskParameterType.WorktopHeight].Value;

            _wrapper.BuildCuboid(
                PlaneType.XoY,
                0,
                0,
                worktopLength,
                worktopWidth,
                worktopHeight,
                false,
                true);
        }

        /// <summary>
        /// Метод для построения ножек письменного стола.
        /// </summary>
        /// <param name="legHeight"> Высота ножек.</param>
        /// <param name="worktopWidth"> Ширина столешницы, необходимая для расчета
        /// положения ножек относительно краев стола.</param>
        private void BuildLegs(int legHeight, int worktopWidth)
        {
            // Получаем параметры, необходимые для построения ножек стола.
            //
            var legType = _parameters.LegType;
            var legBaseValue = _parameters[DeskParameterGroupType.Legs,
                _parameters.LegType.GetLegBaseType()].Value;

            // Создаем словари, содержащие в качестве ключа порядковый номер ножки,
            // а в качестве значения - одну из координат ее основания.
            var x = new Dictionary<int, double>();
            var y = new Dictionary<int, double>();

            // Строим основания ножек.
            var legBases = legType == LegType.Round
                ? CreateRoundLegBases(legBaseValue, worktopWidth, x, y)
                : CreateSquareLegBases(legBaseValue, worktopWidth, x, y);

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
        /// <param name="drawerHeight"> Высота ящиков для канцелярии.</param>
        /// <param name="worktopLength"> Длина столешницы, необходимая для расчета
        /// размеров и положения ящиков для канцелярии, их дверц и ручек.</param>
        /// <param name="worktopWidth"> Ширина столешницы, необходимая для расчета
        /// размеров и положения ящиков для канцелярии, их дверц и ручек.</param>
        private void BuildDrawers(int drawerNumber, double drawerHeight, 
            int worktopLength, int worktopWidth)
        {
            // Получаем все параметры письменного стола, необходимые, чтобы построить
            // ящики для канцелярии.
            //
            var drawerLength = _parameters[DeskParameterGroupType.Drawers,
                DeskParameterType.DrawerLength].Value;
            var drawerHandleType = _parameters.HandleType;
            var drawerHandleDimension = _parameters[DeskParameterGroupType.Drawers,
                _parameters.HandleType.GetHandleMutableParameterType()].Value;

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
                switch (drawerHandleType)
                {
                    case DrawerHandleType.Railing:
                    {
                        BuildRailingDrawerHandle(
                            worktopLength,
                            drawerLength,
                            drawerHeight,
                            drawerZ, 
                            drawerHandleDimension);
                        break;
                    }
                    case DrawerHandleType.Knob:
                    {
                        BuildKnobDrawerHandle(
                            worktopLength,
                            drawerLength,
                            drawerHeight,
                            drawerZ,
                            drawerHandleDimension);
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
                            drawerHandleDimension);
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
                (double)DeskParameters.HandleHeight / 2;

            var crossbeamLength = distanceBetweenFasteners + 
                RailingHandleDimensions.CrossbeamFastenersLengthDifference;
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
                RailingHandleDimensions.CrossbeamDiameter,
                crossbeamX,
                railingZ,
                crossbeamRotationAxis,
                RailingHandleDimensions.CrossbeamRotationAngleInDegrees,
                crossbeamRotationPoint,
                crossbeamLength);

            // Строим левую крепежную ножку.
            _wrapper.BuildCylinder(
                PlaneType.XoZ,
                RailingHandleDimensions.FasteningLegDiameter,
                leftFasteningLegX,
                railingZ,
                crossbeamHalfLength,
                false);

            // Строим правую крепежную ножку.
            _wrapper.BuildCylinder(
                PlaneType.XoZ,
                RailingHandleDimensions.FasteningLegDiameter,
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
            // Разница в длине между внешним и внутренним пространством ручки-скобы.
            const int gripHandleOuterInnerHandleLengthDifference = 40;

            // Вычисляем все размерности, необходимые для построения ручки-скобы.
            //
            var gripX = worktopLength - drawerLength / 2 - distanceBetweenFasteners / 2;
            var gripZ = locationAlongZ + drawerHeight / 2 - 
                (double)DeskParameters.HandleHeight / 2;

            var outerGripX = gripX - 
                gripHandleOuterInnerHandleLengthDifference / 2;
            var outerGripLength = distanceBetweenFasteners + 
                gripHandleOuterInnerHandleLengthDifference;
            var outerGripWidth = (double)distanceBetweenFasteners / 2;

            var innerGripWidth = outerGripWidth - 
                DeskParameters.OuterInnerDrawerHeightDifference;

            // Строим внешнее пространство ручки-скобы.
            _wrapper.BuildCuboid(
                PlaneType.XoZ,
                outerGripX,
                gripZ,
                outerGripLength,
                DeskParameters.HandleHeight,
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
                DeskParameters.HandleHeight,
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
                (double)DeskParameters.HandleHeight / 2;

            var knobLegX = knobX - baseDiameter / 2.5;
            var knobLegY = -(KnobHandleDimensions.HandleBaseWidth + 
                KnobHandleDimensions.HandleLegWidth);
            var knobLegSketchBulgeByVertex = new Dictionary<Point, double>
            {
                {
                    new Point(knobLegX, knobLegY),
                    KnobHandleDimensions.HandleLegBulgeAngleInDegrees
                },
                {
                    new Point(knobLegX, -KnobHandleDimensions.HandleBaseWidth),
                    0.0
                },
                {
                    new Point(knobX, -KnobHandleDimensions.HandleBaseWidth),
                    0.0
                },
                {
                    new Point(knobX, knobLegY), 0.0
                }
            };

            var knobLegDisplacementStartPoint = new Point3D(knobX,
                -KnobHandleDimensions.HandleBaseWidth, 0);
            var knobLegDisplacementEndPoint = new Point3D(knobX,
                -KnobHandleDimensions.HandleBaseWidth, -knobZ);
            var knobLegRevolutionAxisStartPoint = new Point3D(knobX,
                -KnobHandleDimensions.HandleBaseWidth, -knobZ);
            var knobLegRevolutionAxisEndPoint = new Point3D(knobX, knobLegY, -knobZ);

            var knobCapBaseDiameter = baseDiameter + 
                KnobHandleDimensions.HandleCapBaseHandleBaseDiameterDifference;
            var knobCapBaseY = -(KnobHandleDimensions.HandleBaseWidth + 
                KnobHandleDimensions.HandleLegWidth);
            var knobCapBaseDisplacementStartPoint = new Point3D(knobX, 0, knobZ);
            var knobCapBaseDisplacementEndPoint = new Point3D(knobX, knobCapBaseY, 
                knobZ);

            var knobCapDiameter = baseDiameter + 
                KnobHandleDimensions.HandleCapHandleBaseDiameterDifference;
            var knobCapY = -(KnobHandleDimensions.HandleBaseWidth + 
                KnobHandleDimensions.HandleLegWidth + 
                KnobHandleDimensions.HandleCapBaseWidth);
            var knobCapDisplacementStartPoint = new Point3D(knobX, 0, knobZ);
            var knobCapDisplacementEndPoint = new Point3D(knobX, knobCapY, knobZ);

            // Строим основание ручки-кнопки, закругляя его ребра.
            _wrapper.BuildCylinder(PlaneType.XoZ,
                baseDiameter,
                knobX,
                knobZ, 
                KnobHandleDimensions.HandleBaseWidth, 
                false,
                KnobHandleDimensions.HandleBaseFilletEdgeRadius,
                KnobHandleDimensions.HandleBaseFilletEdgeStartSetback,
                KnobHandleDimensions.HandleBaseFilletEdgeEndSetback);

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
                KnobHandleDimensions.HandleCapBaseWidth,
                KnobHandleDimensions.HandleCapBaseFilletEdgeRadius,
                KnobHandleDimensions.HandleCapBaseFilletEdgeStartSetback,
                KnobHandleDimensions.HandleCapBaseFilletEdgeEndSetback);

            // Строим шляпку, закругляя ее ребра.
            _wrapper.BuildCylinder(
                PlaneType.XoZ,
                knobCapDiameter,
                knobX,
                knobZ,
                knobCapDisplacementStartPoint,
                knobCapDisplacementEndPoint,
                KnobHandleDimensions.HandleCapWidth,
                KnobHandleDimensions.HandleCapFilletEdgeRadius,
                KnobHandleDimensions.HandleCapFilletEdgeStartSetback,
                KnobHandleDimensions.HandleCapFilletEdgeEndSetback);
        }

        #endregion
    }
}
