using System.Collections.Generic;
using NUnit.Framework;
using Parameters.Static;

namespace ParameterTests.StaticClassTests
{
    /// <summary>
    /// Класс, содержащий модульные тесты для класса
    /// <see cref="RailingHandleDimensions"/>.
    /// </summary>
    public class RailingHandleDimensionsTests
    {
        #region Test Data Sources

        #region Constants

        /// <summary>
        /// Название модульных тестов для статических свойств тестируемого класса.
        /// </summary>
        protected const string TestStaticProperties_GoodScenario_TestName =
            "Позитивный тест геттера статического свойства {0}";

        #endregion

        #endregion

        #region Static Property Tests

        [TestCaseSource(nameof(StaticPropertyTestCases))]
        public void TestStaticPropertiesGet_GoodScenario(string staticPropertyName,
            int expected)
        {
            // Act
            var actual = typeof(RailingHandleDimensions)
                .GetProperty(staticPropertyName)?
                .GetValue(null, null);

            // Arrange
            if (actual is int)
            {
                Assert.AreEqual(expected, actual);
            }
        }

        #endregion

        #region Test Case Sources

        /// <summary>
        /// Метод-источник тестовых случаев для тестирования статических свойств
        /// класса <see cref="RailingHandleDimensions"/>.
        /// </summary>
        /// <returns> Перечисление тестовых случаев <see cref="TestCaseData"/>.
        /// </returns>
        private static IEnumerable<TestCaseData> StaticPropertyTestCases()
        {
            yield return new TestCaseData(
                nameof(RailingHandleDimensions.CrossbeamFastenersLengthDifference),
                60).SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(RailingHandleDimensions.CrossbeamDiameter), 25)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(RailingHandleDimensions.CrossbeamRotationAngleInDegrees), 90)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(RailingHandleDimensions.FasteningLegDiameter), 20)
                .SetName(TestStaticProperties_GoodScenario_TestName);
        }

        #endregion
    }
}
