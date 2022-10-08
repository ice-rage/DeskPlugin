using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace Desk.Views.Windows
{
    /// <summary>
    /// Класс главного окна плагина.
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Создает экземпляр <see cref="MainWindow"/>.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // TODO: Костыль?..
            // Чтобы во время запуска плагина из AutoCAD не было сгенерировано
            // исключение "Could not load file or assembly Microsoft.Xaml.Behaviors",
            // приходится ссылаться на (любой) объект этой библиотеки из code-behind
            var _ = new DefaultTriggerAttribute(typeof(Trigger),
                typeof(Microsoft.Xaml.Behaviors.TriggerBase), null);
        }
    }
}
