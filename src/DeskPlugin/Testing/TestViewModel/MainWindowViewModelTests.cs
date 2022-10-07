using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.Input;
using NUnit.Framework;
using Parameters;
using Parameters.Enums;
using ViewModel;

namespace TestViewModel
{
	// TODO: Полетела кодировка
	/// <summary>
	/// �����, ���������� ��������� ����� ��� ������ <see cref="MainWindowViewModel"/>.
	/// </summary>
	[TestFixture]
    public class MainWindowViewModelTests
    {
        #region Constants For Testing

        /// <summary>
        /// �������� ��������� ������ ��� ������, ��������������� �������� ���������� �� ���������.
        /// </summary>
        private const string TestSetCommands_CanExecute_TestName = "��� ������ ������� {0} " + 
            "��� ������ ��������� ����������";

        #endregion

        #region Property Tests

        [TestCase(TestName = "���������� ���� ������� Parameters")]
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

        [TestCase(TestName = "���������� ���� ������� � ������� IsDataValid")]
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

        #region Command Tests

        [TestCase(TestName = "��� ������ ������� BuildModelCommand ��� ������ ��������� " + 
            "����������")]
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
            Debug.Assert(commandProperty != null, nameof(commandProperty) + " != null");

            bool canExecute = ((RelayCommand)commandProperty.GetValue(mainWindowViewModel))
                .CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute);
        }

        [TestCaseSource(nameof(SetCommandsTestCases))]
        public void TestSetCommands_Execute(string commandName, IList<int> expected, 
            Func<DeskParameter, int> getParameterAcceptableLimit)
        {
            // Arrange
            var mainWindowViewModel = new MainWindowViewModel();

            // Act
            PropertyInfo commandProperty = typeof(MainWindowViewModel).GetProperty(commandName);
            Debug.Assert(commandProperty != null, nameof(commandProperty) + " != null");

            ((RelayCommand)commandProperty.GetValue(mainWindowViewModel))
                .Execute(mainWindowViewModel.Parameters);

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

        [TestCase(TestName = "���� ��� ��������� �������� ���������� ������, �������� IsDataValid" + 
            "������ ��������� �������� true")]
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

        [TestCase(TestName = "���� ���� �� ���� �� ���������� �������� ������������ ������, " + 
            "�������� IsDataValid ������ ��������� �������� false")]
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
        /// �����-�������� �������� ������� ��� ������������ ������, ��������������� ��������
        /// ���������� �� ���������.
        /// </summary>
        /// <returns> ������������ �������� ������� <see cref="TestCaseData"/>.</returns>
        private static IEnumerable<TestCaseData> SetCommandsTestCases()
        {
            yield return new TestCaseData(nameof(MainWindowViewModel.SetMinimumParametersCommand),
                new List<int> { 800, 400, 30, 50, 690, 3, 250 }, (Func<DeskParameter, int>)
                (parameter => parameter.Min))
                .SetName("��� ������ ������� {0} ������ ���� ����������� ����������� �������� " +
                         "���� ����������");

            yield return new TestCaseData(nameof(MainWindowViewModel.SetAverageParametersCommand),
                new List<int> { 1000, 625, 35, 60, 715, 4, 291 }, (Func<DeskParameter, int>)
                (parameter => (parameter.Min + parameter.Max) / 2))
                .SetName("��� ������ ������� {0} ������ ���� ����������� ������� �������� " +
                         "���� ����������");

            yield return new TestCaseData(nameof(MainWindowViewModel.SetMaximumParametersCommand),
                new List<int> { 1200, 750, 40, 70, 740, 5, 400 }, (Func<DeskParameter, int>)
                (parameter => parameter.Max))
                .SetName("��� ������ ������� {0} ������ ���� ����������� ������������ �������� " +
                         "���� ����������");
        }

        #endregion
    }
}