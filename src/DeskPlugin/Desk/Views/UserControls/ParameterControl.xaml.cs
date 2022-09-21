using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace Desk.Views.UserControls
{
    public partial class ParameterControl
    {
        public ParameterControl()
        {
            InitializeComponent();

            //TODO: костыль?..
            var _ = new DefaultTriggerAttribute(typeof(Trigger),
                typeof(Microsoft.Xaml.Behaviors.TriggerBase), null);
        }
    }
}
