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

            /*
             * This test verifies that when the setDifficulty method is called with the argument "easy", 
             * it successfully sets the difficulty level to Difficulty.easy. 
             * It checks that the method returns true, indicating success, and that the difficulty property of the LoginManager instance
             * is set to Difficulty.easy.
             */
        }

        [Fact]
        public void SM8_02_SetDifficultyMedium()
        {
            var lm = LoginManager.Instance;
            bool result = lm.setDifficulty("medium");

            Assert.True(result);
            Assert.Equal(Difficulty.medium, lm.difficulty);

            /*
            * This test verifies that when the setDifficulty method is called with the argument "medium", 
            * it successfully sets the difficulty level to Difficulty.medium. 
            * It checks that the method returns true, indicating success, and that the difficulty property of the LoginManager instance
            * is set to Difficulty.medium.
            */
        }

        [Fact]
        public void SM8_03_AgeVertification_Adult()
        {
            var lm = LoginManager.Instance;
            var dob = DateOnly.FromDateTime(DateTime.Now.AddYears(-20)); // 20 years old

            Assert.True(lm.AgeVertification(dob));

            /*
            * This test verifies that the AgeVertification method correctly identifies an adult user. 
            * It creates a DateOnly object representing a date of birth that is 20 years in the past, 
            * and asserts that the AgeVertification method returns true, indicating that the user is considered an adult.
            */
        }

        [Fact]
        public void SM8_04_AgeVertification_Underage()
        {
            var lm = LoginManager.Instance;
            var dob = DateOnly.FromDateTime(DateTime.Now.AddYears(-10)); // 10 years old

            Assert.False(lm.AgeVertification(dob));

            /*
            * This test verifies that the AgeVertification method correctly identifies an underage user.
            * it creates a DateOnly object representing a date of birth that is 10 years in the past,
            * and asserts that the AgeVertification method returns false, indicating that the user is considered underage and should not be allowed to play.
            */
        }

        [Fact]
        public void SM8_05_SetVegasMode()
        {
            var lm = LoginManager.Instance;
            lm.gamble = false;

            bool result = lm.setVegasMode(true);

            Assert.True(result);
            Assert.True(lm.gamble);

            /*
             * This test verifies that the setVegasMode method correctly enables Vegas mode when called with the argument true.
             * It first sets the gamble property to false to ensure that it is in a known state, 
             * then calls setVegasMode(true) and asserts that the method returns true, indicating success, 
             * and that the gamble property is now true, indicating that Vegas mode has been enabled.
             */
        }

        [Fact]
        public void SM8_06_CreateNewPlayer()
        {
            var lm = LoginManager.Instance;

            var playersField = typeof(LoginManager).GetField("players", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var internalList = (List<Player>)playersField.GetValue(lm);
            internalList.Clear();

            var result = lm.LoadPlayerData("John", "Doe", DateOnly.FromDateTime(DateTime.Now.AddYears(-25)));

            Assert.NotNull(result);
            Assert.Equal(result, lm.Players[0]);

            /*
            * This test verifies that the LoadPlayerData method successfully creates a new player when provided with valid input.
            * It first clears the internal list of players to ensure a clean state, then calls LoadPlayerData with a new player's information.
            * The test asserts that the result is not null, indicating that a player was created, 
            * and that the created player is added to the Players list at index 0.
            */
        }

        [Fact]
        public void SM8_07_LoadPlayer()
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

            /*
             * This test verifies that the LoadPlayerData method correctly loads an existing player when the same information is provided.
             * It first clears the internal list of players, then calls LoadPlayerData twice with the same player's information.
             * The test asserts that there is only one player in the Players list, and that both calls to LoadPlayerData return the same player instance, 
             * indicating that the existing player was loaded rather than creating a duplicate.
             */
        }

        [Fact]
        public void SM8_08_SavePlayer()
        {
            var lm = LoginManager.Instance;

            var playersField = typeof(LoginManager).GetField("players", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var internalList = (List<Player>)playersField.GetValue(lm);
            internalList.Clear();

            var player = lm.LoadPlayerData("Alice", "Johnson", DateOnly.FromDateTime(DateTime.Now.AddYears(-22)));
            var before = new Player(player.fName, player.lName, player.dob, player.balance);

            lm.currentPlayer.balance = lm.currentPlayer.balance - 52;

            bool result = lm.savePlayerData(lm.currentPlayer);

            Assert.True(result);
            Assert.NotEqual(before, lm.Players[0]);

            /*
             * This test verifies that the savePlayerData method successfully saves changes to a player's data.
             * It first clears the internal list of players, then creates a new player and stores a copy of their initial state.
             * The test modifies the current player's balance, calls savePlayerData, and asserts that the method returns true, indicating success,
             * and that the player's data in the Players list has been updated and is not equal to the original state.
             */
        }

        [Fact]
        public void SM8_09_NotSavingGuestData()
        {
            var lm = LoginManager.Instance;

            var playersField = typeof(LoginManager).GetField("players", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var internalList = (List<Player>)playersField.GetValue(lm);
            internalList.Clear();

            lm.isGuest = true;

            var player = lm.LoadPlayerData("Guest", "User", DateOnly.FromDateTime(DateTime.Now.AddYears(-20)));
            bool result = lm.savePlayerData(player);

            Assert.False(result);
            Assert.Empty(lm.Players);

            /*
             * This test verifies that the savePlayerData method does not save data for a guest user.
             * It first clears the internal list of players, sets the isGuest property to true, and creates a player with guest information.
             * The test then calls savePlayerData and asserts that it returns false, indicating that guest data was not saved, 
             * and that the Players list remains empty, confirming that no player data was stored for the guest user.
             */
        }
    }
}
