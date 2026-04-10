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
            if (card == null) return false;
            if (cards.Contains(card)) return false;
            try { cards.Add(card); return true; } catch { return false; }
        }

        public bool removeTop(Card card)
        {
            if (card == null) return false;
            int index = cards.IndexOf(card);
            if (index == -1) return false;
            if (index != cards.Count - 1) return false;
            try { cards.RemoveAt(index); return true; } catch { return false; }
        }

        public bool addStack(List<Card> stack)
        {
            if (stack == null || stack.Count == 0) return false;
            if (stack.Any(c => c == null)) return false;
            if (stack.Any(c => cards.Contains(c))) return false;
            try { cards.AddRange(stack); return true; } catch { return false; }
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

        // Make this overridable so subclasses enforce their rules at runtime
        public virtual bool canAccept(Card card) => true;
    }
}
