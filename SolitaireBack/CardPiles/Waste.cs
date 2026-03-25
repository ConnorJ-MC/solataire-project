using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireBack.CardPiles
{
    public class Waste : CardPile
    {
        public Waste() : base()
        {
        }

        public Card topcard()
        {
            if (isEmpty())
            {
                return null;
            }
            return cards[cards.Count - 1];
        }

        public List<Card> reset()
        {
            List<Card> cardsToReturn = new List<Card>(cards);
            cards.Clear();
            return cardsToReturn;
        }
    }
}
