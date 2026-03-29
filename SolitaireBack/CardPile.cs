using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SolitaireBack
{
    public class CardPile : INotifyPropertyChanged
    {
        //#1
        public ObservableCollection<Card> Cards { get; set; } = new ObservableCollection<Card>();
        public Card TopCard => Cards.Count > 0 ? Cards[Cards.Count - 1] : null;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public bool addCard(Card card)
        {
            if (card == null)
            {
                return false;
            }

            // Prevent adding the exact same card instance twice.
            if (Cards.Contains(card))
            {
                return false;
            }

            try
            {
                // Set the old top card to false
                if (Cards.Count > 0) {
                    Cards.Last().IsTopCard = false;
                }

                Cards.Add(card);

                // Set the new top card to true
                card.IsTopCard = true;

                OnPropertyChanged(nameof(TopCard));
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

            int index = Cards.IndexOf(card);
            if (index == -1)
            {
                // Card not found in pile.
                return false;
            }

            // Only allow removing if the specified card is the top card.
            if (index != Cards.Count - 1)
            {
                return false;
            }

            try
            {
                Cards.RemoveAt(index);
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
            if (stack.Any(c => Cards.Contains(c)))
            {
                return false;
            }

            try
            {
                //#1 cards.AddRange(stack);
                foreach (Card card in stack)
                {
                    Cards.Add(card);
                }
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
            int index = Cards.IndexOf(card);
            if (index == -1)
                throw new ArgumentException("Card not found in pile.");
            //#1 List<Card> stack = cards.GetRange(index, cards.Count - index);
            List<Card> stack = Cards.Skip(index).ToList();
            while (Cards.Count > index)
            {
                Cards.RemoveAt(index);
            }
            //#1 cards.RemoveRange(index, cards.Count - index);
            return stack;
        }

        public Card peakTop()
        {
            if (Cards.Count == 0)
                throw new InvalidOperationException("Cannot peak top card of an empty pile.");

            Card top = Cards[Cards.Count - 1];

            // Flip the top card if it's currently face-down, then return it.
            if (!top.isFaceUp)
            {
                top.flip();
            }

            return top;
        }

        public List<Card> getStackFrom(Card c)
        {
            int index = Cards.IndexOf(c);
            if (index == -1 || Cards[index].isFaceUp == false)
            {
                return null;
            }
            // #1return cards.GetRange(index, cards.Count - index);
            return Cards.Skip(index).ToList();
            
        }

        public bool isEmpty() => Cards.Count == 0;

        public bool contains(Card card) => Cards.Contains(card);

        public bool canAccept(Card card) => true; // Base CardPile has no restrictions, override in subclasses as needed.
    }
}
