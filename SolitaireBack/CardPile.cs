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

            /*
             * this adds a single carrd to the target pile, and returns true if successful, false otherwise.
             * It should return false if the card is null, or if the card is already in the pile. It should not throw an exception.
             */
        }

        public bool removeTop(Card card)
        {
            if (card == null) return false;
            int index = cards.IndexOf(card);
            if (index == -1) return false;
            if (index != cards.Count - 1) return false;
            try { cards.RemoveAt(index); return true; } catch { return false; }

            /*
             * this removes the specified card from the pile, but only if it is the top card. 
             * It should return true if successful, false otherwise. 
             * It should return false if the card is null, if the card is not in the pile, or if the card is not the top card. 
             * It should not throw an exception.
             */
        }

        public bool addStack(List<Card> stack)
        {
            if (stack == null || stack.Count == 0) return false;
            if (stack.Any(c => c == null)) return false;
            if (stack.Any(c => cards.Contains(c))) return false;
            try { cards.AddRange(stack); return true; } catch { return false; }

            /*
             * this adds a stack of cards to the target pile, and returns true if successful, false otherwise.
             * It should return false if the stack is null, empty, contains null cards, or if any card in the stack is already in the pile.
             * It should not throw an exception.
             */
        }

        public List<Card> removeStack(Card card)
        {
            int index = cards.IndexOf(card);
            if (index == -1)
                throw new ArgumentException("Card not found in pile.");
            List<Card> stack = cards.GetRange(index, cards.Count - index);
            cards.RemoveRange(index, cards.Count - index);
            return stack;

            /*
             * this removes the specified card and all cards above it from the pile, and returns them as a list.
             * It should throw an ArgumentException if the card is not found in the pile.
             */
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

            /*
             * this returns the top card of the pile without removing it. If the pile is empty, it should throw an InvalidOperationException.
             * If the top card is face down, it should be flipped face up before being returned.
             */
        }

        public List<Card> getStackFrom(Card c)
        {
            int index = cards.IndexOf(c);
            if (index == -1 || cards[index].isFaceUp == false)
            {
                return null;
            }
            return cards.GetRange(index, cards.Count - index);

            /*
             * this returns a list of the specified card and all cards above it in the pile, without removing them. 
             * If the specified card is not found in the pile, or if the specified card is face down, it should return null.
             */
        }

        public bool isEmpty() => cards.Count == 0; // A method that returns true if the pile is empty (i.e., contains no cards) and false otherwise.

        public bool contains(Card card) => cards.Contains(card); 
        /*
         * A method that returns true if the specified card is in the pile and false otherwise. 
         * It should return false if the card is null.
         */

        // Make this overridable so subclasses enforce their rules at runtime
        public virtual bool canAccept(Card card) => true;
    }
}
