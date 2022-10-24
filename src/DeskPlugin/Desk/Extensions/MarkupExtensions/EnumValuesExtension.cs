using System;
using System.Windows.Markup;

namespace Desk.Extensions.MarkupExtensions
{
    /// <summary>
    /// Класс, предоставляющий значения некоторого перечисления по его типу.
    /// <para> Наследуется от класса <see cref="MarkupExtension"/> с целью
    /// использования данного класса непосредственно в разметке окон.</para>
    /// </summary>
    internal class EnumValuesExtension : MarkupExtension
    {
        #region Properties

        /// <summary>
        /// Тип перечисления.
        /// </summary>
        public Type EnumType { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Создает экземпляр <see cref="EnumValuesExtension"/>.
        /// </summary>
        /// <param name="enumType"> Тип перечисления, значения которого необходимо
        /// получить.</param>
        public EnumValuesExtension(Type enumType) => EnumType = enumType;

        #endregion

        #region Methods

        #region MarkupExtension Overriding

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override object ProvideValue(IServiceProvider serviceProvider) =>
            Enum.GetValues(EnumType);

        #endregion

        #endregion
    }
}
