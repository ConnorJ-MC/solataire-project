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

            /*
             * The Convert method takes an object value, which is expected to be an IEnumerable<CardViewModel>
             * representing the cards in a foundation pile. 
             * It retrieves the top card from the enumerable and returns the ImagePath of that card. 
             * If the enumerable is null or empty, it returns a default image path for an empty foundation pile.
             */

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();

            /*
            * The ConvertBack method is not implemented in this converter, as it is not needed for the intended use case. 
            * It simply throws a NotImplementedException to indicate that this operation is not supported.
            */
        }

        /*
         * The FoundationToImageConverter class implements the IValueConverter interface,
         * which is used in WPF data binding to convert values from one type to another. 
         * In this case, it converts a collection of CardViewModel objects representing a foundation pile 
         * into a string that represents the file path of the image to be displayed for that pile.
         */
    }
}
