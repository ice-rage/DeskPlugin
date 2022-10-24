namespace Wrappers.SolidCreationInfo
{
    /// <summary>
    /// Класс, содержащий данные об операции выдавливания, которую необходимо
    /// произвести над определенным 2D-объектом.
    /// </summary>
    internal class ExtrusionInfo
    {
        #region Properties

        /// <summary>
        /// Высота выдавливания.
        /// </summary>
        public double Height { get; }

        /// <summary>
        /// Угол образования конуса (применяется только для создания усеченных тел).
        /// </summary>
        public double TaperAngle { get; }

        /// <summary>
        /// Показывает, выполняется ли операция вырезания текущего объекта из другого
        /// объекта во время выдавливания.
        /// </summary>
        public bool IsExtrusionCuttingOut { get; }

        /// <summary>
        /// Показывает, в каком направлении выполняется выдавливание.
        /// </summary>
        public bool IsDirectionPositive { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Создает экземпляр <see cref="ExtrusionInfo"/>.
        /// </summary>
        /// <param name="height"> Высота выдавливания.</param>
        /// <param name="taperAngle"> Угол образования конуса.</param>
        /// <param name="isExtrusionCuttingOut"> Показывает, выполняется ли операция
        /// вырезания текущего объекта из другого объекта во время выдавливания.
        /// </param>
        /// <param name="isDirectionPositive"> Показывает, в каком направлении
        /// выполняется выдавливание.</param>
        public ExtrusionInfo(
            double height,
            double taperAngle,
            bool isExtrusionCuttingOut, 
            bool isDirectionPositive)
        {
            Height = height;
            TaperAngle = taperAngle;
            IsExtrusionCuttingOut = isExtrusionCuttingOut;
            IsDirectionPositive = isDirectionPositive;
        }

        #endregion
    }
}
