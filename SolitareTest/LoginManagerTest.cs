using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolitaireBack;
using Xunit;

namespace SolitareTest
{
    public class LoginManagerTest : TestBase
    {


        [Fact]
        public void SM8_01_SetDifficultyEasy()
        {
            var lm = LoginManager.Instance;
            bool result = lm.setDifficulty("easy");

            Assert.True(result);
            Assert.Equal(Difficulty.easy, lm.difficulty);
        }

        [Fact]
        public void SM8_02_SetDifficultyMedium()
        {
            var lm = LoginManager.Instance;
            bool result = lm.setDifficulty("medium");

            Assert.True(result);
            Assert.Equal(Difficulty.medium, lm.difficulty);
        }

        [Fact]
        public void SM8_03_AgeVertification_Adult()
        {
            var lm = LoginManager.Instance;
            var dob = DateOnly.FromDateTime(DateTime.Now.AddYears(-20)); // 20 years old

            Assert.True(lm.AgeVertification(dob));
        }

        [Fact]
        public void SM8_04_AgeVertification_Underage()
        {
            var lm = LoginManager.Instance;
            var dob = DateOnly.FromDateTime(DateTime.Now.AddYears(-10)); // 10 years old

            Assert.False(lm.AgeVertification(dob));
        }

        [Fact]
        public void SM8_05_SetVegasMode()
        {
            var lm = LoginManager.Instance;
            lm.gamble = false;

            lm.setVegasMode(true);

            Assert.True(lm.gamble);
        }

        [Fact]
        public void SM8_06_CreateNewPlayer()
        {
            var lm = LoginManager.Instance;

            var result = lm.LoadPlayerData("John", "Doe", DateOnly.FromDateTime(DateTime.Now.AddYears(-25)));

            Assert.NotNull(result);
            Assert.Equal("John", lm.currentPlayer.fName);
        }

        [Fact]
        public void SM8_07_NoDupes()
        {
            var lm = LoginManager.Instance;

            var playersField = typeof(LoginManager).GetField("players", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    
            var internalList = (List<Player>)playersField.GetValue(lm);
            internalList.Clear();

            var result1 = lm.LoadPlayerData("Jane", "Smith", DateOnly.FromDateTime(DateTime.Now.AddYears(-30)));
            var result2 = lm.LoadPlayerData("Jane", "Smith", DateOnly.FromDateTime(DateTime.Now.AddYears(-30)));

            Assert.Single(lm.Players);
            Assert.Equal(result1, lm.Players[0]);
            Assert.Equal(result2, lm.Players[0]);
        }

        [Fact]
        public void SM8_08_SavePlayer()
        {
            var lm = LoginManager.Instance;

            var playersField = typeof(LoginManager).GetField("players", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var internalList = (List<Player>)playersField.GetValue(lm);
            internalList.Clear();

            var player = lm.createPlayer("Alice", "Johnson", DateOnly.FromDateTime(DateTime.Now.AddYears(-22)));
            bool result = lm.savePlayerData(player);

            Assert.True(result);
            Assert.Equal(player, lm.Players[0]);
        }
    }
}
