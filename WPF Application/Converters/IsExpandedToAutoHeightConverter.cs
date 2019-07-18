using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KallitheaKlone.WPF.Converters
{
    public class IsExpandedToAutoHeightConverter : BaseConverter<IsExpandedToAutoHeightConverter>
    {
        //  Constants
        //  =========

        private const string MustBeABool = "The given value must be a boolean";

        //  Methods
        //  =======

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                throw new ArgumentException(MustBeABool);

            return new GridLength(1, (bool)value ? GridUnitType.Star : GridUnitType.Auto);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
