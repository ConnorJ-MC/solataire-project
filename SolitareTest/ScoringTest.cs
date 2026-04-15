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
    public class ScoringTest : TestBase
    {
        private Card C(Suit s, int r, bool faceUp = true)
        {
            var c = new Card(s, r);
            c.isFaceUp = faceUp;
            return c;
        }

        [Fact]
        public void SM5_01_StartGame_RemovesFiftyTwo()
        {
            var gm = GameManager.Instance;
            var lm = LoginManager.Instance;
            lm.setVegasMode(true);

            lm.currentPlayer = new Player("Test", "Player", DateOnly.FromDateTime(DateTime.Now.AddYears(-20)), 0);
            lm.isGuest = false;

            int balanceBefore = lm.currentPlayer.balance;

            gm.StartGame();

            Assert.Equal(balanceBefore - 52, lm.currentPlayer.balance);
        }

        [Fact]
        public void SM5_02_MoveToFoundation_AddsFive()
        {
            var gm = GameManager.Instance;
            gm.StartGame();

            var lm = LoginManager.Instance;
            lm.setVegasMode(true);

            lm.currentPlayer = new Player("Test", "Player", DateOnly.FromDateTime(DateTime.Now.AddYears(-20)), 0);
            lm.isGuest = false;

            var ace = C(Suit.clubs, 1);
            gm.Waste.addCard(ace);

            var f = gm.Foundations.First(f => f.suit == Suit.clubs);

            gm.TryMove(ace, f);

            Assert.Equal(5, lm.currentPlayer.balance);
        }
    }
}
