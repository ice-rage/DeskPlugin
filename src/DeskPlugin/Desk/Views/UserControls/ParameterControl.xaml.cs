using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace Desk.Views.UserControls
{
    /// <summary>
    /// Класс пользовательского элемента управления для отображения параметра письменного стола.
    /// </summary>
    public partial class ParameterControl
    {
        /// <summary>
        /// Создает экземпляр <see cref="ParameterControl"/>.
        /// </summary>
        public ParameterControl()
        {
            InitializeComponent();

            // TODO: Костыль?..
            // Чтобы во время запуска плагина из AutoCAD не было сгенерировано исключение "Could
            // not load file or assembly Microsoft.Xaml.Behaviors", приходится ссылаться на (любой)
            // объект этой библиотеки из code-behind
            var _ = new DefaultTriggerAttribute(typeof(Trigger),
                typeof(Microsoft.Xaml.Behaviors.TriggerBase), null);
        }
    }
}
