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
        private Card C(Suit s, int r, bool faceUp = true)
        {
            var c = new Card(s, r);
            c.isFaceUp = faceUp;
            return c;
        }
        
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
        }

        [Fact]
        public void SM6_02_LoseGame()
        {
            var gm = GameManager.Instance;
            gm.StartGame();

            gm.Foundations[0].cards.Clear();
            gm.Foundations[0].addCard(new Card(gm.Foundations[0].suit, 1));

            Assert.False(gm.winCheck());
        }
    }
}
