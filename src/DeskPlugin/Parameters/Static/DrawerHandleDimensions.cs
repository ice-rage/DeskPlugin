using Parameters.Enums;

namespace Parameters.Static
{
    /// <summary>
    /// Статический класс, хранящий размерности различных частей ручек ящиков
    /// для канцелярии, которые описаны в перечислении <see cref="DrawerHandleType"/>.
    /// </summary>
    public static class DrawerHandleDimensions
    {
        /// <summary>
        /// Высота ручек ящиков для канцелярии.
        /// </summary>
        public static int HandleHeight => 20;

        #region Railing Handle Dimensions

        /// <summary>
        /// Разница в длине поперечной перекладины ручки-рейлинг и расстоянии между
        /// ее крепежными элементами.
        /// </summary>
        public static int RailingCrossbeamFastenersLengthDifference => 60;

        /// <summary>
        /// Диаметр поперечной перекладины ручки-рейлинг.
        /// </summary>
        public static int RailingCrossbeamDiameter => 25;

        /// <summary>
        /// Угол поворота (в градусах) поперечной перекладины ручки-рейлинг
        /// (для горизонтального положения в пространстве).
        /// </summary>
        public static int RailingCrossbeamRotationAngleInDegrees => 90;

        /// <summary>
        /// Диаметр крепежной ножки ручки-рейлинг.
        /// </summary>
        public static int RailingFasteningLegDiameter => 20;

        #endregion

        #region Grip Handle Dimensions

        /// <summary>
        /// Разница в длине между внешним и внутренним пространством ручки-скобы.
        /// </summary>
        public static int GripOuterInnerHandleLengthDifference => 40;

        #endregion

        #region Knob Handle Dimensions

        /// <summary>
        /// Ширина основания ручки-кнопки.
        /// </summary>
        public static int KnobHandleBaseWidth => 10;

        /// <summary>
        /// Радиус закругления ребер для основания ручки-кнопки.
        /// </summary>
        public static int KnobHandleBaseFilletEdgeRadius => 10;

        /// <summary>
        /// Начальный отступ закругления ребер для основания ручки-кнопки.
        /// </summary>
        public static int KnobHandleBaseFilletEdgeStartSetback => 1;

        /// <summary>
        /// Конечный отступ закругления ребер для основания ручки-кнопки.
        /// </summary>
        public static int KnobHandleBaseFilletEdgeEndSetback => 5;

        /// <summary>
        /// Ширина ножки ручки-кнопки.
        /// </summary>
        public static int KnobHandleLegWidth => 30;

        /// <summary>
        /// Угол изгиба (выпуклости) ножки ручки-кнопки, выраженный в градусах.
        /// </summary>
        public static int KnobHandleLegBulgeAngleInDegrees => 15;

        /// <summary>
        /// Ширина основания шляпки ручки-кнопки.
        /// </summary>
        public static int KnobHandleCapBaseWidth => 5;

        /// <summary>
        /// Радиус закругления ребер для основания шляпки ручки-кнопки.
        /// </summary>
        public static int KnobHandleCapBaseFilletEdgeRadius => 10;

        /// <summary>
        /// Начальный отступ закругления ребер для основания шляпки ручки-кнопки.
        /// </summary>
        public static int KnobHandleCapBaseFilletEdgeStartSetback => 1;

        /// <summary>
        /// Конечный отступ закругления ребер для основания шляпки ручки-кнопки.
        /// </summary>
        public static int KnobHandleCapBaseFilletEdgeEndSetback => 5;

        /// <summary>
        /// Разница между диаметром основания шляпки ручки-кнопки и диаметром
        /// основания всей ручки-кнопки целиком.
        /// </summary>
        public static int KnobHandleCapBaseHandleBaseDiameterDifference => 20;

        /// <summary>
        /// Разница между диаметром шляпки ручки-кнопки и диаметром основания всей
        /// ручки-кнопки целиком.
        /// </summary>
        public static int KnobHandleCapHandleBaseDiameterDifference => 10;

        /// <summary>
        /// Ширина шляпки ручки-кнопки.
        /// </summary>
        public static int KnobHandleCapWidth => 20;

        /// <summary>
        /// Радиус закругления ребер для шляпки ручки-кнопки.
        /// </summary>
        public static int KnobHandleCapFilletEdgeRadius => 20;

        /// <summary>
        /// Начальный отступ закругления ребер для шляпки ручки-кнопки.
        /// </summary>
        public static int KnobHandleCapFilletEdgeStartSetback => 5;

        /// <summary>
        /// Конечный отступ закругления ребер для шляпки ручки-кнопки.
        /// </summary>
        public static int KnobHandleCapFilletEdgeEndSetback => 10;

        #endregion
    }
}
