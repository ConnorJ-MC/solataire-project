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
        public void SM7_01_resetGame_incrementsGamesPlayes()
        {
            var gm = GameManager.Instance;
            gm.StartGame();

            var lm = LoginManager.Instance;

            lm.currentPlayer = new Player("Test", "Player", DateOnly.FromDateTime(DateTime.Now.AddYears(-20)),0);
            lm.isGuest = false;
            lm.currentPlayer.gamesP = 0;

            gm.resetGame();

            Assert.Equal(1, lm.currentPlayer.gamesP);
        }
    }
}
