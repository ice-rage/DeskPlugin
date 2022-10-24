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
        /// Создает замкнутую ломаную линию, содержащую эллиптические дуги.
        /// </summary>
        /// <param name="planeType"> Плоскость, в которой необходимо построить
        /// ломаную линию.</param>
        /// <param name="bulgesByVertex"> Словарь для сопоставления определенного
        /// угла выпуклости (в градусах) каждой вершине ломаной линии.</param>
        /// <returns> Полученная ломаная линия.</returns>
        object CreatePolylineWithArcSegments(PlaneType planeType, Dictionary<Point, 
            double> bulgesByVertex);

        /// <summary>
        /// Перемещает объект в другую точку пространства относительно указанной
        /// базовой точки.
        /// </summary>
        /// <param name="obj"> Объект, который необходимо переместить.</param>
        /// <param name="basePoint"> Базовая точка перемещения.</param>
        /// <param name="displacementPoint"> Конечная точка перемещения.</param>
        /// <returns> Объект с измененными координатами расположения в трехмерном
        /// пространстве.
        /// </returns>
        object Move(object obj, Point3D basePoint, Point3D displacementPoint);

        /// <summary>
        /// Совершает поворот объекта вдоль указанной оси, проходящей через некоторую
        /// точку, на определенный угол вращения.
        /// </summary>
        /// <param name="obj"> Вращаемый объект.</param>
        /// <param name="rotationAxis"> Вектор оси вращения объекта.</param>
        /// <param name="rotationAngle"> Угол вращения (в градусах).</param>
        /// <param name="rotationPoint"> Исходная точка вращения.</param>
        /// <returns> Объект, изменивший угол расположения относительно указанной оси
        /// и точки вращения.</returns>
        object Rotate(object obj, Vector3D rotationAxis, double rotationAngle,
            Point3D rotationPoint);

        /// <summary>
        /// Создает твердое тело посредством операции выдавливания 2D-объекта
        /// на заданную высоту.
        /// </summary>
        /// <param name="obj"> 2D-объект, на основе которого необходимо выполнить
        /// выдавливание.</param>
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
        /// Создает твердое тело путем вращения 2D-объекта вокруг оси, которая
        /// определяется заданной начальной и конечной точкой.
        /// </summary>
        /// <param name="obj"> Двумерный объект, из которого необходимо получить
        /// твердотельный объект.</param>
        /// <param name="startAxisPoint"> Начальная точка оси вращения.</param>
        /// <param name="endAxisPoint"> Конечная точка оси вращения.</param>
        /// <param name="angle"> Угол вращения объекта (в градусах). По умолчанию
        /// составляет 360 градусов (для получения замкнутого твердого тела).</param>
        void Revolve(
            object obj, 
            Point3D startAxisPoint, 
            Point3D endAxisPoint, 
            double angle = 360);

        /// <summary>
        /// Закругляет ребра 3D-объекта.
        /// </summary>
        /// <param name="obj"> Объект, для которого применяется скругление.</param>
        /// <param name="radius"> Радиус скругления.</param>
        /// <param name="startSetback"> Начало отступа ("задержки") скругления
        /// относительно ребра объекта.</param>
        /// <param name="endSetback"> Конец отступа ("задержки") скругления
        /// относительно ребра объекта.</param>
        void FilletEdges(
            object obj,
            double radius,
            double startSetback,
            double endSetback);

        /// <summary>
        /// Вызывается непосредственно для построения 3D-модели в используемой САПР.
        /// </summary>
        void Build();
    }
}
