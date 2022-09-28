using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Parameters;
using Parameters.Enums;
using Parameters.Enums.Extensions;

namespace TestParameters
{
    [TestFixture]
    internal class DeskParameterTests
    {
        #region Constants

        private readonly DeskParameter _testParameter = new DeskParameter(DeskParameterType
            .WorktopLength, 800, 1200, 1000);

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

        [TestCase(1000, false, false, TestName = "Позитивный тест геттера " +
                                          "и сеттера Value с тем же значением параметра")]
        [TestCase(1100, true, true, TestName = "Позитивный тест геттера " +
                                         "и сеттера Value с новым значением параметра")]
        public void TestValueGetSet_GoodScenario(int expectedValue,
            bool isValueChangedExpectedToBeInvoked, bool isDataValidChangedExpectedToBeInvoked)
        {
            // Arrange
            var isValueChangedInvoked = false;
            var isDataValidChangedInvoked = false;

            // Act
            if (!(_testParameter.Clone() is DeskParameter parameter))
            {
                return;
            }

            parameter.ValueChanged += (s, e) => isValueChangedInvoked = true;
            parameter.DataValidChanged += (s, e) => isDataValidChangedInvoked = true;

            parameter.Value = expectedValue;
            int actual = parameter.Value;

            // Assert
            Assert.AreEqual(expectedValue, actual);
            Assert.AreEqual(isValueChangedExpectedToBeInvoked, isValueChangedInvoked);
            Assert.AreEqual(isDataValidChangedExpectedToBeInvoked, isDataValidChangedInvoked);
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

        private const string TestConstructor_CheckMinMaxAcceptableRange_ReturnsValue_TestName =
            "При вызове конструктора для параметра {0} с ограничениями от {1} до {2} строка " +
            "с ограничениями равна {4}";

        [TestCase(DeskParameterType.WorktopLength, 1200, 800, 900, "Error",
            TestName = TestConstructor_CheckMinMaxAcceptableRange_ReturnsValue_TestName)]
        [TestCase(DeskParameterType.WorktopLength, 1000, 1000, 900, "Error",
            TestName = TestConstructor_CheckMinMaxAcceptableRange_ReturnsValue_TestName)]
        [TestCase(DeskParameterType.DrawerNumber, 3, 5, 4, "(3-5 pcs)",
            TestName = TestConstructor_CheckMinMaxAcceptableRange_ReturnsValue_TestName)]
        [TestCase(DeskParameterType.WorktopLength, 800, 1200, 900, "(800-1200 mm)",
            TestName = TestConstructor_CheckMinMaxAcceptableRange_ReturnsValue_TestName)]
        public void TestConstructor_CheckMinMaxLimits_ReturnsValue(DeskParameterType name, int min,
            int max, int value, string expectedAcceptableRange)
        {
            // Act
            var parameter = new DeskParameter(name, min, max, value);
            string actual = parameter.AcceptableRange;

            // Assert
            Assert.AreEqual(expectedAcceptableRange, actual);
        }

        #endregion

        #region Test Validators

        private const string TestIndexerReturnsValueMinErrorMessage =
            "Parameter \"Length (L1)\" must be greater than or equal to 800";

        private const string TestIndexerReturnsValueMaxErrorMessage =
            "Parameter \"Length (L1)\" must be less than or equal to 1200";

        private const string TestIndexerReturnsValueTestName =
            "При вызове валидатора для значения {0} возвращается текст ошибки {1}";

        [TestCase(955, TestName = "При вызове валидатора для значения {0} возвращается пустое " +
                             "перечисление")]
        public void TestGetErrors_ValidValue_ReturnsEmptyEnumerable(int value)
        {
            // Act
            var parameter = new DeskParameter(DeskParameterType.WorktopLength, 800, 1200,
                value);
            IEnumerable<string> actual = parameter.GetErrors(nameof(parameter.Value)).Cast<string>();

            // Assert
            Assert.IsEmpty(actual);
        }

        [TestCase(500, TestIndexerReturnsValueMinErrorMessage, TestName =
            TestIndexerReturnsValueTestName)]
        [TestCase(1500, TestIndexerReturnsValueMaxErrorMessage, TestName =
            TestIndexerReturnsValueTestName)]
        public void TestGetErrors_InvalidValue_ReturnsEnumerableWithErrorMessage(int value,
            string errorMessage)
        {
            // Arrange
            string expected = errorMessage;

            // Act
            var parameter = new DeskParameter(DeskParameterType.WorktopLength, 800, 1200,
                value);
            IEnumerable<string> actual = parameter.GetErrors(nameof(parameter.Value))
                .Cast<string>();

            // Assert
            Assert.IsTrue(actual.Contains(expected));
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

        [TestCase(TestName = "У одинаковых объектов одинаковые хеш-коды")]
        public void TestGetHashCode_EqualObjects_EqualHashCodes()
        {
            // Arrange
            int expected = _testParameter.GetHashCode();

            // Act
            if (!(_testParameter.Clone() is DeskParameter cloneParameter))
            {
                return;
            }

            int actual = cloneParameter.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(TestName = "У разных объектов разные хеш-коды")]
        public void TestGetHashCode_DifferentObjects_DifferentHashCodes()
        {
            // Arrange
            int expected = _testParameter.GetHashCode();

            // Act
            if (!(_testParameter.Clone() is DeskParameter cloneParameter))
            {
                return;
            }

            cloneParameter.Value = 1077;
            int actual = cloneParameter.GetHashCode();

            // Assert
            Assert.AreNotEqual(expected, actual);
        }

        #endregion
    }
}