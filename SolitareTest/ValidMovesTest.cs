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
    public class ValidMovesTest : TestBase
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
        public void SM1_01_TableuaToTableau()
        {
            var source = new Tableau();
            var target = new Tableau();

            var red7 = C(Suit.hearts, 7);
            var black8 = C(Suit.spades, 8);

            source.addCard(red7);
            target.addCard(black8);

            var gm = GameManager.Instance;
            gm.StartGame();
            gm.Tableaus[0].cards.Clear();
            gm.Tableaus[1].cards.Clear();

            gm.Tableaus[0].addCard(red7);
            gm.Tableaus[1].addCard(black8);

            bool result = gm.TryMove(red7, gm.Tableaus[1]);

            Assert.True(result);
            Assert.Contains(red7, gm.Tableaus[1].cards);
            Assert.DoesNotContain(red7, gm.Tableaus[0].cards);

            /* This test verifies that a valid move from one tableau to another is successful. 
             * It creates two tableaus, adds a red 7 to the source tableau and a black 8 to the target tableau, 
             * and then attempts to move the red 7 onto the black 8. The test asserts that the move is successful, 
             * that the red 7 is now in the target tableau, and that it has been removed from the source tableau.
             */
        }

        [Fact]
        public void SM1_02_TableauToFoundation_AceToEmpty()
        {
            var gm = GameManager.Instance;
            gm.StartGame();

            var ace = C(Suit.clubs, 1);

            gm.Tableaus[0].cards.Clear();
            gm.Tableaus[0].addCard(ace);

            var f = gm.Foundations.First(x => x.suit == Suit.clubs);

            bool result = gm.TryMove(ace, f);

            Assert.True(result);
            Assert.Contains(ace, f.cards);

            /* This test verifies that a valid move of an Ace from a tableau to an empty foundation is successful. 
            * It creates an Ace of clubs, adds it to the first tableau, and then attempts to move it to the foundation for clubs. 
            * The test asserts that the move is successful and that the Ace is now in the foundation.
            */
        }

        [Fact]
        public void SM1_03_TableauToFoundation_NextRank()
        {
            var gm = GameManager.Instance;
            gm.StartGame();

            var ace = C(Suit.clubs, 1);
            var two = C(Suit.clubs, 2);

            gm.Tableaus[0].cards.Clear();
            gm.Tableaus[0].addCard(two);

            var f = gm.Foundations.First(x => x.suit == Suit.clubs);
            f.cards.Clear();
            f.addCard(ace);

            bool result = gm.TryMove(two, f);

            Assert.True(result);
            Assert.Contains(two, f.cards);

            /* 
             * This test verifies that a valid move of the next rank card (2 of clubs) from a tableau to a foundation 
             * that already has the previous rank card (Ace of clubs) is successful. 
             * It creates an Ace and a 2 of clubs, adds the 2 to the first tableau, and adds the Ace to the foundation for clubs. 
             * Then it attempts to move the 2 to the foundation and asserts that the move is successful and that the 2 is now in the foundation.
             */
        }

        [Fact]
        public void SM1_04_WasteToTableau()
        {
            var black8 = C(Suit.spades, 8);
            var red7 = C(Suit.hearts, 7);

            var gm = GameManager.Instance;
            gm.StartGame();

            gm.Waste.addCard(red7);
            gm.Tableaus[0].addCard(black8);

            bool result = gm.TryMove(red7, gm.Tableaus[0]);

            Assert.True(result);
            Assert.Contains(red7, gm.Tableaus[0].cards);

            /* 
             * This test verifies that a valid move from the waste to a tableau is successful. 
             * It creates a red 7 and a black 8, adds the red 7 to the waste and the black 8 to the first tableau, 
             * and then attempts to move the red 7 onto the black 8. The test asserts that the move is successful 
             * and that the red 7 is now in the tableau.
             */
        }

        [Fact]
        public void SM1_05_WasteToFoundation()
        {
            var ace = C(Suit.clubs, 1);

            var gm = GameManager.Instance;
            gm.StartGame();

            gm.Waste.addCard(ace);
            var f = gm.Foundations.First(x => x.suit == Suit.clubs);

            bool result = gm.TryMove(ace, f);

            Assert.True(result);
            Assert.Contains(ace, f.cards);

            /* 
             * This test verifies that a valid move from the waste to a foundation is successful. 
             * It creates an Ace of clubs, adds it to the waste, and then attempts to move it to the foundation for clubs. 
             * The test asserts that the move is successful and that the Ace is now in the foundation.
             */
        }
    }
}
