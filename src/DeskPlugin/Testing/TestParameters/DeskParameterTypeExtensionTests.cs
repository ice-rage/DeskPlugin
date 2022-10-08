using System.Collections.Generic;
using NUnit.Framework;
using Parameters.Enums;
using Parameters.Enums.Extensions;

namespace TestParameters
{
    /// <summary>
    /// Класс, содержащий модульные тесты для класса
    /// <see cref="DeskParameterTypeExtension"/>.
    /// </summary>
    [TestFixture]
    public class DeskParameterTypeExtensionTests
    {
        #region Constants For Testing

        /// <summary>
        /// Название модульного теста для метода
        /// <see cref="DeskParameterTypeExtension.GetDescription"/>.
        /// </summary>
        private const string TestGetDescription_ReturnsValue_TestName = 
            "При вызове метода GetDescription() для параметра {0} должна " +
            "возвращаться строка {1}";

        #endregion

        #region Method Tests

        [TestCaseSource(nameof(GetDescriptionTestCases))]
        public void TestGetDescription_ReturnsValue(DeskParameterType parameterType, 
            string expected)
        {
            // Act
            var actual = parameterType.GetDescription();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Test Case Sources

        /// <summary>
        /// Метод-источник тестовых случаев для тестирования метода
        /// <see cref="DeskParameterTypeExtension.GetDescription"/>.
        /// </summary>
        /// <returns> Перечисление тестовых случаев <see cref="TestCaseData"/>.
        /// </returns>
        private static IEnumerable<TestCaseData> GetDescriptionTestCases()
        {
            yield return new TestCaseData(DeskParameterType.WorktopLength, 
                "Length (L1)").SetName(TestGetDescription_ReturnsValue_TestName);

            yield return new TestCaseData(DeskParameterType.WorktopWidth, 
                "Width (B)").SetName(TestGetDescription_ReturnsValue_TestName);

            yield return new TestCaseData(DeskParameterType.WorktopHeight, 
                "Height (H1)").SetName(TestGetDescription_ReturnsValue_TestName);

            yield return new TestCaseData(DeskParameterType.LegBaseDiameter, 
                "Base Diameter (D)")
                .SetName(TestGetDescription_ReturnsValue_TestName);

            yield return new TestCaseData(DeskParameterType.LegBaseLength, 
                "Base Length (A)")
                .SetName(TestGetDescription_ReturnsValue_TestName);

            yield return new TestCaseData(DeskParameterType.LegHeight, 
                "Height (H2)").SetName(TestGetDescription_ReturnsValue_TestName);

            yield return new TestCaseData(DeskParameterType.DrawerNumber, "Number")
                .SetName(TestGetDescription_ReturnsValue_TestName);

            yield return new TestCaseData(DeskParameterType.DrawerLength, 
                "Length (L2)").SetName(TestGetDescription_ReturnsValue_TestName);
        }

        #endregion
    }
}
