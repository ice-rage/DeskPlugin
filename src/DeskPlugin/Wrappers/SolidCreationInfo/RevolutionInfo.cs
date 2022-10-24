using System.Windows.Media.Media3D;

namespace Wrappers.SolidCreationInfo
{
    /// <summary>
    /// Класс, содержащий данные об операции вращения, которую необходимо произвести
    /// над определенным 2D-объектом, чтобы получить из него 3D-объект.
    /// </summary>
    internal class RevolutionInfo
    {
        #region Properties

        /// <summary>
        /// Начальная точка оси вращения.
        /// </summary>
        public Point3D StartAxisPoint { get; }

        /// <summary>
        /// Конечная точка оси вращения.
        /// </summary>
        public Point3D EndAxisPoint { get; }

        /// <summary>
        /// Угол вращения (в градусах).
        /// </summary>
        public double Angle { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Создает экземпляр <see cref="RevolutionInfo"/>.
        /// </summary>
        /// <param name="startAxisPoint"> Начальная точка оси вращения.</param>
        /// <param name="endAxisPoint"> Конечная точка оси вращения.</param>
        /// <param name="angle"> Угол вращения (в градусах).</param>
        public RevolutionInfo(Point3D startAxisPoint, Point3D endAxisPoint, 
            double angle)
        {
            StartAxisPoint = startAxisPoint;
            EndAxisPoint = endAxisPoint;
            Angle = angle;
        }

        #endregion
    }
}
