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
    public class GameStartTest : TestBase
    {
        [Fact]
        public void SM4_01_StartGame_InitialSetup()
        {
            var gm = GameManager.Instance;
            bool result = gm.StartGame();

            Assert.True(result);
            Assert.NotNull(gm.Stock);
            Assert.NotNull(gm.Waste);
            Assert.Equal(7, gm.Tableaus.Count);
            Assert.Equal(4, gm.Foundations.Count);

            // Ensures that each of the piles mentioned, exist within the game
        }

        [Fact]
        public void SM4_02_TopCardFaceUp()
        {
            var gm = GameManager.Instance;
            gm.StartGame();

            foreach (var t in gm.Tableaus)
            {
                Assert.True(t.cards.Last().isFaceUp);
            }

            // 
        }
    }
}
