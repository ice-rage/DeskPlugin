using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Desk.Extensions;
using Microsoft.Xaml.Behaviors;

namespace Desk.Behaviors
{
    /// <summary>
    /// Класс, реализующий поведение для минимизации (сворачивания) окна.
    /// </summary>
    public class MinimizeWindowBehavior : Behavior<Button>
    {
        #region Methods

        /// <inheritdoc/>
        protected override void OnAttached() => AssociatedObject.Click += OnButtonClick;

        /// <inheritdoc/>
        protected override void OnDetaching() => AssociatedObject.Click -= OnButtonClick;

        #region EventHandlers

        /// <summary>
        /// Обработчик события нажатия на кнопку минимизации окна.
        /// </summary>
        /// <param name="sender"> Отправитель события.</param>
        /// <param name="e"> Аргументы события.</param>
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (!(AssociatedObject.FindRoot(typeof(VisualTreeHelper)) is Window window))
            {
                return;
            }

            window.WindowState = WindowState.Minimized;
        }

        #endregion

        #endregion
    }
}
