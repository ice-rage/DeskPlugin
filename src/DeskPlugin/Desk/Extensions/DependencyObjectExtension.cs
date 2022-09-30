using System;
using System.Windows;
using System.Windows.Media;

namespace Desk.Extensions
{
    /// <summary>
    /// Класс, расширяющий базовый класс <see cref="DependencyObject"/>.
    /// </summary>
    public static class DependencyObjectExtension
    {
        /// <summary>
        /// Метод для поиска корневого элемента объекта, являющегося наследником класса
        /// <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="obj"> Объект-наследник базового класса <see cref="DependencyObject"/>.
        /// </param>
        /// <param name="helperType"> Тип статического класса, который задается в зависимости от
        /// назначения поиска корневого элемента (в визуальном или логическом дереве элементов).
        /// </param>
        /// <returns> Найденный корневой объект визуального (логического) дерева.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static DependencyObject FindRoot(this DependencyObject obj, Type helperType)
        {
            if (helperType is null || (helperType != typeof(VisualTreeHelper) && 
                helperType != typeof(LogicalTreeHelper)))
            {
                throw new ArgumentException($"The \"{nameof(helperType)}\" parameter " + 
                    "must be of type VisualTreeHelper or LogicalTreeHelper");
            }

            do
            {
                DependencyObject parent = helperType == typeof(VisualTreeHelper) 
                    ? VisualTreeHelper.GetParent(obj) 
                    : LogicalTreeHelper.GetParent(obj);

                if (parent is null)
                {
                    return obj;
                }

                obj = parent;
            }
            while (true);
        }
    }
}
