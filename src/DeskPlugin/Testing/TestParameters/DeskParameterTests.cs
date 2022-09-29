using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Parameters;
using Parameters.Enums;
using Parameters.Enums.Extensions;

namespace TestParameters
{
    /// <summary>
    /// Класс, содержащий модульные тесты для класса <see cref="DeskParameter"/>.
    /// </summary>
    [TestFixture]
    public class DeskParameterTests
    {
        #region Constants

        /// <summary>
        /// Тестовый параметр.
        /// </summary>
        private readonly DeskParameter _testParameter = new DeskParameter(DeskParameterType
            .WorktopLength, 800, 1200, 1000);

        /// <summary>
        /// Название модульного теста для конструктора.
        /// </summary>
        private const string TestConstructor_CheckAcceptableRange_ReturnsValue_TestName =
            "При вызове конструктора для параметра {0} с ограничениями от {1} до {2} строка " +
            "с ограничениями равна {4}";

        /// <summary>
        /// Ожидаемое сообщение об ошибке, когда значение параметра меньше допустимого минимума.
        /// </summary>
        private const string TestGetErrors_ValueLessThanMin_ErrorMessage =
            "Parameter \"Length (L1)\" must be greater than or equal to 800";

        /// <summary>
        /// Ожидаемое сообщение об ошибке, когда значение параметра больше допустимого максимума.
        /// </summary>
        private const string TestGetErrors_ValueGreaterThanMax_ErrorMessage =
            "Parameter \"Length (L1)\" must be less than or equal to 1200";

        #endregion

        #region Test Properties

        [TestCase(TestName = "Позитивный тест геттера Name")]
        public void TestNameGet_GoodScenario()
        {
            // Arrange
            var expected = DeskParameterType.WorktopLength;

            // Act
            DeskParameterType actual = _testParameter.Name;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(TestName = "Позитивный тест геттера Min")]
        public void TestMinGet_GoodScenario()
        {
            // Arrange
            var expected = 800;

            // Act
            int actual = _testParameter.Min;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(TestName = "Позитивный тест геттера Max")]
        public void TestMaxGet_GoodScenario()
        {
            // Arrange
            var expected = 1200;

            // Act
            int actual = _testParameter.Max;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(1000, TestName = "Позитивный тест геттера и сеттера Value с тем же значением " +
                                   "параметра")]
        [TestCase(1100, TestName = "Позитивный тест геттера и сеттера Value с новым значением " +
                                   "параметра")]
        public void TestValueGetSet_GoodScenario(int expected)
        {
            // Act
            if (!(_testParameter.Clone() is DeskParameter parameter))
            {
                return;
            }

            parameter.Value = expected;
            int actual = parameter.Value;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(TestName = "Позитивный тест геттера Description")]
        public void TestDescriptionGet_GoodScenario()
        {
            // Arrange
            string expected = DeskParameterType.WorktopLength.GetDescription();

            // Act
            string actual = _testParameter.Description;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(TestName = "Позитивный тест геттера AcceptableRange")]
        public void TestAcceptableRangeGet_GoodScenario()
        {
            // Arrange
            var expected = "(800-1200 mm)";

            // Act
            string actual = _testParameter.AcceptableRange;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(TestName = "Позитивный тест геттера HasErrors")]
        public void TestHasErrorsGet_GoodScenario()
        {
            // Act
            bool actual = _testParameter.HasErrors;

            // Assert
            Assert.IsFalse(actual);
        }

        #endregion

        #region Test Constructors

        [TestCase(DeskParameterType.WorktopLength, 1200, 800, 900, "Error",
            TestName = TestConstructor_CheckAcceptableRange_ReturnsValue_TestName)]
        [TestCase(DeskParameterType.WorktopLength, 1000, 1000, 900, "Error",
            TestName = TestConstructor_CheckAcceptableRange_ReturnsValue_TestName)]
        [TestCase(DeskParameterType.DrawerNumber, 3, 5, 4, "(3-5 pcs)",
            TestName = TestConstructor_CheckAcceptableRange_ReturnsValue_TestName)]
        [TestCase(DeskParameterType.WorktopLength, 800, 1200, 900, "(800-1200 mm)",
            TestName = TestConstructor_CheckAcceptableRange_ReturnsValue_TestName)]
        public void TestConstructor_CheckAcceptableRange_ReturnsValue(DeskParameterType name, 
            int min, int max, int value, string expected)
        {
            // Act
            var parameter = new DeskParameter(name, min, max, value);
            string actual = parameter.AcceptableRange;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Test Methods

        [TestCase(TestName = "При сравнении одинаковых объектов возвращается true")]
        public void TestEqualsAndClone_EqualValues_ReturnsTrue()
        {
            // Arrange
            DeskParameter expected = _testParameter;

            // Act
            if (!(_testParameter.Clone() is DeskParameter actual))
            {
                return;
            }

            bool isEqual = actual.Equals(expected);

            // Assert
            Assert.IsTrue(isEqual);
        }

        [TestCase(TestName = "При сравнении различных объектов возвращается false")]
        public void TestEquals_DifferentObjects_ReturnsFalse()
        {
            // Arrange
            DeskParameter expected = _testParameter;

            // Act
            if (!(_testParameter.Clone() is DeskParameter actual))
            {
                return;
            }

            actual.Value = 1172;
            bool isEqual = actual.Equals(expected);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [TestCase(TestName = "При сравнении с null возвращается false")]
        public void TestEquals_NullValue_ReturnsFalse()
        {
            // Act
            DeskParameter actual = _testParameter;
            bool isEqual = actual.Equals(null);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [TestCase(TestName = "Для одинаковых объектов возвращаются одинаковые хеш-коды")]
        public void TestGetHashCode_EqualObjects_EqualHashCodes()
        {
            // Arrange
            int expected = _testParameter.GetHashCode();

            // Act
            if (!(_testParameter.Clone() is DeskParameter parameter))
            {
                return;
            }

            int actual = parameter.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(TestName = "Для разных объектов возвращаются разные хеш-коды")]
        public void TestGetHashCode_DifferentObjects_DifferentHashCodes()
        {
            // Arrange
            int expected = _testParameter.GetHashCode();

            // Act
            if (!(_testParameter.Clone() is DeskParameter parameter))
            {
                return;
            }

            parameter.Value = 877;
            int actual = parameter.GetHashCode();

            // Assert
            Assert.AreNotEqual(expected, actual);
        }

        #region Test Validators

        [TestCase(null, 500, TestGetErrors_ValueLessThanMin_ErrorMessage)]
        [TestCase(null, 1500, TestGetErrors_ValueGreaterThanMax_ErrorMessage)]
        [TestCase(null, 1050, "")]
        [TestCase(nameof(DeskParameter.Max), 500, "")]
        public void TestGetErrors_ReturnsValue(string propertyName, int value, string expected)
        {
            // Act
            if (!(_testParameter.Clone() is DeskParameter parameters))
            {
                return;
            }

            parameters.Value = value;
            List<string> errors = parameters.GetErrors(propertyName).OfType<string>().ToList();

            // Assert
            if (errors.Any())
            {
                bool isContained = errors.Contains(expected);
                Assert.IsTrue(isContained);
            }
            else
            {
                var actual = string.Empty;
                Assert.AreEqual(expected, actual);
            }
        }

        #endregion

        #endregion
    }
}