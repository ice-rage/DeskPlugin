using System.Collections.Generic;
using System.Windows.Media.Media3D;
using Parameters;
using Parameters.Enums;
using Parameters.Enums.Extensions;
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
        /// Объект класса-оболочки для изолированного вызова методов API используемой САПР.
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
        /// <param name="parameters"> Параметры, необходимые для построения 3D-модели.</param>
        public void BuildDesk(DeskParameters parameters)
        {
            int worktopLength = parameters[DeskParameterGroupType.Worktop,
                DeskParameterType.WorktopLength].Value;
            int worktopWidth = parameters[DeskParameterGroupType.Worktop,
                DeskParameterType.WorktopWidth].Value;
            int worktopHeight = parameters[DeskParameterGroupType.Worktop,
                DeskParameterType.WorktopHeight].Value;
            int legHeight = parameters[DeskParameterGroupType.Legs,
                DeskParameterType.LegHeight].Value;
            int drawerNumber = parameters[DeskParameterGroupType.Drawers,
                DeskParameterType.DrawerNumber].Value;
            int drawerLength = parameters[DeskParameterGroupType.Drawers,
                DeskParameterType.DrawerLength].Value;
            double drawerHeight = (double)legHeight / drawerNumber;

            LegType legType = parameters.LegType;
            int legBaseValue = parameters[DeskParameterGroupType.Legs,
                parameters.LegType.GetLegBaseType()].Value;

            BuildWorktop(worktopLength, worktopWidth, worktopHeight, legHeight);

            BuildLegs(legType, legBaseValue, legHeight, worktopWidth);

            BuildDrawers(drawerNumber, drawerLength, drawerHeight, worktopLength,
                worktopWidth);

            _wrapper.Build();
        }

        /// <summary>
        /// Метод для построения столешницы.
        /// </summary>
        /// <param name="worktopLength"> Длина столешницы.</param>
        /// <param name="worktopWidth"> Ширина столешницы.</param>
        /// <param name="worktopHeight"> Высота столешницы.</param>
        /// <param name="legHeight"> Высота ножек.</param>
        private void BuildWorktop(int worktopLength, int worktopWidth, int worktopHeight, 
            int legHeight)
        {
            var displacementEndPoint = new Point3D(worktopLength, worktopWidth, 
                worktopHeight + legHeight);
            _wrapper.BuildSimpleBox(worktopLength, worktopWidth, worktopHeight, 
                displacementEndPoint);
        }

        /// <summary>
        /// Метод для построения ножек письменного стола.
        /// </summary>
        /// <param name="legType"> Тип ножек, описанный в перечислении
        /// <see cref="DeskParameterType"/>.</param>
        /// <param name="legBase"> Значение основания ножек.</param>
        /// <param name="legHeight"> Высота ножек.</param>
        /// <param name="worktopWidth"> Ширина столешницы, необходимая для расчета положения ножек
        /// относительно краев стола.</param>
        private void BuildLegs(LegType legType, int legBase, int legHeight, int worktopWidth)
        {
            // Создаем словари, содержащие в качестве ключа порядковый номер ножки, а в качестве
            // значения - одну из координат ее основания.
            //
            var x = new Dictionary<int, double>();
            var y = new Dictionary<int, double>();

            // Строим основания ножек.
            IEnumerable<object> legBases = legType == LegType.Round
                ? CreateRoundLegBases(legBase, worktopWidth, x, y)
                : CreateSquareLegBases(legBase, worktopWidth, x, y);

            // Выполняем операцию выдавливания для основания каждой ножки.
            foreach (object leg in legBases)
            {
                _wrapper.Extrude(leg, legHeight);
            }
        }

        /// <summary>
        /// Метод для создания круглых оснований ножек письменного стола (если выбран тип ножек
        /// <see cref="LegType.Round"/>).
        /// </summary>
        /// <param name="baseDiameter"> Диаметр основания ножек.</param>
        /// <param name="worktopWidth"> Ширина столешницы, необходимая для расчета положения ножек
        /// относительно краев стола.</param>
        /// <param name="x"> Словарь, содержащий в качестве ключа порядковый номер ножки, а
        /// в качестве значения - x-координату центра окружности основания ножки.</param>
        /// <param name="y"> Словарь, содержащий в качестве ключа порядковый номер ножки, а
        /// в качестве значения - y-координату центра окружности основания ножки.</param>
        /// <returns> Массив круглых оснований ножек.</returns>
        private IEnumerable<object> CreateRoundLegBases(int baseDiameter, int worktopWidth,
            IDictionary<int, double> x, IDictionary<int, double> y)
        {
            var roundLegBases = new List<object>();

            // В каждый словарь оснований добавляем координату центра окружности основания
            // соответствующей ножки.
            //
            int baseCenter = DeskParameters.DistanceFromWorktopCorner + (baseDiameter / 2);
            x.Add(0, baseCenter);
            y.Add(0, baseCenter);

            x.Add(1, baseCenter);
            y.Add(1, worktopWidth - baseCenter);

            // Создаем окружности основания ножек и добавляем их в массив.
            for (var i = 0; i < x.Count; i++)
            {
                object circle = _wrapper.CreateCircle(baseDiameter, x[i], y[i]);
                roundLegBases.Add(circle);
            }

            return roundLegBases;
        }

        /// <summary>
        /// Метод для создания квадратных оснований ножек письменного стола (если выбран тип ножек
        /// <see cref="LegType.Square"/>).
        /// </summary>
        /// <param name="baseLength"> Длина основания ножек.</param>
        /// <param name="worktopWidth"> Ширина столешницы, необходимая для расчета положения ножек
        /// относительно краев стола.</param>
        /// <param name="x"> Словарь, содержащий в качестве ключа порядковый номер ножки, а
        /// в качестве значения - x-координату левого нижнего угла квадрата основания ножки.</param>
        /// <param name="y"> Словарь, содержащий в качестве ключа порядковый номер ножки, а
        /// в качестве значения - y-координату левого нижнего угла квадрата основания ножки.</param>
        /// <returns> Массив квадратных оснований ножек.</returns>
        private IEnumerable<object> CreateSquareLegBases(int baseLength, int worktopWidth,
            IDictionary<int, double> x, IDictionary<int, double> y)
        {
            var squareLegBases = new List<object>();

            // В каждый словарь оснований добавляем координату левого нижнего угла квадрата
            // основания соответствующей ножки.
            //
            x.Add(0, DeskParameters.DistanceFromWorktopCorner);
            y.Add(0, DeskParameters.DistanceFromWorktopCorner);

            x.Add(1, DeskParameters.DistanceFromWorktopCorner);
            y.Add(1, worktopWidth - DeskParameters.DistanceFromWorktopCorner - baseLength);

            // Создаем квадраты оснований ножек и добавляем их в массив.
            for (var i = 0; i < x.Count; i++)
            {
                object square = _wrapper.CreateSquare(baseLength, x[i], y[i]);
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
        /// <param name="worktopLength"> Длина столешницы, необходимая для расчета размеров
        /// и положения ящиков для канцелярии, их дверц и ручек в пространстве чертежа.</param>
        /// <param name="worktopWidth"> Ширина столешницы, необходимая для расчета размеров
        /// и положения ящиков для канцелярии, их дверц и ручек в пространстве чертежа.</param>
        private void BuildDrawers(
            int drawerNumber,
            int drawerLength,
            double drawerHeight,
            int worktopLength,
            int worktopWidth)
        {
            for (var i = 1; i <= drawerNumber; i++)
            {
                // Строим ящик для канцелярии.
                _wrapper.BuildCompositeBox(
                    drawerLength,
                    worktopWidth - DeskParameters.WorktopDrawerWidthDifference,
                    drawerHeight,
                    new Point3D(
                        worktopLength - DeskParameters.DistanceFromWorktopCorner,
                        worktopWidth - DeskParameters.DistanceFromWorktopCorner,
                        drawerHeight * i),
                    drawerLength - DeskParameters.OuterInnerDrawerLengthDifference,
                    worktopWidth - DeskParameters.WorktopDrawerWidthDifference -
                    DeskParameters.OuterInnerDrawerWidthDifference,
                    drawerHeight - DeskParameters.OuterInnerDrawerHeightDifference,
                    new Point3D(
                        worktopLength - DeskParameters.OuterInnerDrawerLengthDifference,
                        worktopWidth - DeskParameters.OuterInnerDrawerWidthDifference,
                        drawerHeight * i));

                // Строим дверцу ящика.
                _wrapper.BuildSimpleBox(
                    drawerLength - DeskParameters.DrawerDoorLengthDifference,
                    DeskParameters.DoorWidth,
                    drawerHeight - DeskParameters.DrawerDoorHeightDifference,
                    new Point3D(
                        worktopLength - DeskParameters.DrawerDoorLengthDifference,
                        DeskParameters.DoorWidth + DeskParameters.DistanceFromWorktopCorner,
                        drawerHeight * i));

                // Строим ручку ящика.
                _wrapper.BuildCompositeBox(
                    (double)drawerLength / 2,
                    DeskParameters.OuterHandleWidth,
                    DeskParameters.HandleHeight,
                    new Point3D(
                        worktopLength - DeskParameters.DistanceFromWorktopCorner - 
                        drawerLength / 4,
                        DeskParameters.DistanceFromWorktopCorner,
                        drawerHeight * i - drawerHeight / 2 + 
                        (double)DeskParameters.HandleHeight / 2),
                    drawerLength / 2 - 
                    DeskParameters.OuterInnerHandleLengthDifference,
                    DeskParameters.InnerHandleWidth,
                    DeskParameters.HandleHeight,
                    new Point3D(
                        worktopLength - DeskParameters.DistanceFromWorktopCorner - 
                        drawerLength / 4 - DeskParameters.OuterInnerHandleLengthDifference / 2,
                        DeskParameters.DistanceFromWorktopCorner,
                        drawerHeight * i - drawerHeight / 2 + 
                        (double)DeskParameters.HandleHeight / 2));
            }
        }

        #endregion
    }
}
