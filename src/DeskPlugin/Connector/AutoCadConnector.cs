using Autodesk.AutoCAD.Runtime;
using Desk.Views.Windows;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Connector
{
    /// <summary>
    /// Класс, предназначенный для взаимодействия AutoCAD с плагином.
    /// </summary>
    public class AutoCadConnector : IExtensionApplication
    {
        #region Methods

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

        #endregion
    }
}
