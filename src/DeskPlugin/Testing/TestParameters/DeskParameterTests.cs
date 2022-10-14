using System.Linq;
using NUnit.Framework;
using Parameters;
using Parameters.Enums;

namespace TestParameters
{
    // TODO: Полетела кодировка
    /// <summary>
    /// Класс, содержащий модульные тесты для класса <see cref="DeskParameter"/>.
    /// </summary>
    [TestFixture]
    public class DeskParameterTests
    {
        #region Test Data Sources

        #region Constants

        /// <summary>
        /// Название модульного теста для конструктора.
        /// </summary>
        private const string TestConstructor_CheckAcceptableRange_ReturnsValue_TestName =
            "При вызове конструктора для параметра {0} с ограничениями от {1} до " +
            "{2} строка с ограничениями равна {4}";

        /// <summary>
        /// Ожидаемое сообщение об ошибке, когда значение параметра меньше
        /// допустимого минимума.
        /// </summary>
        private const string TestGetErrors_ValueLessThanMin_ErrorMessage =
            "Parameter \"Length (L1)\" must be greater than or equal to 800";

        /// <summary>
        /// Ожидаемое сообщение об ошибке, когда значение параметра больше
        /// допустимого максимума.
        /// </summary>
        private const string TestGetErrors_ValueGreaterThanMax_ErrorMessage =
            "Parameter \"Length (L1)\" must be less than or equal to 1200";

        #endregion

        #region Fields

        /// <summary>
        /// Тестовый параметр.
        /// </summary>
        private readonly DeskParameter _testParameter =
            new DeskParameter(DeskParameterType.WorktopLength, 800, 1200,
                1000);

        #endregion

        #endregion

        #region Property Tests

        [TestCase(TestName = "Позитивный тест геттера Name")]
        public void TestNameGet_GoodScenario()
        {
            // Arrange
            var expected = DeskParameterType.WorktopLength;

            // Act
            var actual = _testParameter.Name;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(TestName = "Позитивный тест геттера Min")]
        public void TestMinGet_GoodScenario()
        {
            // Arrange
            var expected = 800;

            // Act
            var actual = _testParameter.Min;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(TestName = "Позитивный тест геттера Max")]
        public void TestMaxGet_GoodScenario()
        {
            // Arrange
            var expected = 1200;

            // Act
            var actual = _testParameter.Max;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(1000, TestName = "Позитивный тест геттера и сеттера Value " + 
            "с тем же значением параметра")]
        [TestCase(1100, TestName = "Позитивный тест геттера и сеттера Value " + 
            "с новым значением параметра")]
        public void TestValueGetSet_GoodScenario(int expected)
        {
            // Act
            var parameter = (DeskParameter)_testParameter.Clone();
            parameter.Value = expected;
            var actual = parameter.Value;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(TestName = "Позитивный тест геттера Description")]
        public void TestDescriptionGet_GoodScenario()
        {
            // Arrange
            var expected = "Length (L1)";

            // Act
            var actual = _testParameter.Description;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(TestName = "Позитивный тест геттера AcceptableRange")]
        public void TestAcceptableRangeGet_GoodScenario()
        {
            // Arrange
            var expected = "(800-1200 mm)";

            // Act
            var actual = _testParameter.AcceptableRange;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(TestName = "Позитивный тест геттера HasErrors")]
        public void TestHasErrorsGet_GoodScenario()
        {
            // Act
            var actual = _testParameter.HasErrors;

            // Assert
            Assert.IsFalse(actual);
        }

        #endregion

        #region Constructor Tests

        [TestCase(DeskParameterType.WorktopLength, 1200, 800, 900, 
            "Error", TestName = 
                TestConstructor_CheckAcceptableRange_ReturnsValue_TestName)]
        [TestCase(DeskParameterType.WorktopLength, 1000, 1000, 
            900, "Error", TestName = 
                TestConstructor_CheckAcceptableRange_ReturnsValue_TestName)]
        [TestCase(DeskParameterType.DrawerNumber, 3, 5, 4, 
            "(3-5 pcs)", TestName = 
                TestConstructor_CheckAcceptableRange_ReturnsValue_TestName)]
        [TestCase(DeskParameterType.WorktopLength, 800, 1200, 900, 
            "(800-1200 mm)", TestName = 
                TestConstructor_CheckAcceptableRange_ReturnsValue_TestName)]
        public void TestConstructor_CheckAcceptableRange_ReturnsValue(
            DeskParameterType name, int min, int max, int value, string expected)
        {
            // Act
            var parameter = new DeskParameter(name, min, max, value);
            var actual = parameter.AcceptableRange;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Method Tests

        [TestCase(TestName = "При сравнении одинаковых объектов возвращается true")]
        public void TestEquals_EqualObjects_ReturnsTrue()
        {
            // Arrange
            var expected = _testParameter;

            // Act
            var actual = (DeskParameter)expected.Clone();
            var isEqual = actual.Equals(expected);

            // Assert
            Assert.IsTrue(isEqual);
        }

        [TestCase(TestName = "При сравнении различных объектов возвращается false")]
        public void TestEquals_DifferentObjects_ReturnsFalse()
        {
            // Arrange
            var expected = _testParameter;

            // Act
            var actual = (DeskParameter)expected.Clone();
            actual.Value = 1172;
            var isEqual = actual.Equals(expected);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [TestCase(TestName = "При сравнении объекта с null возвращается false")]
        public void TestEquals_NullObject_ReturnsFalse()
        {
            // Act
            var actual = _testParameter;
            var isEqual = actual.Equals(null);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [TestCase(TestName = "Для одинаковых объектов возвращаются одинаковые " + 
            "хеш-коды")]
        public void TestGetHashCode_EqualObjects_EqualHashCodes()
        {
            // Arrange
            var parameter = _testParameter;
            var expected = parameter.GetHashCode();

            // Act
            var equalParameter = (DeskParameter)parameter.Clone();
            var actual = equalParameter.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(TestName = "Для разных объектов возвращаются разные хеш-коды")]
        public void TestGetHashCode_DifferentObjects_DifferentHashCodes()
        {
            // Arrange
            var parameter = _testParameter;
            var expected = parameter.GetHashCode();

            // Act
            var differentParameter = (DeskParameter)parameter.Clone();
            differentParameter.Value = 877;
            var actual = differentParameter.GetHashCode();

            // Assert
            Assert.AreNotEqual(expected, actual);
        }

        #region Validator Tests

        [TestCase(null, 500, 
            TestGetErrors_ValueLessThanMin_ErrorMessage)]
        [TestCase(null, 1500, 
            TestGetErrors_ValueGreaterThanMax_ErrorMessage)]
        [TestCase(null, 1050, "")]
        [TestCase(nameof(DeskParameter.Max), 500, "")]
        public void TestGetErrors_ReturnsValue(string propertyName, int value, 
            string expected)
        {
            // Act
            var parameter = (DeskParameter)_testParameter.Clone();

            parameter.Value = value;
            var validationErrors = parameter
                .GetErrors(propertyName)
                .OfType<string>()
                .ToList();

            // Assert
            if (validationErrors.Any())
            {
                var isContained = validationErrors.Contains(expected);
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