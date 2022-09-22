using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace Desk.Views.Windows
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            //TODO: костыль?..
            var _ = new DefaultTriggerAttribute(typeof(Trigger), 
                typeof(Microsoft.Xaml.Behaviors.TriggerBase), null);
        }
    }
}
