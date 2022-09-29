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
        #region Constants

        /// <summary>
        /// Тестовый словарь, содержащий группы параметров и соответствующие им списки параметров.
        /// </summary>
        private readonly Dictionary<DeskParameterGroupType, ObservableCollection<DeskParameter>>
            _testParametersByGroup = new Dictionary<DeskParameterGroupType,
                ObservableCollection<DeskParameter>>
            {
                { 
                    DeskParameterGroupType.Worktop, new ObservableCollection<DeskParameter>
                    { 
                        new DeskParameter(DeskParameterType.WorktopLength, 800, 1200, 1000), 
                        new DeskParameter(DeskParameterType.WorktopWidth, 500, 750, 625), 
                        new DeskParameter(DeskParameterType.WorktopHeight, 30, 40, 35)
                    }
                },
                {
                    DeskParameterGroupType.Legs, new ObservableCollection<DeskParameter>
                    {
                        new DeskParameter(DeskParameterType.LegBaseDiameter, 50, 70, 60),
                        new DeskParameter(DeskParameterType.LegHeight, 690, 740, 715)
                    }
                },
                {
                    DeskParameterGroupType.Drawers, new ObservableCollection<DeskParameter>
                    {
                        new DeskParameter(DeskParameterType.DrawerNumber, 3, 5, 4),
                        new DeskParameter(DeskParameterType.DrawerLength, 250, 333, 291)
                    }
                }
            };

        
        /// <summary>
        /// Названия модульных тестов статических свойств класса <see cref="DeskParameters"/>.
        /// </summary>
        private const string TestStaticProperty_GoodScenario_TestName = "Позитивный тест геттера " + 
            "статического свойства {0}";

        #endregion

        #region Test Properties

        #region Test Static Properties

        [TestCaseSource(nameof(GetStaticPropertiesTestCases))]
        public void TestStaticPropertiesGet_GoodScenario(string staticPropertyName, int expected)
        {
            // Act
            object actual = typeof(DeskParameters).GetProperty(staticPropertyName)?.GetValue(null, null);

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
            Dictionary<DeskParameterGroupType, ObservableCollection<DeskParameter>> expected = 
                _testParametersByGroup;

            // Act
            var parameters = new DeskParameters();
            Dictionary<DeskParameterGroupType, ObservableCollection<DeskParameter>> actual =
                parameters.ParametersByGroup;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(LegType.Square, true, TestName = "Позитивный тест " + 
            "геттера и сеттера LegType при пустом словаре параметров")]
        [TestCase(LegType.Square, false, TestName = "Позитивный тест " + 
            "геттера и сеттера LegType в случае, когда словарь параметров содержит значения")]
        public void TestLegTypeGetSet_GoodScenario(LegType expected, 
            bool doParametersByGroupNeedToBeCleared)
        {
            // Act
            var parameters = new DeskParameters();

            if (doParametersByGroupNeedToBeCleared)
            {
                parameters.ParametersByGroup.Clear();
            }

            parameters.LegType = LegType.Square;
            LegType actual = parameters.LegType;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Test Constructors

        [TestCase(TestName = "Когда вызывается конструктор класса DeskParameters, словарь " + 
            "параметров должен заполниться значениями по умолчанию")]
        public void TestConstructor_CheckAcceptableRange_ReturnsValue()
        {
            // Arrange
            Dictionary<DeskParameterGroupType, ObservableCollection<DeskParameter>> expected = 
                _testParametersByGroup;

            // Act
            var parameters = new DeskParameters();
            Dictionary<DeskParameterGroupType, ObservableCollection<DeskParameter>> actual = 
                parameters.ParametersByGroup;

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion

        #region Test Methods

        [TestCase(TestName = "При изменении значения длины столешницы должны измениться" + 
            "максимальное значение ширины столешницы и минимальное значение длины ящиков для " + 
            "канцелярии")]
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

            int actualMinWorktopWidth = parameters[DeskParameterGroupType.Worktop, 
                DeskParameterType.WorktopWidth].Min;
            int actualMaxDrawerLength = parameters[DeskParameterGroupType.Drawers,
                DeskParameterType.DrawerLength].Max;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedMinWorktopWidth, actualMinWorktopWidth);
                Assert.AreEqual(expectedMaxDrawerLength, actualMaxDrawerLength);
            });
        }

        #region Test Indexers

        [TestCase(TestName = "Позитивный тест геттера и сеттера индексатора " + 
            "this[DeskParameterGroupType index]")]
        public void TestIndexerGet_ReturnsKeyValuePair()
        {
            // Arrange
            var parameters = new DeskParameters();

            var expected = new KeyValuePair<DeskParameterGroupType,
                ObservableCollection<DeskParameter>>(DeskParameterGroupType.Drawers,
                new ObservableCollection<DeskParameter>
                {
                    new DeskParameter(DeskParameterType.DrawerNumber, 3, 5, 4),
                    new DeskParameter(DeskParameterType.DrawerLength, 250, 333, 291)
                });

            // Act
            KeyValuePair<DeskParameterGroupType, ObservableCollection<DeskParameter>> actual = 
                parameters[DeskParameterGroupType.Drawers];

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(TestName = "Позитивный тест геттера и сеттера индексатора " + 
            "this[DeskParameterGroupType firstIndex, DeskParameterType secondIndex]")]
        public void TestIndexerGetSet_SetsReturnsDeskParameter()
        {
            // Arrange
            var expected = new DeskParameter(DeskParameterType.LegHeight, 690, 740, 715);

            // Act
            var parameters = new DeskParameters
            {
                [DeskParameterGroupType.Legs, DeskParameterType.LegHeight] = expected
            };

            DeskParameter actual = parameters[DeskParameterGroupType.Legs, 
                DeskParameterType.LegHeight];

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #endregion

        #region Test Case Sources

        /// <summary>
        /// Метод-источников тестовых случаев для тестирования статических свойств класса
        /// <see cref="DeskParameters"/>.
        /// </summary>
        /// <returns>Перечисление тестовых случаев <see cref="TestCaseData"/>.</returns>
        private static IEnumerable<TestCaseData> GetStaticPropertiesTestCases()
        {
            yield return new TestCaseData(nameof(DeskParameters.DistanceFromWorktopCorner), 20)
                .SetName(TestStaticProperty_GoodScenario_TestName);

            yield return new TestCaseData(nameof(DeskParameters.WorktopDrawerWidthDifference), 40)
                .SetName(TestStaticProperty_GoodScenario_TestName);

            yield return new TestCaseData(nameof(DeskParameters.OuterInnerDrawerLengthDifference), 
                40).SetName(TestStaticProperty_GoodScenario_TestName);

            yield return new TestCaseData(nameof(DeskParameters.OuterInnerDrawerWidthDifference), 
                40).SetName(TestStaticProperty_GoodScenario_TestName);

            yield return new TestCaseData(nameof(DeskParameters.OuterInnerDrawerHeightDifference), 
                20).SetName(TestStaticProperty_GoodScenario_TestName);

            yield return new TestCaseData(nameof(DeskParameters.DrawerDoorLengthDifference), 40)
                .SetName(TestStaticProperty_GoodScenario_TestName);

            yield return new TestCaseData(nameof(DeskParameters.DoorWidth), 20)
                .SetName(TestStaticProperty_GoodScenario_TestName);

            yield return new TestCaseData(nameof(DeskParameters.DrawerDoorHeightDifference), 20)
                .SetName(TestStaticProperty_GoodScenario_TestName);

            yield return new TestCaseData(nameof(DeskParameters.HandleHeight), 20)
                .SetName(TestStaticProperty_GoodScenario_TestName);

            yield return new TestCaseData(nameof(DeskParameters.OuterInnerHandleLengthDifference),
                40).SetName(TestStaticProperty_GoodScenario_TestName);

            yield return new TestCaseData(nameof(DeskParameters.OuterHandleWidth), 40)
                .SetName(TestStaticProperty_GoodScenario_TestName);

            yield return new TestCaseData(nameof(DeskParameters.InnerHandleWidth), 20)
                .SetName(TestStaticProperty_GoodScenario_TestName);
        }

        #endregion
    }
}
