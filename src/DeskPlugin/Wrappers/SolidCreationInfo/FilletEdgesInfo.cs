namespace Wrappers.SolidCreationInfo
{
    /// <summary>
    /// Класс, содержащий данные об операции скругления ребер 3D-объекта.
    /// </summary>
    internal class FilletEdgesInfo
    {
        #region Properties

        /// <summary>
        /// Радиус скругления.
        /// </summary>
        public double Radius { get; }

        /// <summary>
        /// Начальный отступ от ребра 3D-объекта.
        /// </summary>
        public double StartSetback { get; }

        /// <summary>
        /// Конечный отступ от ребра 3D-объекта.
        /// </summary>
        public double EndSetback { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Создает экземпляр <see cref="FilletEdgesInfo"/>.
        /// </summary>
        /// <param name="radius"> Радиус скругления.</param>
        /// <param name="startSetback"> Начальный отступ от ребра 3D-объекта.</param>
        /// <param name="endSetback"> Конечный отступ от ребра 3D-объекта.</param>
        public FilletEdgesInfo(double radius, double startSetback, double endSetback)
        {
            Radius = radius;
            StartSetback = startSetback;
            EndSetback = endSetback;
        }

        #endregion
    }
}
