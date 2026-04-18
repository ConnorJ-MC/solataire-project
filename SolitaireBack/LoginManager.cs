using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace SolitaireBack
{
    public enum Difficulty
    {
        easy = 1,
        medium = 3
    }
    public class LoginManager
    {
        private static LoginManager _instance;
        public static LoginManager Instance => _instance ??= new LoginManager();

        private readonly object _fileLock = new();

        private readonly string jsonFilePath;

        private LoginManager()
        {
            string saveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "NCL",
                "Solitaire");

            Directory.CreateDirectory(saveFolder);

            jsonFilePath = Path.Combine(saveFolder, "players.json");

            // If this is the first run and we have a deployed data file, copy it to AppData
            TryCopyDefaultFromDeploymentData();

            loadAllPlayersFromJSON();

            /*
             * The LoginManager class is responsible for managing player login and data persistence. 
             * It uses a singleton pattern to ensure only one instance exists throughout the application. 
             * The constructor initializes the path to the players.json file in the user's AppData directory, creates the directory if it doesn't exist, 
             * and attempts to copy a default players.json from the deployment location if it doesn't already exist in AppData. 
             * Finally, it loads all player data from the JSON file into memory.
             */
        }

        private void TryCopyDefaultFromDeploymentData()
        {
            try
            {
                // Look for a shipped players.json in the app's base directory (works for ClickOnce or regular publish)
                string baseDir = AppContext.BaseDirectory ?? AppDomain.CurrentDomain.BaseDirectory;
                string candidate = Path.Combine(baseDir, "players.json");

                // Some deployment scenarios (ClickOnce) may place data in a sibling Data directory named "data"
                if (!File.Exists(candidate))
                {
                    string dataDirCandidate = Path.Combine(baseDir, "data", "players.json");
                    if (File.Exists(dataDirCandidate)) candidate = dataDirCandidate;
                }

                if (File.Exists(candidate) && !File.Exists(jsonFilePath))
                {
                    File.Copy(candidate, jsonFilePath);
                }
            }
            catch
            {
                // best-effort: do not block startup if we can't find or copy the file
            }

            /*
             * The TryCopyDefaultFromDeploymentData method attempts to copy a default players.json file from the application's deployment directory 
             * to the user's AppData directory if it doesn't already exist there. 
             * It checks both the base directory and a potential "data" subdirectory for the players.json file, which accommodates different deployment scenarios 
             * such as ClickOnce. 
             * If a source file is found and the destination file doesn't exist, it copies the file. 
             * Any exceptions during this process are caught and ignored to avoid blocking application startup.
             */
        }

        public Difficulty difficulty { get; private set; }
        public bool gamble { get; set; }
        private List<Player> players = new List<Player>();
        public IReadOnlyList<Player> Players => players.AsReadOnly();
        public Player currentPlayer;

        public bool isGuest { get; set; }

        public bool setDifficulty(String diff)
        {
            if (diff == "easy")
            {
                difficulty = Difficulty.easy;
                return true;
            }
            else if (diff == "medium")
            {
                difficulty = Difficulty.medium;
                return true;
            }
            else
            {
                return false;
            }

            /*
             * The setDifficulty method takes a string input representing the desired difficulty level. 
             * It checks if the input is "easy" or "medium" and sets the difficulty property accordingly. 
             * If the input matches one of the valid options, it returns true; otherwise, it returns false to indicate an invalid selection.
             */
        }

        public bool setVegasMode(bool selected)
        {
            if (!selected || isGuest)
            {
                gamble = false;
                return false;
            }
            else
            {
                gamble = true;
                return true;
            }

            /*
             * The setVegasMode method enables or disables the gambling mode based on the input boolean. 
             * If the selected value is false or if the current player is in guest mode, it sets gamble to false and returns false. 
             * If the selected value is true and the player is not a guest, it sets gamble to true and returns true. 
             * This ensures that gambling mode cannot be enabled for guest players.
             */
        }
        public bool AgeVertification(DateOnly dob)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            int age = today.Year - dob.Year;
            if (dob > today.AddYears(-age)) age--;
            return age >= 18;

            /*
            * The AgeVerification method checks if the player is at least 18 years old based on their date of birth. 
            * It calculates the age by comparing the current date with the provided date of birth. 
            * If the calculated age is 18 or older, it returns true; otherwise, it returns false.
            */
        }

        public void setGuestMode(bool slected)
        {
            isGuest = slected;

            if (isGuest)
                currentPlayer = new Player("Guest", "Player", DateOnly.MinValue, 0)
                {
                    balance = 0,
                    gamesP = 0,
                    gamesW = 0,
                    gamesL = 0
                };

            /*
             * The setGuestMode method sets the isGuest property based on the input boolean. 
             * If guest mode is selected, it initializes the currentPlayer property with a new Player object representing a guest player, 
             * with default values for name, date of birth, balance, and game statistics. 
             * This allows the application to operate in a guest mode without saving or loading player data from JSON.
             */
        }

        public Player createPlayer(string fName, string lName, DateOnly dob)
        {
            Player p = new(fName, lName, dob, 0)
            {
                balance = 0,
                gamesP = 0,
                gamesW = 0,
                gamesL = 0
            };

            players.Add(p);

            return p;

            /*
            * The createPlayer method creates a new Player object with the provided first name, last name, and date of birth. 
            * It initializes the player's balance and game statistics to zero. 
            * The new player is added to the in-memory list of players, and the method returns the newly created Player object.
            */
        }

        public bool savePlayerData(Player p)
        {
            if (isGuest) return false; // Guest mode doesn't save to JSON.

            lock (_fileLock)
            {
                // update in-memory list
                int i = players.FindIndex(x => x.fName == p.fName && x.lName == p.lName && x.dob == p.dob);
                if (i >= 0) players[i] = p;
                else players.Add(p);

                return saveAllPlayersToJSON();
            }

            /*
            * The savePlayerData method saves the provided Player object to the JSON file. 
            * If the application is in guest mode, it returns false since guest data should not be saved. 
            * For non-guest players, it locks the file access to ensure thread safety, 
            * updates the in-memory list of players with the provided player data (adding it if it doesn't already exist), 
            * and then calls saveAllPlayersToJSON to persist the changes to the JSON file. 
            * The method returns true if the save operation was successful, or false if it failed.
            */
        }

        public Player LoadPlayerData(string fName, string lName, DateOnly dob)
        {
            if (isGuest) return currentPlayer; // Guest mode doesn't load from JSON.

            currentPlayer = players.
                FirstOrDefault(p => p.fName == fName && p.lName == lName && p.dob == dob)
                ?? createPlayer(fName, lName, dob);

            return currentPlayer;

            /*
             * The LoadPlayerData method retrieves a Player object from the in-memory list based on the provided first name, last name, and date of birth. 
             * If the application is in guest mode, it returns the currentPlayer (which is a guest player). 
             * For non-guest players, it searches the list of players for a match. 
             * If a matching player is found, it sets currentPlayer to that player; 
             * if no match is found, it creates a new player with the provided information and sets currentPlayer to the new player. 
             * Finally, it returns the currentPlayer object.
             */
        }

        private bool jsonLoaded = false;
        /*
         * This flag indicates whether the players.json file has been loaded into memory.
         * It prevents redundant loading attempts and can be used to check if the player data is ready for access.
         */
        

        public bool loadAllPlayersFromJSON()
        {
            if (jsonLoaded)
            {
                System.Diagnostics.Debug.WriteLine("Players JSON already loaded.");
                return true;
            }

            try
            {
                if (!File.Exists(jsonFilePath))
                {
                    System.Diagnostics.Debug.WriteLine($"players.json not found at {jsonFilePath}");
                    jsonLoaded = true;
                    players = new List<Player>();
                    return true;
                }

                string jsonData = File.ReadAllText(jsonFilePath);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                };
                options.Converters.Add(new DateOnlyJsonConverter());

                using (var doc = JsonDocument.Parse(jsonData))
                {
                    var root = doc.RootElement;
                    if (root.ValueKind == JsonValueKind.Array)
                    {
                        // file is a plain array of players
                        players = JsonSerializer.Deserialize<List<Player>>(root.GetRawText(), options) ?? new List<Player>();
                        jsonLoaded = true;
                        System.Diagnostics.Debug.WriteLine($"Loaded {players.Count} player(s) (array) from {jsonFilePath}");
                        foreach (var p in players)
                            System.Diagnostics.Debug.WriteLine($"Player: {p.fName} {p.lName} dob={p.dob} bal={p.balance}");
                        return true;
                    }

                    if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("players", out var playersElem))
                    {
                        // file wraps players in { "players": [...] }
                        players = JsonSerializer.Deserialize<List<Player>>(playersElem.GetRawText(), options) ?? new List<Player>();
                        jsonLoaded = true;
                        System.Diagnostics.Debug.WriteLine($"Loaded {players.Count} player(s) (wrapped) from {jsonFilePath}");
                        foreach (var p in players)
                            System.Diagnostics.Debug.WriteLine($"Player: {p.fName} {p.lName} dob={p.dob} bal={p.balance}");
                        return true;
                    }
                }

                // unsupported structure
                System.Diagnostics.Debug.WriteLine("players.json structure not recognized.");
                players = new List<Player>();
                jsonLoaded = true;
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to load players.json: " + ex);
                players = new List<Player>();
                jsonLoaded = true;
                return false;
            }

            /*
             * The loadAllPlayersFromJSON method loads player data from the players.json file into memory. 
             * It first checks if the data has already been loaded to avoid redundant operations. 
             * If the file does not exist, it initializes an empty player list and marks the data as loaded. 
             * If the file exists, it reads the JSON content and attempts to parse it. 
             * The method supports two JSON structures: a plain array of players or an object containing a "players" array. 
             * If the structure is recognized, it deserializes the player data into the in-memory list and marks it as loaded. 
             * If any exceptions occur during this process, it logs the error, initializes an empty player list, 
             * and marks the data as loaded to prevent further attempts.
             */
        }

        public bool saveAllPlayersToJSON()
        {
            lock (_fileLock)
            {
                try
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    string jsonData = JsonSerializer.Serialize(players, options);

                    // atomic write: write to temp then replace
                    string tmp = jsonFilePath + ".tmp";
                    File.WriteAllText(tmp, jsonData);
                    // Use File.Replace when possible to preserve existing file metadata
                    if (File.Exists(jsonFilePath))
                    {
                        File.Replace(tmp, jsonFilePath, null);
                    }
                    else
                    {
                        File.Move(tmp, jsonFilePath);
                    }

                    return true;
                }
                catch
                {
                    // best-effort save
                    return false;
                }
            }

            /*
            * The saveAllPlayersToJSON method saves the in-memory list of players to the players.json file. 
            * It locks the file access to ensure thread safety during the write operation. 
            * The method serializes the player list into JSON format with indentation for readability. 
            * To ensure an atomic write, it first writes the JSON data to a temporary file and then replaces the original file with the temporary file. 
            * If the original file does not exist, it simply moves the temporary file to the target location. 
            * The method returns true if the save operation was successful, or false if any exceptions occur during the process.
            */
        }
    }

    // Add this DateOnly converter class into the same file (below the LoginManager class or inside it as static/private)
    internal class DateOnlyJsonConverter : System.Text.Json.Serialization.JsonConverter<DateOnly>
    {
        private const string Format = "yyyy-MM-dd";
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString();
                if (DateOnly.TryParseExact(s, Format, null, System.Globalization.DateTimeStyles.None, out var d))
                    return d;
                if (DateOnly.TryParse(s, out d))
                    return d;
            }
            throw new JsonException($"Unable to convert to DateOnly from token: {reader.GetString()}");
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format));
        }

        /*
        * The DateOnlyJsonConverter class is a custom JSON converter for the DateOnly type, which is not natively supported by System.Text.Json. 
        * It defines a specific date format ("yyyy-MM-dd") for serialization and deserialization. 
        * The Read method attempts to parse a string token from JSON into a DateOnly object, 
        * first trying the exact format and then falling back to a more flexible parse. 
        * If parsing fails, it throws a JsonException. 
        * The Write method serializes a DateOnly value into a string using the defined format.
        */
    }
}
