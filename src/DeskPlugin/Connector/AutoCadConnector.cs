using System.Diagnostics;
using System.IO;
using Autodesk.AutoCAD.Runtime;
using Builder;
using Desk;
using Desk.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic.Devices;
using Parameters;
using Wrappers;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Connector
{
    /// <summary>
    /// Класс, предназначенный для взаимодействия AutoCAD с плагином.
    /// </summary>
    public class AutoCadConnector : IExtensionApplication
    {
        #region Fields

        /// <summary>
        /// Текущий рабочий каталог программы.
        /// </summary>
        private readonly string _outputDirectory = Directory.GetCurrentDirectory();

        #endregion

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

        /// <summary>
        /// Командный метод, запускающий нагрузочное тестирование плагина.
        /// </summary>
        [CommandMethod("StartLoadTesting", CommandFlags.Session)]
        public void StartLoadTesting()
        {
            // Один байт в гигабайтах.
            const double oneByteInGigabytes = 0.000000000931322574615478515625;

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var builder = new DeskBuilder(new AutoCadWrapper("Desk"));
            var parameters = new DeskParameters();
            var streamWriter = new StreamWriter(Path.Combine(_outputDirectory, 
                "LoadTestingResults.txt"), true);
            var deskModelCount = 0;

            while (true)
            {
                builder.BuildDesk(parameters);
                var computerInfo = new ComputerInfo();

                // Вычисляем память ПК (в гигабайтах), затраченную на построение
                // 3D-моделей письменного стола в AutoCAD.
                var utilizedMemory = (computerInfo.TotalPhysicalMemory 
                    - computerInfo.AvailablePhysicalMemory) * oneByteInGigabytes;

                // В текстовый файл выводим информацию в виде трех колонок: порядковый
                // номер 3D-модели; общее количество миллисекунд с момента запуска
                // нагрузочного тестирования; общее количество затраченной памяти ПК
                // (в гигабайтах).
                streamWriter.WriteLine($"{++deskModelCount}\t" 
                    + $"{stopWatch.ElapsedMilliseconds}\t{utilizedMemory}");
                streamWriter.Flush();
            }
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
