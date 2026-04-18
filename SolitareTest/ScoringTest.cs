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

            /*
             * A helper method to create a card with the specified suit, rank, and face-up status. 
             * By default, the card is created as face up (faceUp = true), but this can be overridden by passing false for the faceUp parameter.
             */
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

            /*
            * This test verifies that when the StartGame method is called in Vegas mode, it deducts 52 from the player's balance. 
            * It first retrieves the current player's balance before starting the game, then calls StartGame,
            * and finally asserts that the player's balance has been reduced by 52.
            */
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

            /*
             * This test checks that moving an Ace to the foundation in Vegas mode adds 5 to the player's balance. 
             * It starts a new game, sets up a test player in Vegas mode, creates an Ace of clubs, and adds it to the waste pile. 
             * Then it finds the corresponding foundation for clubs and attempts to move the Ace there. 
             * Finally, it asserts that the player's balance has increased by 5.
             */
        }
    }
}
