namespace Parameters.Static
{
    /// <summary>
    /// Статический класс, хранящий размерности ручек-рейлинг ящиков для канцелярии.
    /// </summary>
    public class RailingHandleDimensions
    {
        /// <summary>
        /// Разница в длине поперечной перекладины ручки-рейлинг и расстоянии между
        /// ее крепежными элементами.
        /// </summary>
        public static int CrossbeamFastenersLengthDifference => 60;

        /// <summary>
        /// Диаметр поперечной перекладины ручки-рейлинг.
        /// </summary>
        public static int CrossbeamDiameter => 25;

        /// <summary>
        /// Угол поворота (в градусах) поперечной перекладины ручки-рейлинг
        /// (для горизонтального положения в пространстве).
        /// </summary>
        public static int CrossbeamRotationAngleInDegrees => 90;

        /// <summary>
        /// Диаметр крепежной ножки ручки-рейлинг.
        /// </summary>
        public static int FasteningLegDiameter => 20;
    }
}
