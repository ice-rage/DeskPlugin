using Services.Enums;

namespace Services.Interfaces
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
        /// <param name="diameter"> Диаметр окружности.</param>
        /// <param name="x"> Координата центра окружности по оси X.</param>
        /// <param name="y"> Координата центра окружности по оси Y.</param>
        /// <returns> Полученная окружность.</returns>
        object CreateCircle(int diameter, double x, double y);

        /// <summary>
        /// Создает твердое тело посредством операции выдавливания 2D-объекта на
        /// заданную высоту.
        /// </summary>
        /// <param name="obj"> 2D-объект, на основе которого необходимо выполнить
        /// выдавливание.</param>
        /// <param name="height"> Высота выдавливания.</param>
        /// <param name="cuttingByExtrusion"> Показывает, выполнять ли вырезание
        /// выдавливанием (данная операция применяется в некоторых САПР для удаления
        /// объема внутри другого 3D-тела).</param>
        /// <param name="isPositiveDirection"> Показывает, в каком направлении
        /// выполнять операцию выдавливания (по умолчанию выполняется
        /// в положительном направлении).</param>
        void Extrude(object obj, double height, bool cuttingByExtrusion = false, 
            bool isPositiveDirection = true);

        /// <summary>
        /// Вызывается непосредственно для построения 3D-модели в используемой САПР.
        /// </summary>
        void Build();
    }
}
