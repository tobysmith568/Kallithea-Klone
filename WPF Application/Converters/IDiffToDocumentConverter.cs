using KallitheaKlone.Models.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace KallitheaKlone.WPF.Converters
{
    public class IDiffToDocumentConverter : BaseConverter<IDiffToDocumentConverter>
    {
        //  Constants
        //  =========

        private const string MustBeAIDiff = "The given value must be an IDiff";
        private const string Consolas = "consolas";

        //  Methods
        //  =======

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is IDiff))
                throw new ArgumentException(MustBeAIDiff);

            Paragraph paragraph = new Paragraph();

            ObservableCollection<string> lines = (value as IDiff).Text;
            for (int i = 0; i < lines.Count; i++)
            {
                paragraph.Inlines.Add(new TextBlock()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(0),
                    Text = lines[i],
                    TextWrapping = TextWrapping.NoWrap,
                    Background = GetBackgroundColour(lines[i]),
                    FontFamily = new FontFamily(Consolas)
                });

                if (i != lines.Count - 1)
                {
                    paragraph.Inlines.Add(new LineBreak());
                }
            }

            return new FlowDocument(paragraph);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private Brush GetBackgroundColour(string line)
        {
            if (line == null || line == string.Empty)
            {
                return Brushes.Gray;
            }

            switch (line[0])
            {
                case '+':
                    return Brushes.LightGreen;
                case '-':
                    return new SolidColorBrush(Color.FromRgb(240, 156, 156));
                default:
                    return Brushes.Transparent;
            }
        }
    }
}
