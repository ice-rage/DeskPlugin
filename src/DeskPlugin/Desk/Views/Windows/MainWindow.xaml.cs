using DeskParameters.Enums;
using DeskViewModel;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System;

namespace Desk.Views.Windows
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            //TODO: костыли?..
            var _ = new DefaultTriggerAttribute(typeof(Trigger), 
                typeof(Microsoft.Xaml.Behaviors.TriggerBase), null);

            DataContext = new MainWindowViewModel();

            Array legTypeEnumDataSource = Enum.GetValues(typeof(LegType));
            LegTypeComboBox.ItemsSource = legTypeEnumDataSource;
        }
    }
}
