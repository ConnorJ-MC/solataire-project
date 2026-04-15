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
        }
    }
}
