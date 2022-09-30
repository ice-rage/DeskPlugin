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

            //TODO: костыль?..
            var _ = new DefaultTriggerAttribute(typeof(Trigger), 
                typeof(Microsoft.Xaml.Behaviors.TriggerBase), null);
        }
    }
}
