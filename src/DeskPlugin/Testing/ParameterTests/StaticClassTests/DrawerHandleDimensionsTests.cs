using System.Collections.Generic;
using NUnit.Framework;
using Parameters.Static;

namespace ParameterTests.StaticClassTests
{
    /// <summary>
    /// Класс, содержащий модульные тесты для класса
    /// <see cref="DrawerHandleDimensions"/>.
    /// </summary>
    public class DrawerHandleDimensionsTests
    {
        #region Test Data Sources

        #region Constants

        /// <summary>
        /// Название модульных тестов для статических свойств класса
        /// <see cref="DrawerHandleDimensions"/>.
        /// </summary>
        private const string TestStaticProperties_GoodScenario_TestName =
            "Позитивный тест геттера статического свойства {0}";

        #endregion

        #endregion

        #region Static Property Tests

        [TestCaseSource(nameof(StaticPropertyTestCases))]
        public void TestStaticPropertiesGet_GoodScenario(string staticPropertyName,
            int expected)
        {
            // Act
            var actual = typeof(DrawerHandleDimensions)
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
        /// класса <see cref="DrawerHandleDimensions"/>.
        /// </summary>
        /// <returns> Перечисление тестовых случаев <see cref="TestCaseData"/>.
        /// </returns>
        private static IEnumerable<TestCaseData> StaticPropertyTestCases()
        {
            yield return new TestCaseData(
                nameof(DrawerHandleDimensions.HandleHeight), 20)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DrawerHandleDimensions.RailingCrossbeamFastenersLengthDifference),
                60).SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DrawerHandleDimensions.RailingCrossbeamDiameter), 25)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DrawerHandleDimensions.RailingCrossbeamRotationAngleInDegrees),
                90).SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DrawerHandleDimensions.RailingFasteningLegDiameter), 20)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DrawerHandleDimensions.GripOuterInnerHandleLengthDifference),
                40).SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DrawerHandleDimensions.KnobHandleBaseWidth), 10)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DrawerHandleDimensions.KnobHandleBaseFilletEdgeRadius), 10)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DrawerHandleDimensions.KnobHandleBaseFilletEdgeStartSetback),
                1).SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DrawerHandleDimensions.KnobHandleBaseFilletEdgeEndSetback), 5)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DrawerHandleDimensions.KnobHandleLegWidth), 30)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DrawerHandleDimensions.KnobHandleLegBulgeAngleInDegrees), 15)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DrawerHandleDimensions.KnobHandleCapBaseWidth), 5)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DrawerHandleDimensions.KnobHandleCapBaseFilletEdgeRadius), 10)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DrawerHandleDimensions.KnobHandleCapBaseFilletEdgeStartSetback),
                1).SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DrawerHandleDimensions.KnobHandleCapBaseFilletEdgeEndSetback),
                5).SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DrawerHandleDimensions.KnobHandleCapBaseHandleBaseDiameterDifference),
                20).SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DrawerHandleDimensions.KnobHandleCapHandleBaseDiameterDifference),
                10).SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DrawerHandleDimensions.KnobHandleCapWidth), 20)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DrawerHandleDimensions.KnobHandleCapFilletEdgeRadius), 20)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DrawerHandleDimensions.KnobHandleCapFilletEdgeStartSetback), 5)
                .SetName(TestStaticProperties_GoodScenario_TestName);

            yield return new TestCaseData(
                nameof(DrawerHandleDimensions.KnobHandleCapFilletEdgeEndSetback), 10)
                .SetName(TestStaticProperties_GoodScenario_TestName);
        }

        #endregion
    }
}
