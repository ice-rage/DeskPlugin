using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Desk.Behaviors
{
    /// <summary>
    /// Класс, реализующий поведение для отображения меню окна при нажатии на его иконку левой
    /// кнопкой мыши.
    /// </summary>
    internal class ShowWindowMenuBehavior : Behavior<FrameworkElement>
    {
        #region Methods

        /// <inheritdoc/>
        protected override void OnAttached() => AssociatedObject.MouseUp += OnMouseUp;

        /// <inheritdoc/>
        protected override void OnDetaching() => AssociatedObject.MouseUp -= OnMouseUp;

        #region Event Handlers

        /// <summary>
        /// Обработчик события освобождения кнопки мыши.
        /// </summary>
        /// <param name="sender"> Отправитель события.</param>
        /// <param name="e"> Аргументы события.</param>
        private static void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is FrameworkElement element))
            {
                return;
            }

            if (!(element.TemplatedParent is Window window))
            {
                return;
            }

            // Вычисляем координаты центра иконки окна и в зависимости от состояния окна отображаем
            // меню либо в правом нижнем углу иконки, либо в ее центре.
            double halfWidthIcon = element.ActualWidth / 2;
            double halfHeightIcon = element.ActualHeight / 2;

            Point point = window.WindowState == WindowState.Maximized
                ? new Point(halfWidthIcon, halfHeightIcon)
                : new Point(window.Left + halfWidthIcon, window.Top + halfHeightIcon);

            SystemCommands.ShowSystemMenu(window, point);
        }

        #endregion

        #endregion
    }
}
