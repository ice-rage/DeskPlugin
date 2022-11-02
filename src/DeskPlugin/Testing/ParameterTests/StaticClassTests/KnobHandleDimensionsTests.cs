using System.Collections.Generic;
using NUnit.Framework;
using Parameters.Static;

namespace ParameterTests.StaticClassTests
{
    /// <summary>
    /// Класс, содержащий модульные тесты для класса
    /// <see cref="RailingHandleDimensions"/>.
    /// </summary>
    public class KnobHandleDimensionsTests
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

        #region Property Tests

        #region Static Property Tests

        [TestCaseSource(nameof(StaticPropertyTestCases))]
        public void TestStaticPropertiesGet_GoodScenario(string staticPropertyName,
            int expected)
        {
            // Act
            var actual = typeof(KnobHandleDimensions)
                .GetProperty(staticPropertyName)?
                .GetValue(null, null);

            // Arrange
            if (actual is int)
            {
                Assert.AreEqual(expected, actual);
            }
        }

        #endregion

        #endregion

        #region Test Case Sources

        /// <summary>
        /// Метод-источник тестовых случаев для тестирования статических свойств
        /// класса <see cref="KnobHandleDimensions"/>.
        /// </summary>
        /// <returns> Перечисление тестовых случаев <see cref="TestCaseData"/>.
        /// </returns>
        private static IEnumerable<TestCaseData> StaticPropertyTestCases()
        {
            yield return new TestCaseData(nameof(KnobHandleDimensions.HandleBaseWidth), 
                10).SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(KnobHandleDimensions.HandleBaseFilletEdgeRadius), 10)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(KnobHandleDimensions.HandleBaseFilletEdgeStartSetback),
                1).SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(KnobHandleDimensions.HandleBaseFilletEdgeEndSetback), 5)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(nameof(KnobHandleDimensions.HandleLegWidth), 
                30).SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(KnobHandleDimensions.HandleLegBulgeAngleInDegrees), 15)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(KnobHandleDimensions.HandleCapBaseWidth), 5)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(KnobHandleDimensions.HandleCapBaseFilletEdgeRadius), 10)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(KnobHandleDimensions.HandleCapBaseFilletEdgeStartSetback),
                1).SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(KnobHandleDimensions.HandleCapBaseFilletEdgeEndSetback),
                5).SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(KnobHandleDimensions.HandleCapBaseHandleBaseDiameterDifference),
                20).SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(KnobHandleDimensions.HandleCapHandleBaseDiameterDifference),
                10).SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(nameof(KnobHandleDimensions.HandleCapWidth),
                20).SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(KnobHandleDimensions.HandleCapFilletEdgeRadius), 20)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(KnobHandleDimensions.HandleCapFilletEdgeStartSetback), 5)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(KnobHandleDimensions.HandleCapFilletEdgeEndSetback), 10)
                .SetName(TestStaticProperties_GoodScenario_TestName);
        }

        #endregion
    }
}
