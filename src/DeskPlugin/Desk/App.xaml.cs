using System;
using System.Windows;
using Desk.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using ViewModels.Registration;
using Wrappers.Registration;

namespace Desk
{
    /// <summary>
    /// Базовый класс приложения WPF, производный от класса
    /// <see cref="Application"/>.
    /// </summary>
    public partial class App
    {
        #region Fields

        /// <summary>
        /// Обеспечивает доступ ко всем зарегистрированным сервисам плагина.
        /// </summary>
        private static IServiceProvider _serviceProvider;

        #endregion

        #region Properties

        /// <summary>
        /// Обеспечивает доступ ко всем зарегистрированным сервисам плагина.
        /// </summary>
        public static IServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider != null)
                {
                    return _serviceProvider;
                }

                var services = new ServiceCollection();

                // Выполняем необходимую настройку сервисов.
                ConfigureServices(services);
                _serviceProvider = services.BuildServiceProvider();

                return _serviceProvider;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Метод для конфигурации сервисов плагина.
        /// </summary>
        /// <param name="services"> Коллекция сервисов плагина, с которой предстоит
        /// работать.</param>
        private static void ConfigureServices(IServiceCollection services)
        {
            // Регистрируем окна плагина, модели-представления и классы-оболочки
            // используемых САПР.
            services
                .AddSingleton<MainWindow>()
                .RegisterViewModels()
                .RegisterWrappers();
        }

        #region Event Handlers

        /// <summary>
        /// Обработчик события запуска плагина (используется вместо создания
        /// и отображения главного окна напрямую).
        /// </summary>
        /// <param name="sender"> Отправитель события.</param>
        /// <param name="e"> Аргументы события.</param>
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = ServiceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }

        #endregion

        #endregion
    }
}
