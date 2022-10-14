using Autodesk.AutoCAD.Runtime;
using Desk;
using Desk.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Connector
{
    /// <summary>
    /// Класс, предназначенный для взаимодействия AutoCAD с плагином.
    /// </summary>
    public class AutoCadConnector : IExtensionApplication
    {
        #region Methods

        /// <summary>
        /// Метод для запуска плагина из среды AutoCAD.
        /// </summary>
        [CommandMethod("StartDeskPlugin")]
        public static void StartDeskPlugin()
        {
            // Извлекаем нужный сервис главного окна, используя контейнер сервисов
            // плагина.
            Application.ShowModelessWindow(App.ServiceProvider
                .GetRequiredService<MainWindow>());
        }

        /// <inheritdoc/>
        public void Initialize()
        {
        }

        /// <inheritdoc/>
        public void Terminate()
        {
        }

        #endregion
    }
}
