using System;
using System.Globalization;
using System.Windows;

namespace KallitheaKlone.WPF.Converters
{
    public class DoubleToPixelGridLengthConverter : BaseConverter<DoubleToPixelGridLengthConverter>
    {
        //  Constants
        //  =========

        private const string MustBeADouble = "The given value must be a double";
        private const string MustBeAGridlength = "The given value must be a GridLength";
        private const string MustBeAPixelValue = "The given GridLength must be a pixel value";

        //  Methods
        //  =======

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double))
            {
                throw new ArgumentException(MustBeADouble);
            }

            return new GridLength((double)value, GridUnitType.Pixel);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is GridLength))
            {
                throw new ArgumentException(MustBeAGridlength);
            }

            GridLength gridLength = (GridLength)value;

            if (!gridLength.IsAbsolute)
            {
                throw new ArgumentException(MustBeAPixelValue);
            }

            return gridLength.Value;
        }
    }
}
