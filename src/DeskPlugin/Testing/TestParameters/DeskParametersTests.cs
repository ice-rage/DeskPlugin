using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Parameters;
using Parameters.Enums;

namespace TestParameters
{
    /// <summary>
    /// Класс, содержащий модульные тесты для класса <see cref="DeskParameters"/>.
    /// </summary>
    [TestFixture]
    public class DeskParametersTests
    {
        #region Test Data Sources

        #region Constants

        /// <summary>
        /// Название модульных тестов для статических свойств класса
        /// <see cref="DeskParameters"/>.
        /// </summary>
        private const string TestStaticProperties_GoodScenario_TestName =
            "Позитивный тест геттера статического свойства {0}";

        #endregion

        #region Fields

        /// <summary>
        /// Тестовый словарь, содержащий группы параметров и соответствующие им
        /// коллекции параметров.
        /// </summary>
        private readonly Dictionary<DeskParameterGroupType,
                ObservableCollection<DeskParameter>>
            _testParametersByGroup = new Dictionary<DeskParameterGroupType,
                ObservableCollection<DeskParameter>>(
                    new Dictionary<DeskParameterGroupType, 
                        ObservableCollection<DeskParameter>>
            {
                {
                    DeskParameterGroupType.Worktop,
                    new ObservableCollection<DeskParameter>
                    {
                        new DeskParameter(DeskParameterType.WorktopLength,
                            800, 1200, 1000),
                        new DeskParameter(DeskParameterType.WorktopWidth,
                            500, 750, 625),
                        new DeskParameter(DeskParameterType.WorktopHeight,
                            30, 40, 35)
                    }
                },
                {
                    DeskParameterGroupType.Legs,
                    new ObservableCollection<DeskParameter>
                    {
                        new DeskParameter(DeskParameterType.LegBaseDiameter,
                            50, 70, 60),
                        new DeskParameter(DeskParameterType.LegHeight, 690,
                            740, 715)
                    }
                },
                {
                    DeskParameterGroupType.Drawers,
                    new ObservableCollection<DeskParameter>
                    {
                        new DeskParameter(
                            DeskParameterType.DrawerGripHandleFastenerDistance,
                        80, 100, 90),
                        new DeskParameter(DeskParameterType.DrawerNumber, 3,
                        5, 4),
                        new DeskParameter(DeskParameterType.DrawerLength, 250,
                        333, 291)
                    }
                }
            });

        #endregion

        #endregion

        #region Property Tests

        #region Static Property Tests

        [TestCaseSource(nameof(StaticPropertyTestCases))]
        public void TestStaticPropertiesGet_GoodScenario(string staticPropertyName, 
            int expected)
        {
            // Act
            var actual = typeof(DeskParameters)
                .GetProperty(staticPropertyName)?
                .GetValue(null, null);

            // Arrange
            if (actual is int)
            {
                Assert.AreEqual(expected, actual);
            }
        }

        #endregion

        [TestCase(TestName = "Позитивный тест геттера ParametersByGroup")]
        public void TestParametersByGroupGet_GoodScenario()
        {
            // Arrange
            var expected = _testParametersByGroup;

            // Act
            var parameters = new DeskParameters();
            var actual = 
                parameters.ParametersByGroup;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(LegType.Square, 
            TestName = "Позитивный тест геттера и сеттера LegType")]
        public void TestLegTypeGetSet_GoodScenario(LegType expected)
        {
            // Act
            var parameters = new DeskParameters
            {
                LegType = LegType.Square
            };

            var actual = parameters.LegType;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Method Tests

        [TestCase(TestName = "При изменении значения длины столешницы должны " + 
            "измениться максимальное значение ширины столешницы и минимальное " +
            "значение длины ящиков для канцелярии")]
        public void TestOnWorktopLengthChanged_MinWorktopWidthMaxDrawerLengthIsChanged()
        {
            // Arrange
            var expectedMinWorktopWidth = 450;
            var expectedMaxDrawerLength = 300;

            // Act
            var parameters = new DeskParameters
            {
                [DeskParameterGroupType.Worktop, DeskParameterType.WorktopLength] =
                {
                    Value = 900
                }
            };

            var actualMinWorktopWidth = parameters[DeskParameterGroupType
                    .Worktop, DeskParameterType.WorktopWidth].Min;
            var actualMaxDrawerLength = parameters[DeskParameterGroupType
                    .Drawers, DeskParameterType.DrawerLength].Max;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedMinWorktopWidth, actualMinWorktopWidth);
                Assert.AreEqual(expectedMaxDrawerLength, actualMaxDrawerLength);
            });
        }

        #region Indexer Tests

        [TestCase(TestName = "Позитивный тест геттера индексатора " + 
            "this[DeskParameterGroupType index]")]
        public void TestIndexerGet_ReturnsKeyValuePair()
        {
            // Arrange
            var parameters = new DeskParameters();

            var expected = new KeyValuePair<DeskParameterGroupType,
                ObservableCollection<DeskParameter>>(DeskParameterGroupType.Drawers,
                new ObservableCollection<DeskParameter>
                {
                    new DeskParameter(
                        DeskParameterType.DrawerGripHandleFastenerDistance,
                        80, 100, 90),
                    new DeskParameter(DeskParameterType.DrawerNumber, 3,
                        5, 4),
                    new DeskParameter(DeskParameterType.DrawerLength, 250,
                        333, 291)
                });

            // Act
            var actual = 
                parameters[DeskParameterGroupType.Drawers];

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(TestName = "Позитивный тест геттера и сеттера индексатора " + 
            "this[DeskParameterGroupType firstIndex, DeskParameterType " +
            "secondIndex]")]
        public void TestIndexerGetSet_SetsReturnsDeskParameter()
        {
            // Arrange
            var expected = new DeskParameter(DeskParameterType.LegHeight, 
                690, 740, 715);

            // Act
            var parameters = new DeskParameters
            {
                [DeskParameterGroupType.Legs, DeskParameterType.LegHeight] = 
                    expected
            };

            var actual = parameters[DeskParameterGroupType.Legs, 
                DeskParameterType.LegHeight];

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #endregion

        #region Test Case Sources

        /// <summary>
        /// Метод-источник тестовых случаев для тестирования статических свойств
        /// класса <see cref="DeskParameters"/>.
        /// </summary>
        /// <returns> Перечисление тестовых случаев <see cref="TestCaseData"/>.
        /// </returns>
        private static IEnumerable<TestCaseData> StaticPropertyTestCases()
        {
            yield return new TestCaseData(
                nameof(DeskParameters.DistanceFromWorktopCornerToLeg), 20)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DeskParameters.OuterInnerDrawerLengthDifference), 40)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DeskParameters.OuterInnerDrawerWidthDifference), 40)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DeskParameters.OuterInnerDrawerHeightDifference), 20)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DeskParameters.DrawerDoorLengthDifference), 40)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(nameof(DeskParameters.DoorWidth), 20)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DeskParameters.DrawerDoorHeightDifference), 20)
                .SetName(TestStaticProperties_GoodScenario_TestName);
        }

        #endregion
    }
}
