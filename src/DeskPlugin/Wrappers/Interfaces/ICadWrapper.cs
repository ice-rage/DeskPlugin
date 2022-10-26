using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;
using Wrappers.Enums;

namespace Wrappers.Interfaces
{
    /// <summary>
    /// Интерфейс класса-оболочки для изолирования методов API используемой САПР.
    /// </summary>
    public interface ICadWrapper
    {
        /// <summary>
        /// Создает 2D-примитив типа "прямоугольник".
        /// </summary>
        /// <param name="planeType"> Плоскость, в которой необходимо построить
        /// прямоугольник.</param>
        /// <param name="x"> Х-координата базовой точки прямоугольника.</param>
        /// <param name="y"> Y-координата базовой точки прямоугольника.</param>
        /// <param name="width"> Ширина прямоугольника (длина стороны).</param>
        /// <param name="height"> Высота прямоугольника.</param>
        /// /// <returns> Полученный прямоугольник.</returns>
        object CreateRectangle(
            PlaneType planeType, 
            double x, 
            double y, 
            double width, 
            double height);

        /// <summary>
        /// Создает 2D-примитив типа "окружность".
        /// </summary>
        /// <param name="planeType"> Плоскость, в которой необходимо построить
        /// окружность.</param>
        /// <param name="diameter"> Диаметр окружности.</param>
        /// <param name="x"> Координата центра окружности по оси X.</param>
        /// <param name="y"> Координата центра окружности по оси Y.</param>
        /// <returns> Полученная окружность.</returns>
        object CreateCircle(PlaneType planeType, int diameter, double x, double y);

        /// <summary>
        /// Создает твердое тело посредством операции выдавливания области, полученной
        /// из 2D-объекта, на заданную высоту.
        /// </summary>
        /// <param name="obj"> Область, для которой необходимо выполнить выдавливание.
        /// </param>
        /// <param name="height"> Высота выдавливания.</param>
        /// <param name="taperAngle"> Угол образования конуса (в градусах).
        /// Используется для создания усеченной фигуры при выдавливании.</param>
        /// <param name="isExtrusionCuttingOut"> Показывает, выполнять ли вырезание
        /// выдавливанием. Данная операция применяется в некоторых САПР для удаления
        /// объема внутри другого 3D-тела.</param>
        /// <param name="isDirectionPositive"> Показывает, в каком направлении
        /// выполнять операцию выдавливания. По умолчанию выдавливание выполняется
        /// в положительном направлении.</param>
        void Extrude(
            object obj,
            double height,
            double taperAngle = 0.0,
            bool isExtrusionCuttingOut = false,
            bool isDirectionPositive = true);

        /// <summary>
        /// Строит параллелепипед.
        /// </summary>
        /// <param name="planeType"> Плоскость, в которой необходимо построить
        /// параллелепипед.</param>
        /// <param name="x"> Х-координата базовой точки параллелепипеда.</param>
        /// <param name="y"> Y-координата базовой точки параллелепипеда.</param>
        /// <param name="baseWidth"> Ширина основания параллелепипеда (длина стороны).
        /// </param>
        /// <param name="baseHeight"> Высота выдавливания основания параллелепипеда.
        /// </param>
        /// <param name="extrusionHeight"> Высота выдавливания основания
        /// для образования параллелепипеда.</param>
        /// <param name="isExtrusionCuttingOut"> Показывает, выполнять ли вырезание
        /// выдавливанием. Данная операция применяется в некоторых САПР для удаления
        /// объема внутри другого 3D-тела.</param>
        /// <param name="isDirectionPositive"> Показывает, в каком направлении
        /// выполнять операцию выдавливания. По умолчанию выдавливание выполняется
        /// в положительном направлении.</param>
        void BuildCuboid(
            PlaneType planeType,
            double x,
            double y,
            double baseWidth,
            double baseHeight,
            double extrusionHeight,
            bool isExtrusionCuttingOut,
            bool isDirectionPositive);

        /// <summary>
        /// Строит цилиндр.
        /// </summary>
        /// <param name="planeType"> Плоскость, в которой необходимо построить
        /// цилиндр.</param>
        /// <param name="baseDiameter"> Диаметр основания цилиндра.</param>
        /// <param name="x"> Координата центра основания цилиндра по оси X.</param>
        /// <param name="y"> Координата центра основания цилиндра по оси Y.</param>
        /// <param name="extrusionHeight"> Высота выдавливания основания цилиндра.
        /// </param>
        /// <param name="isDirectionPositive"> Показывает, в каком направлении
        /// выполнять операцию выдавливания. По умолчанию выдавливание выполняется
        /// в положительном направлении.</param>
        void BuildCylinder(
            PlaneType planeType,
            int baseDiameter,
            double x,
            double y,
            double extrusionHeight,
            bool isDirectionPositive);

