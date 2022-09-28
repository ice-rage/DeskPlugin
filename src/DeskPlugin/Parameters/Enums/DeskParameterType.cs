using System.ComponentModel;

namespace Parameters.Enums
{
    /// <summary>
    /// Перечисление, содержащее типы параметров письменного стола.
    /// </summary>
    public enum DeskParameterType
    {
        /// <summary>
        /// Длина столешницы.
        /// </summary>
        [Description("Length (L1)")]
        WorktopLength,

        /// <summary>
        /// Ширина столешницы.
        /// </summary>
        [Description("Width (B)")]
        WorktopWidth,

        /// <summary>
        /// Высота столешницы.
        /// </summary>
        [Description("Height (H1)")]
        WorktopHeight,

        /// <summary>
        /// Диаметр основания ножек.
        /// </summary>
        [Description("Base Diameter (D)")]
        LegBaseDiameter,

        /// <summary>
        /// Длина основания ножек.
        /// </summary>
        [Description("Base Length (A)")]
        LegBaseLength,

        /// <summary>
        /// Высота ножек.
        /// </summary>
        [Description("Height (H2)")]
        LegHeight,

        /// <summary>
        /// Количество ящиков для канцелярии.
        /// </summary>
        [Description("Number")]
        DrawerNumber,

        /// <summary>
        /// Длина ящиков для канцелярии.
        /// </summary>
        [Description("Length (L2)")]
        DrawerLength
    }
}