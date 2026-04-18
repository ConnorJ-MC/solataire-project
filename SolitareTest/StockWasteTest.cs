using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolitaireBack;
using Xunit;
using SolitaireBack.CardPiles;

namespace SolitareTest
{
    public class StockWasteTest : TestBase
    {
        private Card C(Suit s, int r, bool faceUp = true)
        {
            var c = new Card(s, r);
            c.isFaceUp = faceUp;
            return c;

            /*
             * A helper method to create a card with the specified suit, rank, and face-up status. 
             * By default, the card is created as face up (faceUp = true), but this can be overridden by passing false for the faceUp parameter.
             */
        }


        [Fact]
        public void SM3_01_Draw_Easy()
        {
            var gm = GameManager.Instance;
            gm.StartGame();

            LoginManager.Instance.setDifficulty("easy");

            int stockBefore = gm.Stock.cards.Count;

            bool result = gm.drawFromStock();

            Assert.True(result);
            Assert.Equal(stockBefore - 1, gm.Stock.cards.Count);
            Assert.Single(gm.Waste.cards);
            Assert.All(gm.Waste.cards, c => Assert.True(c.isFaceUp));

            /*
            * This test verifies that when the difficulty is set to "easy" and the drawFromStock method is called, 
            * it successfully draws one card from the Stock to the Waste. 
            * It checks that the method returns true, indicating a successful draw, that the Stock has one less card than before, 
            * that there is exactly one card in the Waste, and that all cards in the Waste are face up.
            */
        }

        [Fact]
        public void SM3_02_Draw_Medium()
        {
            var gm = GameManager.Instance;
            gm.StartGame();


            LoginManager.Instance.setDifficulty("medium");

            int stockBefore = gm.Stock.cards.Count;

            bool result = gm.drawFromStock();

            Assert.True(result);
            Assert.Equal(stockBefore - 3, gm.Stock.cards.Count);
            Assert.Equal(3, gm.Waste.cards.Count);
            Assert.All(gm.Waste.cards, c => Assert.True(c.isFaceUp));

            /*
             * This test verifies that when the difficulty is set to "medium" and the drawFromStock method is called, 
             * it successfully draws three cards from the Stock to the Waste. 
             * It checks that the method returns true, indicating a successful draw, that the Stock has three fewer cards than before, 
             * that there are exactly three cards in the Waste, and that all cards in the Waste are face up.
             */
        }

        [Fact]
        public void SM3_03_RecycleWasteIntoStock()
        {
            var gm = GameManager.Instance;
            gm.StartGame();

            gm.Stock.cards.Clear();

            var c1 = C(Suit.hearts, 5);
            var c2 = C(Suit.spades, 9);
            gm.Waste.addCard(c1);
            gm.Waste.addCard(c2);

            bool result = gm.drawFromStock();

            Assert.True(result);
            Assert.True(gm.Waste.isEmpty());
            Assert.Equal(2, gm.Stock.cards.Count);
            Assert.All(gm.Stock.cards, c => Assert.False(c.isFaceUp));

            /*
             * This test verifies that when the Stock is empty and the drawFromStock method is called, 
             * it successfully recycles the cards from the Waste back into the Stock. 
             * It checks that the method returns true, indicating a successful recycle, that the Waste is empty after recycling, 
             * that the Stock now contains the cards that were previously in the Waste, and that all cards in the Stock are face down.
             */
        }
    }
}
