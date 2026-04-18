using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolitaireBack;
using Xunit;

namespace SolitareTest
{
    public class WinConditionsTest : TestBase
    {
        [Fact]
        public void SM6_01_WinGame()
        {
            var gm = GameManager.Instance;
            gm.StartGame();

            foreach (var f in gm.Foundations)
            {
                f.cards.Clear();
                for (int i = 1; i <= 13; i++)
                {
                    f.addCard(new Card(f.suit, i));
                }
            }

            Assert.True(gm.winCheck());

            /*
             * This test verifies that the win condition is correctly detected when all Foundations are complete. 
             * It first starts a new game, then clears each Foundation and adds the complete set of cards (Ace through King) for each suit. 
             * Finally, it asserts that the winCheck method returns true, indicating that the game has been won.
             */
        }

        [Fact]
        public void SM6_02_LoseGame()
        {
            var gm = GameManager.Instance;
            gm.StartGame();

            gm.Foundations[0].cards.Clear();
            gm.Foundations[0].addCard(new Card(gm.Foundations[0].suit, 1));

            Assert.False(gm.winCheck());

            /*
             * This test verifies that the win condition is not met when the Foundations are incomplete. 
             * It starts a new game, then clears the first Foundation and adds only the Ace of that suit. 
             * Finally, it asserts that the winCheck method returns false, indicating that the game has not been won.
             */
        }
    }
}
