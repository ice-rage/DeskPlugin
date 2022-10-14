using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using NUnit.Framework;
using Parameters;
using Parameters.Enums;
using Rhino.Mocks;
using Services.Interfaces;
using ViewModels;

namespace TestViewModel
{
    // TODO: Полетела кодировка
    ///<summary>
    /// Класс, содержащий модульные тесты для класса
    /// <see cref="MainWindowViewModel"/>.
    /// </summary>
    [TestFixture]
    public class MainWindowViewModelTests
    {
        #region Test Data Sources

        private readonly MockRepository _repository = new MockRepository();

        private ICadWrapper CreateMock() => _repository.StrictMock<ICadWrapper>();

        #endregion

        #region Constants For Testing

        /// <summary>
        /// Название модульных тестов для команд, устанавливающих значения
        /// параметров по умолчанию.
        /// </summary>
        private const string TestSetCommands_CanExecute_TestName =
            "При вызове команды {0} она должна содержать обработчик";

        #endregion

        #region Property Tests

        [TestCase(TestName = "Позитивный тест геттера Parameters")]
        public void TestParametersGet_GoodScenario()
        {
            // Arrange
            var expected = new DeskParameters();
            var wrapper = CreateMock();

            // Act
            var mainWindowViewModel = new MainWindowViewModel(wrapper);
            var actual = mainWindowViewModel.Parameters;

            // Assert
            Assert.Multiple(() =>
            {
                CollectionAssert.AreEqual(expected.ParametersByGroup,
                    actual.ParametersByGroup);
                Assert.AreEqual(expected.LegType, actual.LegType);
            });
        }

        [TestCase(TestName = "Позитивный тест геттера и сеттера IsDataValid")]
        public void TestIsDataValidGetSet_GoodScenario()
        {
            // Arrange
            var wrapper = CreateMock();

            // Act
            var mainWindowViewModel = new MainWindowViewModel(wrapper)
            {
                IsDataValid = false
            };

            var actual = mainWindowViewModel.IsDataValid;

            // Assert
            Assert.IsFalse(actual);
        }

        #endregion

        #region Command Tests

        [TestCase(TestName = "При вызове команды BuildModelCommand она должна " +
            "содержать обработчик")]
        public void TestBuildModelCommand_CanExecute()
        {
            // Arrange
            var wrapper = CreateMock();

            // Act
            var mainWindowViewModel = new MainWindowViewModel(wrapper);

            // Assert
            Assert.IsTrue(mainWindowViewModel.BuildModelCommand
                .CanExecute(null));
        }

        [TestCase(nameof(MainWindowViewModel.SetMinimumParametersCommand),
            TestName = TestSetCommands_CanExecute_TestName)]
        [TestCase(nameof(MainWindowViewModel.SetAverageParametersCommand),
            TestName = TestSetCommands_CanExecute_TestName)]
        [TestCase(nameof(MainWindowViewModel.SetMaximumParametersCommand),
            TestName = TestSetCommands_CanExecute_TestName)]
        public void TestSetCommands_CanExecute(string commandName)
        {
            // Arrange
            var wrapper = CreateMock();

            // Act
            var mainWindowViewModel = new MainWindowViewModel(wrapper);

            var commandProperty = typeof(MainWindowViewModel)
                .GetProperty(commandName);

            var canExecute = commandProperty != null &&
                ((RelayCommand)commandProperty.GetValue(mainWindowViewModel))
                .CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute);
        }

        [TestCaseSource(nameof(SetCommandsTestCases))]
        public void TestSetCommands_Execute(string commandName, IList<int> expected,
            Func<DeskParameter, int> getParameterAcceptableLimit)
        {
            // Arrange
            var wrapper = CreateMock();

            // Arrange
            var mainWindowViewModel = new MainWindowViewModel(wrapper);

            // Act
            var commandProperty = typeof(MainWindowViewModel)
                .GetProperty(commandName);

            if (commandProperty != null)
            {
                ((RelayCommand)commandProperty.GetValue(mainWindowViewModel))
                    .Execute(mainWindowViewModel.Parameters);
            }

            var actual = new List<int>();

            mainWindowViewModel.Parameters.ParametersByGroup.Values
                .ToList()
                .ForEach(parameters => actual
                    .AddRange(parameters
                        .Select(getParameterAcceptableLimit)));

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion

        #region Method Tests

        [TestCase(TestName = "Если все параметры содержат корректные данные, " +
            "свойство IsDataValid должно принимать значение true")]
        public void TestOnDataValidChanged_ValidData_SetsTrue()
        {
            // Arrange
            var wrapper = CreateMock();

            // Act
            var mainWindowViewModel = new MainWindowViewModel(wrapper);

            mainWindowViewModel.Parameters[DeskParameterGroupType.Worktop,
                DeskParameterType.WorktopHeight].Value = 32;
            mainWindowViewModel.Parameters[DeskParameterGroupType.Legs,
                    DeskParameterType.LegHeight].Value = 705;
            var isValid = mainWindowViewModel.IsDataValid;

            // Assert
            Assert.IsTrue(isValid);
        }

        [TestCase(TestName = "Если хотя бы один из параметров содержит " +
            "некорректные данные, свойство IsDataValid должно принимать значение " +
            "false")]
        public void TestOnDataValidChanged_InvalidData_SetsFalse()
        {
            // Arrange
            var wrapper = CreateMock();

            // Act
            var mainWindowViewModel = new MainWindowViewModel(wrapper);

            mainWindowViewModel.Parameters[DeskParameterGroupType.Worktop,
                DeskParameterType.WorktopHeight].Value = 2000;
            var isValid = mainWindowViewModel.IsDataValid;

            // Assert
            Assert.IsFalse(isValid);
        }

        #endregion

        #region Test Case Sources

        /// <summary>
        /// Метод-источник тестовых случаев для тестирования команд,
        /// устанавливающих значения параметров по умолчанию.
        /// </summary>
        /// <returns> Перечисление тестовых случаев <see cref="TestCaseData"/>.
        /// </returns>
        private static IEnumerable<TestCaseData> SetCommandsTestCases()
        {
            yield return new TestCaseData(
                nameof(MainWindowViewModel.SetMinimumParametersCommand),
                new List<int> { 800, 400, 30, 50, 690, 3, 250 },
                (Func<DeskParameter, int>)(parameter => parameter.Min))
                .SetName("При вызове команды {0} должны быть установлены " +
                    "минимальные значения всех параметров");

            yield return new TestCaseData(
                nameof(MainWindowViewModel.SetAverageParametersCommand),
                new List<int> { 1000, 625, 35, 60, 715, 4, 291 },
                (Func<DeskParameter, int>)(parameter => (parameter.Min +
                    parameter.Max) / 2))
                .SetName("При вызове команды {0} должны быть установлены " +
                    "средние значения всех параметров");

            yield return new TestCaseData(
                nameof(MainWindowViewModel.SetMaximumParametersCommand),
                new List<int> { 1200, 750, 40, 70, 740, 5, 400 },
                (Func<DeskParameter, int>)(parameter => parameter.Max))
                .SetName("При вызове команды {0} должны быть установлены " +
                    "максимальные значения всех параметров");
        }

        #endregion
    }
}