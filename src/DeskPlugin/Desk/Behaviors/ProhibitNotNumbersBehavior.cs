using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Desk.Behaviors
{
    /// <summary>
    /// Класс, реализующий поведение для запрета ввода неразрешенных (отличных от
    /// цифр) символов в текстовые поля.
    /// </summary>
    internal class ProhibitNotNumbersBehavior : Behavior<TextBox>
    {
        #region Fields

        /// <summary>
        /// Регулярное выражение, соответствующее запрещенным символам ввода (т.е.
        /// всем символам, кроме цифр от 0 до 9).
        /// </summary>
        private static readonly Regex _regex = new Regex("[^0-9]+");

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            AssociatedObject.PreviewTextInput += OnPreviewTextInput;
            DataObject.AddPastingHandler(AssociatedObject, OnPasting);
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            AssociatedObject.PreviewTextInput -= OnPreviewTextInput;
            DataObject.RemovePastingHandler(AssociatedObject, OnPasting);
        }

        /// <summary>
        /// Метод, применяющий специальное регулярное выражение для определения
        /// корректности ввода.
        /// </summary>
        /// <param name="text"> Текст, содержащийся в текстовом поле.</param>
        /// <returns> <see langword="true"/>, если ввод корректен, и
        /// <see langword="false"/> в противном случае.</returns>
        private static bool IsTextAllowed(string text) => !_regex.IsMatch(text);

        #region Event Handlers

        /// <summary>
        /// Обработчик события получения элементом <see cref="TextBox"/> текстового
        /// ввода.
        /// </summary>
        /// <param name="sender"> Отправитель события.</param>
        /// <param name="e"> Аргументы события.</param>
        private static void OnPreviewTextInput(object sender, 
            TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        /// <summary>
        /// Обработчик события вставки текста в текстовое поле.
        /// </summary>
        /// <param name="sender"> Отправитель события.</param>
        /// <param name="e"> Аргументы события.</param>
        private static void OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(typeof(string)))
            {
                return;
            }

            var text = (string)e.DataObject.GetData(typeof(string));

            // Прерываем операцию вставки текста в поле текстового элемента, если этот текст
            // содержит неразрешенные символы.
            if (!IsTextAllowed(text))
            {
                e.CancelCommand();
            }
        }

        #endregion

        #endregion
    }
}
