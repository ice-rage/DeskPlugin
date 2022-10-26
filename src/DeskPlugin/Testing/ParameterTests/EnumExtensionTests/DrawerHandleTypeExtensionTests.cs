using NUnit.Framework;
using Parameters.Enums;
using Parameters.Enums.Extensions;

namespace ParameterTests.EnumExtensionTests
{
    /// <summary>
    /// Класс, содержащий модульные тесты для класса
    /// <see cref="DrawerHandleTypeExtensionTests"/>.
    /// </summary>
    public class DrawerHandleTypeExtensionTests
    {
        #region Test Data Sources

        #region Constants

        /// <summary>
        /// Название модульного теста для метода
        /// <see cref="DrawerHandleTypeExtension.GetHandleMutableParameterType"/>.
        /// </summary>
        private const string TestGetHandleMutableParameterType_ReturnsValue_TestName =
            "При вызове метода GetHandleMutableParameterType() для типа ручек {0} " +
            "должна возвращаться строка {1}";

        #endregion

        #endregion

        #region Method Tests

        [TestCase(DeskParameterType.DrawerRailingHandleFastenerDistance,
            DrawerHandleType.Railing,
            TestName = TestGetHandleMutableParameterType_ReturnsValue_TestName)]
        [TestCase(DeskParameterType.DrawerGripHandleFastenerDistance,
            DrawerHandleType.Grip,
            TestName = TestGetHandleMutableParameterType_ReturnsValue_TestName)]
        [TestCase(DeskParameterType.DrawerKnobHandleBaseDiameter,
            DrawerHandleType.Knob,
            TestName = TestGetHandleMutableParameterType_ReturnsValue_TestName)]
        public void TestGetLegBaseType_ReturnsValue(DeskParameterType expected,
            DrawerHandleType handleType)
        {
            // Act
            var actual = handleType.GetHandleMutableParameterType();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
