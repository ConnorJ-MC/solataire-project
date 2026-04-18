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

            /*
             * A helper method to create a card with the specified suit, rank, and face-up status. 
             * By default, the card is created as face up (faceUp = true), but this can be overridden by passing false for the faceUp parameter.
             */
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

            /* This test verifies that moving a card of the same color (red 7 to red 8) onto a tableau is not allowed. 
             * The TryMove method should return false, indicating that the move is invalid according to the game rules. 
             * The test sets up two tableaus, adds the respective cards, and then attempts the move, asserting that it fails as expected.
             */
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

            /* This test checks that moving a non-King card (5 of clubs) to an empty tableau is not allowed. 
            * According to the rules of Solitaire, only a King can be moved to an empty tableau. 
            * The TryMove method should return false, indicating that the move is invalid. 
            * The test initializes the game, clears the first tableau to make it empty, and then attempts to move the 5 of clubs there, 
            * asserting that it fails.
            */
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

            /* This test verifies that moving a non-Ace card (5 of spades) to an empty foundation is not allowed. 
             * In Solitaire, only an Ace can be moved to an empty foundation. 
             * The TryMove method should return false, indicating that the move is invalid. 
             * The test initializes the game, finds the foundation for spades, and then attempts to move the 5 of spades there, 
             * asserting that it fails as expected.
             */
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

            /* This test checks that moving a face-down card (10 of hearts) from one tableau to another is not allowed. 
             * In Solitaire, only face-up cards can be moved. 
             * The TryMove method should return false, indicating that the move is invalid. 
             * The test initializes the game, adds a face-down card to the first tableau, and then attempts to move it to the second tableau, 
             * asserting that it fails as expected.
             */
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

            /* This test verifies that moving a card (3 of diamonds) into the Waste pile is not allowed. 
             * In Solitaire, cards cannot be moved into the Waste pile; they can only be drawn from the Stock to the Waste. 
             * The TryMove method should return false, indicating that the move is invalid. 
             * The test initializes the game, adds a card to the first tableau, and then attempts to move it to the Waste pile, 
             * asserting that it fails as expected.
             */
        }
    }
}
