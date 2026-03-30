using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SolitaireFront
{
    public class FoundationToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var pile = value as IList<CardViewModel>;
            if (pile == null || pile.Count == 0)
                return "/Recources/Playing Cards/card-blank.png";

            return pile[pile.Count - 1].ImagePath;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
