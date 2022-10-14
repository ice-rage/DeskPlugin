using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ViewModels.Registration;

namespace TestViewModels
{
    /// <summary>
    /// Класс, содержащий модульные тесты для класса <see cref="ViewModelRegistrar"/>.
    /// </summary>
    [TestFixture]
    public class ViewModelRegistrarTests
    {
        #region Method Tests

        [TestCase(TestName = "Метод расширения RegisterViewModels() должен " +
                             "возвращать коллекцию зарегистрированных моделей представления " +
                             "с указанным временем жизни")]
        public void TestRegisterViewModel_ReturnsViewModelCollection()
        {
            // Arrange
            var services = new ServiceCollection();
            var expectedViewModels = services.RegisterViewModels();

            var expectedLifeTimes = new List<ServiceLifetime>
            {
                ServiceLifetime.Singleton
            };

            services.Clear();

            // Act
            var actualViewModels = services.RegisterViewModels();
            var actualLifeTimes = actualViewModels
                .Select(actualViewModel => actualViewModel.Lifetime)
                .ToList();

            // Assert
            Assert.Multiple(() =>
            {
                CollectionAssert.AreEqual(expectedViewModels, actualViewModels);
                CollectionAssert.AreEqual(expectedLifeTimes, actualLifeTimes);
            });
        }

        #endregion
    }
}
