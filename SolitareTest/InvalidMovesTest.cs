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
    public class InvalidMovesTest : TestBase
    {
        private Card C(Suit s, int r, bool faceUp = true)
        {
            var c = new Card(s, r);
            c.isFaceUp = faceUp;
            return c;
        }

        [Fact]
        public void SM2_01_SameColorToTableau()
        {
            var source = new Tableau();
            var target = new Tableau();

            var red7 = C(Suit.hearts, 7);
            var red8 = C(Suit.diamonds, 8);

            var gm = GameManager.Instance;
            gm.StartGame();
            gm.Tableaus[0].cards.Clear();
            gm.Tableaus[1].cards.Clear();

            gm.Tableaus[0].addCard(red7);
            gm.Tableaus[1].addCard(red8);

            bool result = gm.TryMove(red7, gm.Tableaus[1]);

            Assert.False(result);
        }

        [Fact]
        public void SM2_02_NonKingToEmptyTableau()
        {
            var five = C(Suit.clubs, 5);

            var gm = GameManager.Instance;
            gm.StartGame();

            gm.Tableaus[0].cards.Clear();

            bool result = gm.TryMove(five, gm.Tableaus[0]);

            Assert.False(result);
        }

        [Fact]
        public void SM2_03_NonAceToEmptyFoundation()
        {
            var five = C(Suit.spades, 5);

            var gm = GameManager.Instance;
            gm.StartGame();

            var f = gm.Foundations.First(f => f.suit == Suit.spades);

            bool result = gm.TryMove(five, f);

            Assert.False(result);
        }

        [Fact]
        public void SM2_04_MoveFromtFaceDownCard()
        {
            var card = C(Suit.hearts, 10, faceUp: false);

            var gm = GameManager.Instance;
            gm.StartGame();

            gm.Tableaus[0].addCard(card);

            bool result = gm.TryMove(card, gm.Tableaus[1]);

            Assert.False(result);
        }

        [Fact]
        public void SM2_05_MoveIntoWaste()
        {
            var card = C(Suit.diamonds, 3);

            var gm = GameManager.Instance;
            gm.StartGame();

            gm.Tableaus[0].addCard(card);

            bool result = gm.TryMove(card, gm.Waste);

            Assert.False(result);
        }
    }
}
