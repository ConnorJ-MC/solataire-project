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

        public List<Card> reset()
        {
            List<Card> cardsToReturn = new List<Card>(cards);
            cards.Clear();
            return cardsToReturn;

            /*
             * This is used to reset the waste pile back to the stock pile when the stock pile is empty.
             * It returns a list of cards that should be moved back to the stock pile, and clears the waste pile.
             */
        }

        public override bool canAccept(Card c)
        {
            return false;

            // The waste pile cannot accept any cards, so this method always returns false.
        }
    }
}
