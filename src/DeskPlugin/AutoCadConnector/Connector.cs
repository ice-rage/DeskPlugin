using Autodesk.AutoCAD.Runtime;
using Desk.Views.Windows;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace AutoCadConnector
{
    /// <summary>
    /// Класс, предназначенный для взаимодействия AutoCAD с плагином.
    /// </summary>
    public class Connector : IExtensionApplication
    {
        [CommandMethod("StartDeskPlugin")]
        public static void StartDeskPlugin() => Application.ShowModelessWindow(new MainWindow());

        /// <inheritdoc/>
        public void Initialize()
        {
        }

        /// <inheritdoc/>
        public void Terminate()
        {
        }
    }
}
