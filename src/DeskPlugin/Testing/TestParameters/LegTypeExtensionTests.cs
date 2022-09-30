using NUnit.Framework;
using Parameters.Enums;
using Parameters.Enums.Extensions;

namespace TestParameters
{
    /// <summary>
    /// Класс, содержащий модульные тесты для класса <see cref="LegTypeExtensionTests"/>.
    /// </summary>
    [TestFixture]
    public class LegTypeExtensionTests
    {
        #region Constants For Testing

        /// <summary>
        /// Название модульного теста для метода <see cref="LegTypeExtension.GetLegBaseType"/>.
        /// </summary>
        private const string TestGetLegBaseType_ReturnsValue_TestName = "Когда вызывается метод " + 
            "GetLegBaseType() для типа ножек {1}, он должен вернуть соответствующий параметр {0}";

        #endregion

        #region Method Tests

        [TestCase(DeskParameterType.LegBaseDiameter, LegType.Round, TestName =
            TestGetLegBaseType_ReturnsValue_TestName)]
        [TestCase(DeskParameterType.LegBaseLength, LegType.Square, TestName =
            TestGetLegBaseType_ReturnsValue_TestName)]
        public void TestGetLegBaseType_ReturnsValue(DeskParameterType expected, LegType legType)
        {
            // Act
            DeskParameterType actual = legType.GetLegBaseType();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
