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
                Card card = Cards[Cards.Count - 1];
                Cards.RemoveAt(Cards.Count - 1);
                drawnCards.Add(card);
            }

            drawnCards.Reverse(); //reverse the order of the drawn cards so that the top card is at the end of the list
            return drawnCards;
        }

        public bool reset(List<Card> newCards) //will reverse the order of the new cards and flip them all face down, then add them to the stock
        {
            if (newCards.Count == 0)
            {
                return false;
            }
            newCards.Reverse();
            foreach (Card card in newCards)
            {
                card.isFaceUp = false;
                Cards.Add(card);
            }
            return true;
            
        }
    }
}
