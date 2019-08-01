using NUnit.Framework;
using KallitheaKlone.WPF.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KallitheaKlone.WPF.Converters.Tests
{
    [TestFixture]
    public class DoubleToPixelGridLengthConverterTests
    {
        private DoubleToPixelGridLengthConverter subject;

        [SetUp]
        public void Setup()
        {
            subject = new DoubleToPixelGridLengthConverter();
        }

        #region Convert

        [TestCase(null)]
        [TestCase(true)]
        [TestCase(5)]
        [TestCase("String")]
        public void Convert_ThrowsIfArgumentIsNotADouble(object input)
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() => subject.Convert(input, null, null, null));

            Assert.AreEqual("The given value must be a double", exception.Message);
        }

        [TestCase(-5)]
        [TestCase(0)]
        [TestCase(5)]
        [TestCase(45)]
        public void Convert_ConvertsDoubleIntoPixelGridLength(double input)
        {
            object objectResult = subject.Convert(input, null, null, null);

            Assert.IsInstanceOf(typeof(GridLength), objectResult);

            GridLength result = (GridLength)objectResult;

            Assert.AreEqual(input, result.Value);
            Assert.IsTrue(result.IsAbsolute);
        }

        #endregion
        #region ConvertBack

        [TestCase(null)]
        [TestCase(true)]
        [TestCase(5)]
        [TestCase("String")]
        public void ConvertBack_ThrowsIfArgumentIsNotAGridLength(object input)
        {
            ArgumentException argumentException = Assert.Throws<ArgumentException>(() => subject.ConvertBack(input, null, null, null));

            Assert.AreEqual("The given value must be a GridLength", argumentException.Message);
        }

        [TestCase(45, GridUnitType.Auto)]
        [TestCase(45, GridUnitType.Star)]
        public void ConvertBack_ThrowsIfGridLengthIsNotAbsolute(int value, GridUnitType type)
        {
            GridLength input = new GridLength(value, type);

            ArgumentException argumentException = Assert.Throws<ArgumentException>(() => subject.ConvertBack(input, null, null, null));

            Assert.AreEqual("The given GridLength must be a pixel value", argumentException.Message);
        }

        [TestCase(-5)]
        [TestCase(0)]
        [TestCase(5)]
        [TestCase(45)]
        public void ConvertBack_ReturnsPixelGridLenthsValue(int value)
        {
            GridLength input = new GridLength(value, GridUnitType.Pixel);

            object resultObject = subject.ConvertBack(input, null, null, null);

            Assert.IsInstanceOf<double>(resultObject);

            double result = (double)resultObject;

            Assert.AreEqual(value, result);
        }

        #endregion
    }
}