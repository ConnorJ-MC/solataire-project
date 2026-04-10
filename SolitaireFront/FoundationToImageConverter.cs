using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SolitaireFront
{
    public class FoundationToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // accept IEnumerable<CardViewModel> and IList<CardViewModel> safely
            var enumerable = value as IEnumerable<CardViewModel>;
            if (enumerable == null)
            {
                return "Recources/Playing Cards/card-foundation-default.png";
            }

            CardViewModel top = enumerable.LastOrDefault();
            if (top == null)
                return "Recources/Playing Cards/card-foundation-default.png";

            return top.ImagePath;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
