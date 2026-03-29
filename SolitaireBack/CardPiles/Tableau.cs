namespace SolitaireBack.CardPiles
{
    public class Tableau : CardPile
    {
        public Tableau() : base()
        {
        }

        public new bool canAccept(Card c)
        {
            if (isEmpty())
            {
                return c.rank == 13;
            }
            Card topCard = Cards.Last();
            if (topCard.isFaceUp == false)
            {
                return false;
            }
            if (c.isRed == topCard.isRed)
            {
                return false;
            }
            return c.rank == topCard.rank - 1;
        }

        public bool removeStack(List<Card> stack)
        {
            if (stack == null || stack.Count == 0) return false;

            // Use a safer removal loop
            int startIndex = Cards.Count - stack.Count;
            while (Cards.Count > startIndex)
            {
                Cards.RemoveAt(startIndex);
            }

            OnPropertyChanged(nameof(TopCard)); // Update the visual top card
            return true;
        }

        
    }
}
