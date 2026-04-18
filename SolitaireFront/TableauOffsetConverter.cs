using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Collections.Generic;
using System.Globalization;
using SolitaireFront;

namespace SolitaireFront
{
    public class TableauOffsetConverter : IMultiValueConverter
    {
        public object Convert(Object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var card = values[0] as CardViewModel;
            var pile = values[1] as IEnumerable<CardViewModel>;

            if (card == null || pile == null)
                return 0;

            int index = pile.ToList().IndexOf(card);

            if (!card.Model.isFaceUp)
                return index * 20;

            return index * 35;

            /*
             * The Convert method takes an array of objects as input, where the first object is expected to be a CardViewModel representing a card in a tableau pile, 
             * and the second object is expected to be an IEnumerable<CardViewModel> representing the entire pile. 
             * It calculates the index of the card within the pile and returns an offset value based on whether the card is face up or face down. 
             * Face-down cards have a smaller offset (20 pixels) compared to face-up cards (35 pixels), creating a visual stacking effect in the UI.
             */
        }

        public object[] ConvertBack(object value, Type[] targetType,  Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
