using NUnit.Framework;
using Parameters.Enums;
using Parameters.Enums.Extensions;

namespace ParameterTests.EnumExtensionTests
{
    /// <summary>
    /// Класс, содержащий модульные тесты для класса
    /// <see cref="LegTypeExtensionTests"/>.
    /// </summary>
    [TestFixture]
    public class LegTypeExtensionTests
    {
        #region Test Data Sources

        #region Constants

        /// <summary>
        /// Название модульного теста для метода
        /// <see cref="LegTypeExtension.GetLegBaseType"/>.
        /// </summary>
        private const string TestGetLegBaseType_ReturnsValue_TestName =
            "При вызове метода GetLegBaseType() для типа ножек {0} должна " +
            "возвращаться строка {1}";

        #endregion

        #endregion

        #region Method Tests

        [TestCase(DeskParameterType.LegBaseDiameter, LegType.Round,
            TestName = TestGetLegBaseType_ReturnsValue_TestName)]
        [TestCase(DeskParameterType.LegBaseLength, LegType.Square,
            TestName = TestGetLegBaseType_ReturnsValue_TestName)]
        public void TestGetLegBaseType_ReturnsValue(DeskParameterType expected,
            LegType legType)
        {
            // Act
            var actual = legType.GetLegBaseType();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
