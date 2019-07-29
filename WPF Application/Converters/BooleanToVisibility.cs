using System;
using System.Globalization;
using System.Windows;

namespace KallitheaKlone.WPF.Converters
{
    public class BooleanToVisibility : BaseConverter<BooleanToVisibility>
    {
        //  Constants
        //  =========

        private const string MustBeABoolean = "The given value must be a boolean";

        //  Methods
        //  =======

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                throw new ArgumentException(MustBeABoolean);

            Visibility hiddenType = parameter is bool && (bool)parameter ? Visibility.Collapsed : Visibility.Hidden;

            return (bool)value ? Visibility.Visible : hiddenType;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
