using System.Collections.Generic;
using Parameters;
using Parameters.Enums;
using Parameters.Enums.Extensions;
using Services.Enums;
using Services.Interfaces;

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
        public DeskBuilder(ICadWrapper wrapper)
        {
            _wrapper = wrapper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Метод для построения 3D-модели письменного стола.
        /// </summary>
        /// <param name="parameters"> Параметры, необходимые для построения
        /// 3D-модели.</param>
        public void BuildDesk(DeskParameters parameters)
        {
	        // TODO: Почему не var? И так по всему классу.
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

            BuildWorktop(worktopLength, worktopWidth, worktopHeight);

            BuildLegs(legType, legBaseValue, legHeight, worktopWidth);

            BuildDrawers(
                drawerNumber, 
                drawerLength, 
                drawerHeight, 
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
            int worktopHeight)
        {
            var worktop = _wrapper.CreateRectangle(PlaneType.XoY, 0, 0, 
                worktopLength, worktopWidth);
            _wrapper.Extrude(worktop, worktopHeight);
        }

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
                _wrapper.Extrude(leg, legHeight, isPositiveDirection: false);
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
                var circle = _wrapper.CreateCircle(baseDiameter, x[i], y[i]);
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
        /// ножки, а
        /// в качестве значения - y-координату левого нижнего угла квадрата
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
                var square = _wrapper.CreateRectangle(PlaneType.XoY, x[i], 
                    y[i], baseLength, baseLength);
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
        /// <param name="worktopLength"> Длина столешницы, необходимая для расчета
        /// размеров и положения ящиков для канцелярии, их дверц и ручек.</param>
        /// <param name="worktopWidth"> Ширина столешницы, необходимая для расчета
        /// размеров и положения ящиков для канцелярии, их дверц и ручек.</param>
        private void BuildDrawers(
            int drawerNumber,
            int drawerLength,
            double drawerHeight,
            int worktopLength,
            int worktopWidth)
        {
            for (var i = 0; i < drawerNumber; i++)
            {
                var y = drawerHeight * i;

                // Строим ящик для канцелярии.
                var outerDrawer = _wrapper.CreateRectangle(
                    PlaneType.XoZ, 
                    worktopLength - drawerLength, 
                    y, 
                    drawerLength, 
                    drawerHeight);
                _wrapper.Extrude(outerDrawer, worktopWidth);

                // Вырезаем отверстие (внутреннее пространство) в ящике.
                var innerDrawer = _wrapper
                    .CreateRectangle(PlaneType.XoZ, 
                        worktopLength - drawerLength + 
                        DeskParameters.OuterInnerDrawerLengthDifference / 2, 
                        y, 
                        drawerLength - 
                            DeskParameters.OuterInnerDrawerLengthDifference, 
                        drawerHeight - 
                            DeskParameters.OuterInnerDrawerHeightDifference);
                _wrapper.Extrude(innerDrawer, worktopWidth - 
                    DeskParameters.OuterInnerDrawerWidthDifference, 
                    cuttingByExtrusion: true);

                // Строим дверцу ящика.
                var drawerDoor = _wrapper
                    .CreateRectangle(
                        PlaneType.XoZ, worktopLength - drawerLength + 
                            DeskParameters.DrawerDoorLengthDifference / 2 + 
                            DeskParameters.InnerDrawerDoorDimensionsDifference, 
                        y + DeskParameters.InnerDrawerDoorDimensionsDifference, 
                        drawerLength - 
                            DeskParameters.DrawerDoorLengthDifference - 
                            2 * DeskParameters.InnerDrawerDoorDimensionsDifference, 
                        drawerHeight - 
                            DeskParameters.DrawerDoorHeightDifference - 
                            2 * DeskParameters.InnerDrawerDoorDimensionsDifference);
                _wrapper.Extrude(drawerDoor, DeskParameters.DoorWidth);

                // Строим ручку ящика.
                //
                var outerBox = _wrapper
                    .CreateRectangle(PlaneType.XoZ, 
                        worktopLength - drawerLength + drawerLength / 4, 
                        y + drawerHeight / 2 - 
                            (double)DeskParameters.HandleHeight / 2, 
                        (double)drawerLength / 2, 
                        DeskParameters.HandleHeight);
                _wrapper.Extrude(outerBox, DeskParameters.OuterHandleWidth, 
                    isPositiveDirection: false);

                var innerBox = _wrapper
                    .CreateRectangle(PlaneType.XoZ, 
                        worktopLength - drawerLength + drawerLength / 4 + 
                            DeskParameters.OuterInnerHandleLengthDifference / 2, 
                        y + drawerHeight / 2 - 
                            (double)DeskParameters.HandleHeight / 2, 
                        drawerLength / 2 - 
                            DeskParameters.OuterInnerHandleLengthDifference, 
                        DeskParameters.HandleHeight);
                _wrapper.Extrude(innerBox, DeskParameters.InnerHandleWidth, 
                    cuttingByExtrusion: true, isPositiveDirection: false);
            }
        }

        #endregion
    }
}