        /// <summary>
        /// Строит цилиндр, поворачивая его в пространстве на указанный угол.
        /// </summary>
        /// <param name="planeType"> Плоскость, в которой необходимо построить
        /// цилиндр.</param>
        /// <param name="baseDiameter"> Диаметр основания цилиндра.</param>
        /// <param name="x"> Координата центра основания цилиндра по оси X.</param>
        /// <param name="y"> Координата центра основания цилиндра по оси Y.</param>
        /// <param name="rotationAxis"> Вектор оси вращения цилиндра.</param>
        /// <param name="rotationAngle"> Угол вращения цилиндра (в градусах).</param>
        /// <param name="rotationPoint"> Исходная точка вращения цилиндра.</param>
        /// <param name="extrusionHeight"> Высота выдавливания основания цилиндра.
        /// </param>
        void BuildCylinder(
            PlaneType planeType,
            int baseDiameter,
            double x,
            double y,
            Vector3D rotationAxis, 
            double rotationAngle,
            Point3D rotationPoint,
            double extrusionHeight);

        /// <summary>
        /// Строит цилиндр с закругленными ребрами.
        /// </summary>
        /// <param name="planeType"> Плоскость, в которой необходимо построить
        /// цилиндр.</param>
        /// <param name="baseDiameter"> Диаметр основания цилиндра.</param>
        /// <param name="x"> Координата центра основания цилиндра по оси X.</param>
        /// <param name="y"> Координата центра основания цилиндра по оси Y.</param>
        /// <param name="extrusionHeight"> Высота выдавливания цилиндра.</param>
        /// <param name="isDirectionPositive"> Показывает, в каком направлении
        /// выполнять операцию выдавливания. По умолчанию выдавливание выполняется
        /// в положительном направлении.</param>
        /// <param name="filletRadius"> Радиус скругления ребер цилиндра.</param>
        /// <param name="filletStartSetback"> Начало отступа ("задержки") скругления
        /// относительно ребра цилиндра.</param>
        /// <param name="filletEndSetback"> Конец отступа ("задержки") скругления
        /// относительно ребра цилиндра.</param>
        void BuildCylinder(
            PlaneType planeType,
            int baseDiameter,
            double x,
            double y,
            double extrusionHeight,
            bool isDirectionPositive,
            double filletRadius,
            double filletStartSetback,
            double filletEndSetback);

        /// <summary>
        /// Строит цилиндр с закругленными ребрами, перемещая его в указанную точку
        /// пространства.
        /// </summary>
        /// <param name="planeType"> Плоскость, в которой необходимо построить
        /// цилиндр.</param>
        /// <param name="baseDiameter"> Диаметр основания цилиндра.</param>
        /// <param name="x"> Координата центра основания цилиндра по оси X.</param>
        /// <param name="y"> Координата центра основания цилиндра по оси Y.</param>
        /// <param name="basePoint"> Базовая точка, относительно которой производится
        /// перемещение цилиндра в пространстве.</param>
        /// <param name="displacementPoint"> Конечная точка, в которую необходимо
        /// переместить цилиндр.</param>
        /// <param name="extrusionHeight"> Высота выдавливания цилиндра.</param>
        /// <param name="filletRadius"> Радиус скругления ребер цилиндра.</param>
        /// <param name="filletStartSetback"> Начало отступа ("задержки") скругления
        /// относительно ребра цилиндра.</param>
        /// <param name="filletEndSetback"> Конец отступа ("задержки") скругления
        /// относительно ребра цилиндра.</param>
        void BuildCylinder(
            PlaneType planeType,
            int baseDiameter,
            double x,
            double y,
            Point3D basePoint,
            Point3D displacementPoint,
            double extrusionHeight,
            double filletRadius,
            double filletStartSetback,
            double filletEndSetback);

        /// <summary>
        /// Строит сложный 3D-объект путем вращения 2D-объекта, образованного ломаной
        /// линией, по оси, которая проходит через данный 2D-объект.
        /// </summary>
        /// <param name="planeType"> Плоскость, в которой необходимо построить
        /// объект.</param>
        /// <param name="bulgeByVertex"> Словарь для сопоставления определенного
        /// угла выпуклости (в градусах) каждой вершине ломаной линии (используется
        /// для создания 3D-объекта сложной формы).</param>
        /// <param name="basePoint"> Базовая точка, относительно которой производится
        /// перемещение объекта в пространстве.</param>
        /// <param name="displacementPoint"> Конечная точка, в которую необходимо
        /// переместить объект.</param>
        /// <param name="revolutionAxisStartPoint"> Начальная точка оси вращения.
        /// </param>
        /// <param name="revolutionAxisEndPoint"> Конечная точка оси вращения.</param>
        /// <param name="revolutionAngle"> Угол вращения объекта (в градусах).
        /// По умолчанию составляет 360 градусов (для получения замкнутого
        /// 3D-объекта).</param>
        void BuildComplexObjectByRevolution(
            PlaneType planeType,
            Dictionary<Point, double> bulgeByVertex,
            Point3D basePoint,
            Point3D displacementPoint,
            Point3D revolutionAxisStartPoint,
            Point3D revolutionAxisEndPoint,
            double revolutionAngle = 360);

        /// <summary>
        /// Вызывается непосредственно для построения 3D-модели в используемой САПР.
        /// </summary>
        void Build();
    }
}
