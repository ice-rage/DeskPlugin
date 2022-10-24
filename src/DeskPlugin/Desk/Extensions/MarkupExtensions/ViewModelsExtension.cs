using System;
using System.Windows.Markup;

namespace Desk.Extensions.MarkupExtensions
{
    /// <summary>
    /// Класс, предоставляющий экземпляр модели представления по ее типу.
    /// <para> Наследуется от класса <see cref="MarkupExtension"/> с целью
    /// использования данного класса непосредственно в разметке окон.</para>
    /// </summary>
    internal class ViewModelsExtension : MarkupExtension
    {
        #region Properties

        /// <summary>
        /// Тип модели представления.
        /// </summary>
        public Type ViewModelType { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Создает экземпляр <see cref="ViewModelsExtension"/>.
        /// </summary>
        /// <param name="viewModelType"> Тип модели представления, которую необходимо
        /// получить.</param>
        public ViewModelsExtension(Type viewModelType) => 
            ViewModelType = viewModelType;

        #endregion

        #region Methods

        #region MarkupExtension Overriding

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // Используем контейнер сервисов плагина, чтобы получить нужную модель
            // представления.
            return App.ServiceProvider.GetService(ViewModelType);
        }

        #endregion

        #endregion
    }
}
