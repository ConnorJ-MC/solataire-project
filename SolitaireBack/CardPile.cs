using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireBack
{
    public class CardPile
    {
        public List<Card> cards = new List<Card>();

        
        public bool addCard(Card card)
        {
            if (card == null)
            {
                return false;
            }

            // Prevent adding the exact same card instance twice.
            if (cards.Contains(card))
            {
                return false;
            }

            try
            {
                cards.Add(card);
                return true;
            }
            catch
            {
                // Swallow unexpected exceptions and indicate failure.
                return false;
            }
        }

        public bool removeTop(Card card)
        {
            if (card == null)
            {
                return false;
            }

            int index = cards.IndexOf(card);
            if (index == -1)
            {
                // Card not found in pile.
                return false;
            }

            // Only allow removing if the specified card is the top card.
            if (index != cards.Count - 1)
            {
                return false;
            }

            try
            {
                cards.RemoveAt(index);
                return true;
            }
            catch
            {
                // Swallow unexpected exceptions and indicate failure.
                return false;
            }
        }

        public bool addStack(List<Card> stack)
        {
            if (stack == null || stack.Count == 0)
            {
                return false;
            }

            // Reject stacks that contain null entries.
            if (stack.Any(c => c == null))
            {
                return false;
            }

            // Prevent adding any card instance that's already present in this pile.
            if (stack.Any(c => cards.Contains(c)))
            {
                return false;
            }

            try
            {
                cards.AddRange(stack);
                return true;
            }
            catch
            {
                // Swallow unexpected exceptions and indicate failure.
                return false;
            }
        }

        public List<Card> removeStack(Card card)
        {
            int index = cards.IndexOf(card);
            if (index == -1)
                throw new ArgumentException("Card not found in pile.");
            List<Card> stack = cards.GetRange(index, cards.Count - index);
            cards.RemoveRange(index, cards.Count - index);
            return stack;
        }

        public Card peakTop()
        {
            if (cards.Count == 0)
                throw new InvalidOperationException("Cannot peak top card of an empty pile.");

            Card top = cards[cards.Count - 1];

            // Flip the top card if it's currently face-down, then return it.
            if (!top.isFaceUp)
            {
                top.flip();
            }

            return top;
        }

        public List<Card> getStackFrom(Card c)
        {
            int index = cards.IndexOf(c);
            if (index == -1 || cards[index].isFaceUp == false)
            {
                return null;
            }
            return cards.GetRange(index, cards.Count - index);
        }

        public bool isEmpty() => cards.Count == 0;

        public bool contains(Card card) => cards.Contains(card);

        public bool canAccept(Card card) => true; // Base CardPile has no restrictions, override in subclasses as needed.
    }
}
