using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.Input;
using NUnit.Framework;
using Parameters;
using Parameters.Enums;
using ViewModel;

namespace TestViewModel
{
    /// <summary>
    /// Класс, содержащий модульные тесты для класса <see cref="MainWindowViewModel"/>.
    /// </summary>
    [TestFixture]
    public class MainWindowViewModelTests
    {
        #region Constants

        private const string TestSetCommands_CanExecute_TestName = "При получении команды {0} " + 
            "она должна содержать обработчик";

        #endregion

        #region Test Properties

        [TestCase(TestName = "Позитивный тест геттера Parameters")]
        public void TestParametersGet_GoodScenario()
        {
            // Arrange
            var expected = new DeskParameters();

            // Act
            var mainWindowViewModel = new MainWindowViewModel();
            DeskParameters actual = mainWindowViewModel.Parameters;

            // Assert
            Assert.Multiple(() =>
            {
                CollectionAssert.AreEqual(expected.ParametersByGroup, actual.ParametersByGroup);
                Assert.AreEqual(expected.LegType, actual.LegType);
            });
        }

        [TestCase(TestName = "Позитивный тест геттера и сеттера IsDataValid")]
        public void TestIsDataValidGetSet_GoodScenario()
        {
            // Act
            var mainWindowViewModel = new MainWindowViewModel
            {
                IsDataValid = false
            };

            bool actual = mainWindowViewModel.IsDataValid;

            // Assert
            Assert.IsFalse(actual);
        }

        #endregion

        #region Test Commands

        [TestCase(TestName = "При получении команды BuildModelCommand она должна содержать " + 
            "обработчик")]
        public void TestBuildModelCommand_CanExecute()
        {
            // Act
            var mainWindowViewModel = new MainWindowViewModel();

            // Assert
            Assert.IsTrue(mainWindowViewModel.BuildModelCommand.CanExecute(null));
        }

        [TestCase(nameof(MainWindowViewModel.SetMinimumParametersCommand), TestName = 
            TestSetCommands_CanExecute_TestName)]
        [TestCase(nameof(MainWindowViewModel.SetAverageParametersCommand), TestName = 
            TestSetCommands_CanExecute_TestName)]
        [TestCase(nameof(MainWindowViewModel.SetMaximumParametersCommand), TestName = 
            TestSetCommands_CanExecute_TestName)]
        public void TestSetCommands_CanExecute(string commandName)
        {
            // Act
            var mainWindowViewModel = new MainWindowViewModel();

            PropertyInfo commandProperty = typeof(MainWindowViewModel).GetProperty(commandName);

            if (commandProperty == null)
            {
                return;
            }

            bool canExecute = ((RelayCommand)commandProperty.GetValue(mainWindowViewModel))
                .CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute);
        }

        [TestCaseSource(nameof(GetSetCommandsTestCases))]
        public void TestSetCommands_Execute(string commandName, IList<int> expected, 
            Func<DeskParameter, int> function)
        {
            // Arrange
            var mainWindowViewModel = new MainWindowViewModel();

            // Act
            PropertyInfo commandProperty = typeof(MainWindowViewModel).GetProperty(commandName);

            if (commandProperty == null)
            {
                return;
            }

            ((RelayCommand)commandProperty.GetValue(mainWindowViewModel))
                .Execute(mainWindowViewModel.Parameters);

            var actual = new List<int>();

            mainWindowViewModel.Parameters.ParametersByGroup.Values
                .ToList()
                .ForEach(parameters => actual
                    .AddRange(parameters
                        .Select(function)));

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion

        #region Test Methods

        [TestCase(TestName = "Если все параметры содержат корректные данные, в свойство " + 
            "IsDataValid класса MainWindowViewModel должно быть установлено значение true")]
        public void TestOnDataValidChanged_ValidData_SetsTrue()
        {
            // Act
            var mainWindowViewModel = new MainWindowViewModel();

            mainWindowViewModel.Parameters[DeskParameterGroupType.Worktop, 
                DeskParameterType.WorktopHeight].Value = 32;
            mainWindowViewModel.Parameters[DeskParameterGroupType.Legs, DeskParameterType.LegHeight]
                .Value = 705;
            bool isValid = mainWindowViewModel.IsDataValid;

            // Assert
            Assert.IsTrue(isValid);
        }

        [TestCase(TestName = "Если в один из параметров вводят некорректные данные, в свойство " + 
            "IsDataValid класса MainWindowViewModel должно быть установлено значение false")]
        public void TestOnDataValidChanged_InvalidData_SetsFalse()
        {
            // Act
            var mainWindowViewModel = new MainWindowViewModel();

            mainWindowViewModel.Parameters[DeskParameterGroupType.Worktop, 
                DeskParameterType.WorktopHeight].Value = 2000;
            bool isValid = mainWindowViewModel.IsDataValid;

            // Assert
            Assert.IsFalse(isValid);
        }

        #endregion

        #region Test Case Sources

        /// <summary>
        /// Метод-источников тестовых случаев для тестирования статических свойств класса
        /// <see cref="DeskParameters"/>.
        /// </summary>
        /// <returns>Перечисление тестовых случаев <see cref="TestCaseData"/>.</returns>
        private static IEnumerable<TestCaseData> GetSetCommandsTestCases()
        {
            yield return new TestCaseData(nameof(MainWindowViewModel.SetMinimumParametersCommand),
                new List<int> { 800, 400, 30, 50, 690, 3, 250 }, (Func<DeskParameter, int>)
                (parameter => parameter.Min))
                .SetName("При вызове команды {0} должны быть установлены минимальные значения " +
                         "всех параметров");

            yield return new TestCaseData(nameof(MainWindowViewModel.SetAverageParametersCommand),
                new List<int> { 1000, 625, 35, 60, 715, 4, 291 }, (Func<DeskParameter, int>)
                (parameter => (parameter.Min + parameter.Max) / 2))
                .SetName("При вызове команды {0} должны быть установлены средние значения " +
                         "всех параметров");

            yield return new TestCaseData(nameof(MainWindowViewModel.SetMaximumParametersCommand),
                new List<int> { 1200, 750, 40, 70, 740, 5, 400 }, (Func<DeskParameter, int>)
                (parameter => parameter.Max))
                .SetName("При вызове команды {0} должны быть установлены максимальные значения " +
                         "всех параметров");
        }

        #endregion
    }
}