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

        // override so runtime dispatch works when called through CardPile reference
        public override bool canAccept(Card c)
        {
            if (c.suit != suit) return false;
            if (isEmpty()) return c.rank == 1;
            return c.rank == cards.Last().rank + 1;
        }
    }
}
