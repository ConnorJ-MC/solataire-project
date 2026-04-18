using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireBack.CardPiles
{
    public class Foundation : CardPile
    {
        public Foundation() : base() { }

        public Foundation(Suit suit)
        {
            this.suit = suit;
        }

        public Suit suit;

        public override bool canAccept(Card c)
        {
            if (c.suit != suit) return false;
            if (isEmpty()) return c.rank == 1;
            return c.rank == cards.Last().rank + 1;

            // The foundation can only accept cards of the same suit, and they must be placed in ascending order starting with the Ace (rank 1).
        }
    }
}
