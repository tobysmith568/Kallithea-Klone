using NUnit.Framework;
using System;
using System.Windows;

namespace KallitheaKlone.WPF.Converters.Tests
{
    [TestFixture]
    public class BooleanToVisibilityTests
    {
        private BooleanToVisibility subject;

        [SetUp]
        public void SetUp()
        {
            subject = new BooleanToVisibility();
        }

        #region Convert

        [TestCase(null)]
        [TestCase(5)]
        [TestCase("String")]
        [TestCase(new Visibility())]
        public void Convert_ThrowsIfArgumentIsNotABool(object input)
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>subject.Convert(input, null, null, null));

            Assert.AreEqual("The given value must be a boolean", exception.Message);
        }

        [TestCase(true, Visibility.Visible)]
        [TestCase(false, Visibility.Collapsed)]
        public void Convert_ConvertsBooleanIntoVisibility(bool input, Visibility expectedResult)
        {
            object objectResult = subject.Convert(input, null, null, null);

            Assert.IsInstanceOf<Visibility>(objectResult);

            Visibility result = (Visibility)objectResult;

            Assert.AreEqual(expectedResult, result);
        }

        #endregion
        #region ConvertBack

        [TestCase(null)]
        [TestCase(true)]
        [TestCase(5)]
        [TestCase("String")]
        [TestCase(new Visibility())]
        public void ConvertBack_AlwaysThrowsNotImplemented(object input)
        {
            Assert.Throws<NotImplementedException>(() => subject.ConvertBack(input, null, null, null));
        }

        #endregion
    }
}