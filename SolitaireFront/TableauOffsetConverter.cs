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
        }

        public object[] ConvertBack(object value, Type[] targetType,  Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
