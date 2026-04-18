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

            String tableauCardCount = "";
            
            for (int i = 0; i < gm.Tableaus.Count; i++)
            {
                tableauCardCount += gm.Tableaus[i].cards.Count + " ";
            }

            Assert.True(result);
            Assert.NotNull(gm.Stock);
            Assert.Equal(24, gm.Stock.cards.Count);
            Assert.NotNull(gm.Waste);
            Assert.Equal(7, gm.Tableaus.Count);
            Assert.Equal("1 2 3 4 5 6 7 ", tableauCardCount);
            Assert.Equal(4, gm.Foundations.Count);

            /*
             * This test verifies that when the StartGame method is called, the game is initialized correctly. 
             * It checks that the Stock is created and contains 24 cards (since 28 cards are dealt to the Tableaus), 
             * that the Waste is created, that there are 7 Tableaus with the correct number of cards (1 card in the first Tableau,
             * 2 in the second, and so on up to 7 in the seventh), 
             * and that there are 4 Foundations created.
             */
        }

        [Fact]
        public void SM4_02_TopCardFaceUp()
        {
            var gm = GameManager.Instance;
            gm.StartGame();

            foreach (var t in gm.Tableaus)
            {
                Assert.True(t.cards.Last().isFaceUp);
                Assert.All(t.cards.Take(t.cards.Count - 1), c => Assert.False(c.isFaceUp));
            }

            /*
            * This test verifies that after starting the game, the top card of each Tableau is face up, 
            * while all other cards in the Tableau are face down. 
            * It iterates through each Tableau and checks that the last card (the top card) is face up, 
            * and that all preceding cards are face down.
            */
        }
    }
}
