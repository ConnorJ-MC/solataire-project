using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolitaireBack;
using Xunit;

namespace SolitareTest
{
    public class ResetGameTest : TestBase
    {
        [Fact]
        public void SM7_01_ResetGame_IncrementsGamesPlayedAndWon()
        {
            var gm = GameManager.Instance;
            gm.StartGame();

            var lm = LoginManager.Instance;

            lm.currentPlayer = new Player("Test", "Player", DateOnly.FromDateTime(DateTime.Now.AddYears(-20)), 0);
            lm.isGuest = false;
            lm.currentPlayer.gamesP = 0;
            lm.currentPlayer.gamesW = 0;
            lm.currentPlayer.gamesL = 0;

            // Simulate winning the game
            foreach (var f in gm.Foundations)
            {
                f.cards.Clear();
                for (int i = 1; i <= 13; i++)
                {
                    f.addCard(new Card(f.suit, i));
                }
            }
            Assert.True(gm.winCheck());
            gm.resetGame();
            Assert.Equal(1, lm.currentPlayer.gamesP);
            Assert.Equal(1, lm.currentPlayer.gamesW);
            Assert.Equal(0, lm.currentPlayer.gamesL);

            /*
             * This test verifies that when the resetGame method is called after winning a game, 
             * it correctly increments the games played and games won counters for the current player.
             * It first starts a new game and sets up a test player. 
             * Then, it simulates winning the game by filling each Foundation with a complete suit of cards (Ace through King).
             * After confirming that the win condition is met, it calls resetGame and asserts that the games played counter has incremented to 1, 
             * the games won counter has incremented to 1, and the games lost counter remains at 0.
             */
        }

        [Fact]
        public void SM7_02_ResetGame_IncrementsGamesPlayedButNotWon()
        {
            var gm = GameManager.Instance;
            gm.StartGame();

            var lm = LoginManager.Instance;

            lm.currentPlayer = new Player("Test", "Player", DateOnly.FromDateTime(DateTime.Now.AddYears(-20)), 0);
            lm.isGuest = false;

            lm.currentPlayer.gamesP = 0;
            lm.currentPlayer.gamesW = 0;
            lm.currentPlayer.gamesL = 0;
            // Simulate losing the game
            gm.resetGame();
            Assert.Equal(1, lm.currentPlayer.gamesP);
            Assert.Equal(0, lm.currentPlayer.gamesW);
            Assert.Equal(1, lm.currentPlayer.gamesL);

            /*
             * This test verifies that when the resetGame method is called without winning the game, 
             * it correctly increments the games played counter and the games lost counter for the current player.
             * It first starts a new game and sets up a test player. 
             * Then, it simulates losing the game by calling resetGame without meeting the win condition.
             * After calling resetGame, it asserts that the games played counter has incremented to 1, 
             * the games won counter remains at 0, and the games lost counter has incremented to 1.
             */
        }
    }
}
