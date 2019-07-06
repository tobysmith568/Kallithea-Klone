using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace KallitheaKlone.WPF.Converters
{
    public class DoubleToStarDataGridLengthConverter : BaseConverter<DoubleToStarDataGridLengthConverter>
    {
        //  Constants
        //  =========

        private const string MustBeADouble = "The given value must be a double";
        private const string MustBeADataGridlength = "The given value must be a DataGridLength";
        private const string MustBeAStarValue = "The given DataGridLength must be a star value";

        //  Methods
        //  =======

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double))
            {
                throw new ArgumentException(MustBeADouble);
            }

            return new DataGridLength((double)value, DataGridLengthUnitType.Star, (double)value, (double)value);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DataGridLength))
            {
                throw new ArgumentException(MustBeADataGridlength);
            }

            DataGridLength dataGridLength = (DataGridLength)value;

            if (!dataGridLength.IsStar)
            {
                throw new ArgumentException(MustBeAStarValue);
            }

            return dataGridLength.Value;
        }
    }
}
