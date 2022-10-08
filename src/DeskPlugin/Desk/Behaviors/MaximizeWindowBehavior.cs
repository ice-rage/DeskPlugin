using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Desk.Extensions;
using Microsoft.Xaml.Behaviors;

namespace Desk.Behaviors
{
    /// <summary>
    /// Класс, реализующий поведение для максимизации (разворачивания на весь экран)
    /// окна.
    /// </summary>
    internal class MaximizeWindowBehavior : Behavior<Button>
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
        /// Обработчик события нажатия на кнопку максимизации окна.
        /// </summary>
        /// <param name="sender"> Отправитель события.</param>
        /// <param name="e"> Аргументы события.</param>
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (!(AssociatedObject.FindRoot(typeof(VisualTreeHelper)) is 
                    Window window))
            {
                return;
            }

            // В зависимости от текущего состояния окна разворачиваем его либо
            // сворачиваем до обычного размера.
            switch (window.WindowState)
            {
                case WindowState.Normal:
                {
                    window.WindowState = WindowState.Maximized;
                    break;
                }
                case WindowState.Maximized:
                {
                    window.WindowState = WindowState.Normal;
                    break;
                }
            }
        }

        #endregion

        #endregion
    }
}
