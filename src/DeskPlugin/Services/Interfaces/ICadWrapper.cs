using System.Windows.Media.Media3D;

namespace Services.Interfaces
{
    /// <summary>
    /// Интерфейс класса-оболочки для изолирования методов API используемой САПР.
    /// </summary>
    public interface ICadWrapper
    {
        /// <summary>
        /// Создает 3D-примитив типа "ящик" ип перемещает его в указанную точку чертежа.
        /// </summary>
        /// <param name="boxLength"> Длина ящика.</param>
        /// <param name="boxWidth"> Ширина ящика.</param>
        /// <param name="boxHeight"> Высота ящика.</param>
        /// <param name="displacementEndPoint"> Точка чертежа, в которую необходимо переместить
        /// ящик.</param>
        void BuildSimpleBox(double boxLength, double boxWidth, double boxHeight,
            Point3D displacementEndPoint);

        /// <summary>
        /// Создает два 3D-примитива типа "ящик", вычитает второй ящик из первого, а затем
        /// добавляет результат вычитания в блок 3D-модели письменного стола.
        /// </summary>
        /// <param name="firstBoxLength"> Длина первого ящика.</param>
        /// <param name="firstBoxWidth"> Ширина первого ящика.</param>
        /// <param name="firstBoxHeight"> Высота первого ящика.</param>
        /// <param name="firstBoxDisplacementEndpoint"> Точка чертежа, в которую необходимо
        /// переместить первый ящик.</param>
        /// <param name="secondBoxLength"> Длина второго ящика.</param>
        /// <param name="secondBoxWidth"> Ширина второго ящика.</param>
        /// <param name="secondBoxHeight"> Высота второго ящика.</param>
        /// <param name="secondBoxDisplacementEndpoint"> Точка чертежа, в которую необходимо
        /// переместить второй ящик.</param>
        void BuildCompositeBox(
            double firstBoxLength,
            double firstBoxWidth,
            double firstBoxHeight,
            Point3D firstBoxDisplacementEndpoint,
            double secondBoxLength,
            double secondBoxWidth,
            double secondBoxHeight,
            Point3D secondBoxDisplacementEndpoint);

        /// <summary>
        /// Создает 2D-примитив типа "окружность".
        /// </summary>
        /// <param name="diameter"> Диаметр окружности.</param>
        /// <param name="x"> Координата центра окружности по оси X.</param>
        /// <param name="y"> Координата центра окружности по оси Y.</param>
        /// <returns> Полученная окружность.</returns>
        object CreateCircle(int diameter, double x, double y);

        /// <summary>
        /// Создает 2D-примитив типа "квадрат".
        /// </summary>
        /// <param name="sideLength"> Длина стороны квадрата.</param>
        /// <param name="x"> Координата левого нижнего угла квадрата по оси X.</param>
        /// <param name="y"> Координата левого нижнего угла квадрата по оси Y.</param>
        /// <returns> Полученный квадрат.</returns>
        object CreateSquare(int sideLength, double x, double y);

        /// <summary>
        /// Создает твердое тело посредством операции выдавливания 2D-объекта на заданную высоту.
        /// </summary>
        /// <param name="obj"> 2D-объект, на основе которого необходимо выполнить выдавливание.
        /// </param>
        /// <param name="height"> Высота выдавливания.</param>
        void Extrude(object obj, double height);

        /// <summary>
        /// Вызывается непосредственно для построения 3D-модели в используемой САПР.
        /// </summary>
        void Build();
    }
}
