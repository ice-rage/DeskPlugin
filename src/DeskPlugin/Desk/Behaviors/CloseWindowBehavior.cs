using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Desk.Extensions;
using Microsoft.Xaml.Behaviors;

namespace Desk.Behaviors
{
    /// <summary>
    /// Класс, реализующий поведение для закрытия окна.
    /// </summary>
    internal class CloseWindowBehavior : Behavior<Button>
    {
        #region Methods

        /// <inheritdoc/>
        protected override void OnAttached() => AssociatedObject.Click += 
            OnButtonClick;

        /// <inheritdoc/>
        protected override void OnDetaching() => AssociatedObject.Click -= 
            OnButtonClick;

        #region Event Handlers

        /// <summary>
        /// Обработчик события нажатия на кнопку закрытия окна.
        /// </summary>
        /// <param name="sender"> Отправитель события.</param>
        /// <param name="e"> Аргументы события.</param>
        private void OnButtonClick(object sender, RoutedEventArgs e) => 
            (AssociatedObject.FindRoot(typeof(VisualTreeHelper)) as Window)?.Close();

        #endregion

        #endregion
    }
}
