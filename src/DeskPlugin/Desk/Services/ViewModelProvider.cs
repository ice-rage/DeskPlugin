using System;
using System.Windows.Markup;

namespace Desk.Services
{
    /// <summary>
    /// Класс, предоставляющий экземпляр модели представления по ее типу.
    /// <para> Наследуется от класса <see cref="MarkupExtension"/> с целью
    /// использования данного класса непосредственно в разметке окон.</para>
    /// </summary>
    public class ViewModelProvider : MarkupExtension
    {
        #region Properties

        /// <summary>
        /// Тип модели представления.
        /// </summary>
        public Type ViewModelType { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Создает экземпляр <see cref="ViewModelProvider"/>.
        /// </summary>
        /// <param name="viewModelType"> Тип модели представления.</param>
        public ViewModelProvider(Type viewModelType)
        {
            ViewModelType = viewModelType;
        }

        #endregion

        #region Methods

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
    }
}
