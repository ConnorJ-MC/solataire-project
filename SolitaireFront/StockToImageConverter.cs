using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SolitaireFront
{
    public class StockToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int count = (int)value;

            if (count == 0) return "/Recources/Playing Cards/card-foundation-default.png";

            return "/Recources/Playing Cards/card-back2.png";

            /* 
            * The Convert method takes an object value, which is expected to be an integer representing the number of cards in the stock pile. 
            * It checks the count and returns a string that represents the file path of the image to be displayed for the stock pile. 
            * If the count is zero, it returns a default image path for an empty stock pile; otherwise, it returns the image path for the back of a card.
            */
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
