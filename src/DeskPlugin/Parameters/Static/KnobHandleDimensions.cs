namespace Parameters.Static
{
    /// <summary>
    /// Статический класс, хранящий размерности ручек-кнопок ящиков для канцелярии.
    /// </summary>
    public class KnobHandleDimensions
    {
        /// <summary>
        /// Ширина основания ручки-кнопки.
        /// </summary>
        public static int HandleBaseWidth => 10;

        /// <summary>
        /// Радиус закругления ребер для основания ручки-кнопки.
        /// </summary>
        public static int HandleBaseFilletEdgeRadius => 10;

        /// <summary>
        /// Начальный отступ закругления ребер для основания ручки-кнопки.
        /// </summary>
        public static int HandleBaseFilletEdgeStartSetback => 1;

        /// <summary>
        /// Конечный отступ закругления ребер для основания ручки-кнопки.
        /// </summary>
        public static int HandleBaseFilletEdgeEndSetback => 5;

        /// <summary>
        /// Ширина ножки ручки-кнопки.
        /// </summary>
        public static int HandleLegWidth => 30;

        /// <summary>
        /// Угол изгиба (выпуклости) ножки ручки-кнопки, выраженный в градусах.
        /// </summary>
        public static int HandleLegBulgeAngleInDegrees => 15;

        /// <summary>
        /// Ширина основания шляпки ручки-кнопки.
        /// </summary>
        public static int HandleCapBaseWidth => 5;

        /// <summary>
        /// Радиус закругления ребер для основания шляпки ручки-кнопки.
        /// </summary>
        public static int HandleCapBaseFilletEdgeRadius => 10;

        /// <summary>
        /// Начальный отступ закругления ребер для основания шляпки ручки-кнопки.
        /// </summary>
        public static int HandleCapBaseFilletEdgeStartSetback => 1;

        /// <summary>
        /// Конечный отступ закругления ребер для основания шляпки ручки-кнопки.
        /// </summary>
        public static int HandleCapBaseFilletEdgeEndSetback => 5;

        /// <summary>
        /// Разница между диаметром основания шляпки ручки-кнопки и диаметром
        /// основания всей ручки-кнопки целиком.
        /// </summary>
        public static int HandleCapBaseHandleBaseDiameterDifference => 20;

        /// <summary>
        /// Разница между диаметром шляпки ручки-кнопки и диаметром основания всей
        /// ручки-кнопки целиком.
        /// </summary>
        public static int HandleCapHandleBaseDiameterDifference => 10;

        /// <summary>
        /// Ширина шляпки ручки-кнопки.
        /// </summary>
        public static int HandleCapWidth => 20;

        /// <summary>
        /// Радиус закругления ребер для шляпки ручки-кнопки.
        /// </summary>
        public static int HandleCapFilletEdgeRadius => 20;

        /// <summary>
        /// Начальный отступ закругления ребер для шляпки ручки-кнопки.
        /// </summary>
        public static int HandleCapFilletEdgeStartSetback => 5;

        /// <summary>
        /// Конечный отступ закругления ребер для шляпки ручки-кнопки.
        /// </summary>
        public static int HandleCapFilletEdgeEndSetback => 10;
    }
}
