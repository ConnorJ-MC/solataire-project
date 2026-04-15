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
            Assert.True(gm.Waste.cards.Last().isFaceUp);
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
            Assert.True(gm.Waste.cards.Last().isFaceUp);
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
        }
    }
}
