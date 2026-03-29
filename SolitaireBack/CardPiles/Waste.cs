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
            return Cards[Cards.Count - 1];
        }

        public List<Card> reset()
        {
            List<Card> cardsToReturn = new List<Card>(Cards);
            Cards.Clear();
            return cardsToReturn;
        }
    }
}
