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
        /// <summary>
        /// Название модульного теста для метода <see cref="LegTypeExtensionTests"/>.
        /// </summary>
        private const string TestGetLegBaseType_ReturnsValue_TestName = "Когда вызывается метод " + 
            "GetLegBaseType() для параметра {1}, он должен вернуть соответствующий параметр {0}";

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
    }
}
