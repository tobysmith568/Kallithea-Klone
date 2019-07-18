using System;
using System.Globalization;
using System.Windows;

namespace KallitheaKlone.WPF.Converters
{
    public class IntegerToWindowStateConverter : BaseConverter<IntegerToWindowStateConverter>
    {
        //  Constants
        //  =========

        private const string MustBeAWindowState = "The given value must be a WindowState";

        //  Methods
        //  =======

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case 1:
                    return WindowState.Minimized;
                case 2:
                    return WindowState.Maximized;
                default:
                    return WindowState.Normal;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is WindowState))
                throw new ArgumentException(MustBeAWindowState);

            int result;

            try
            {
                result = (int)value;
            }
            catch
            {
                result = 0;
            }

            return result;
        }
    }
}