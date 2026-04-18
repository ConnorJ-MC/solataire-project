using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireBack.CardPiles
{
    public class Tableau : CardPile
    {
        public Tableau() : base() { }

        public override bool canAccept(Card c)
        {
            if (isEmpty()) return c.rank == 13;
            Card topCard = cards.Last();
            if (topCard.isFaceUp == false) return false;
            if (c.isRed == topCard.isRed) return false;
            return c.rank == topCard.rank - 1;

            // will only accept a card if the tableau is empty and the card is a king, or if the card is one rank lower than the top card and of opposite color
             
        }

        public bool removeStack(List<Card> stack)
        {
            if (stack == null || stack.Count == 0) return false;
            for (int i = 0; i < stack.Count; i++)
            {
                if (stack[i] != cards[cards.Count - stack.Count + i]) return false;
            }
            try { cards.RemoveRange(cards.Count - stack.Count, stack.Count); return true; } catch { return false; }

            // will only remove the stack if the stack is not null or empty and the stack is the same as the top cards of the tableau
        }
    }
}
