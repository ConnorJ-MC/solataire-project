using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireBack.CardPiles
{
    public class Stock : CardPile
    {
        public Stock() : base()
        {
        }

        public List<Card> draw(int ammount)
        {
            List<Card> drawnCards = new List<Card>();
            for (int i = 0; i < ammount; i++)
            {
                if (isEmpty())
                {
                    break;
                }
                Card card = cards[cards.Count - 1];
                cards.RemoveAt(cards.Count - 1);
                drawnCards.Add(card);
            }

            drawnCards.Reverse();
            return drawnCards;
            
            /* 
             * will draw the specified ammount of cards from the stock, or as many as are left if there are less than the specified ammount. 
             * The drawn cards will be returned in the order they were drawn, with the top card of the stock being the last card in the returned list. 
             */
        }
        

        public bool reset(List<Card> newCards)
        {
            if (newCards.Count == 0)
            {
                return false;
            }
            newCards.Reverse();
            foreach (Card card in newCards)
            {
                card.isFaceUp = false;
                cards.Add(card);
            }
            return true;

            /*
             * will reset the stock with the specified list of cards, which should be the cards that were previously drawn from the stock. 
             * The cards will be added to the stock in the order they are in the list, with the first card in the list being the top card of the stock. 
             * The cards will be flipped face down before being added to the stock. The method will return true if the stock was successfully reset, 
             * or false if the provided list of cards was empty.
             */
        }
    }
}
