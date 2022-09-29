using NUnit.Framework;
using Parameters.Enums;
using Parameters.Enums.Extensions;

namespace TestParameters
{
    /// <summary>
    /// Класс, содержащий модульные тесты для класса <see cref="DeskParameterTypeExtension"/>.
    /// </summary>
    [TestFixture]
    public class DeskParameterTypeExtensionTests
    {
        /// <summary>
        /// Название модульного теста для метода
        /// <see cref="DeskParameterTypeExtension.GetDescription"/>.
        /// </summary>
        private const string TestGetDescription_ReturnsValue_TestName = "Когда вызывается метод " + 
            "GetDescription() для параметра {0}, он должен вернуть строку {1}";

        [TestCase(DeskParameterType.WorktopLength, "Length (L1)",
            TestName = TestGetDescription_ReturnsValue_TestName)]
        [TestCase(DeskParameterType.WorktopWidth, "Width (B)",
            TestName = TestGetDescription_ReturnsValue_TestName)]
        [TestCase(DeskParameterType.WorktopHeight, "Height (H1)",
            TestName = TestGetDescription_ReturnsValue_TestName)]
        [TestCase(DeskParameterType.LegBaseDiameter, "Base Diameter (D)",
            TestName = TestGetDescription_ReturnsValue_TestName)]
        [TestCase(DeskParameterType.LegBaseLength, "Base Length (A)",
            TestName = TestGetDescription_ReturnsValue_TestName)]
        [TestCase(DeskParameterType.LegHeight, "Height (H2)",
            TestName = TestGetDescription_ReturnsValue_TestName)]
        [TestCase(DeskParameterType.DrawerNumber, "Number",
            TestName = TestGetDescription_ReturnsValue_TestName)]
        [TestCase(DeskParameterType.DrawerLength, "Length (L2)",
            TestName = TestGetDescription_ReturnsValue_TestName)]
        public void TestGetDescription_ReturnsValue(DeskParameterType parameterType, 
            string expected)
        {
            // Act
            string actual = parameterType.GetDescription();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
