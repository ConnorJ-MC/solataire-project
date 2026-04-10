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
        }

        public Difficulty difficulty { get; private set; }
        public bool gamble { get; set; }
        private List<Player> players = new List<Player>();
        public IReadOnlyList<Player> Players => players.AsReadOnly();
        public Player currentPlayer;
        //private String tempNewFName;
        //private String tempNewLName;
        //private DateOnly tempNewDOB;
        //private int newBalance;
        //private int newGamesP;
        //private int newGamesW;
        //private int newGamesL;

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
        }

        public void setVegasMode(bool selected)
        {
            if (!selected || isGuest)
            {
                gamble = false;
            }
            else
            {
                gamble = true;
            }
        }
        public bool AgeVertification(DateOnly dob)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            int age = today.Year - dob.Year;
            if (dob > today.AddYears(-age)) age--;
            return age >= 18;
        }

        public void setGuestMode(bool slected)
        {
            isGuest = slected;

            currentPlayer = new Player("Guest", "Player", DateOnly.MinValue, 0)
            {
                balance = 0,
                gamesP = 0,
                gamesW = 0,
                gamesL = 0
            };
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
        }

        public bool savePlayerData(Player p)
        {
            lock (_fileLock)
            {
                // update in-memory list
                int idx = players.FindIndex(x => x.fName == p.fName && x.lName == p.lName && x.dob == p.dob);
                if (idx >= 0) players[idx] = p;
                else players.Add(p);

                return saveAllPlayersToJSON();
            }
        }

        public Player LoadPlayerData(string fName, string lName, DateOnly dob)
        {
            if (isGuest)
            {
                currentPlayer = new Player("Guest", "Player", DateOnly.MinValue, 0)
                {
                    balance = 0,
                    gamesP = 0,
                    gamesW = 0,
                    gamesL = 0
                };

                return currentPlayer; // Guest mode doesn't load from JSON.
            }

            currentPlayer = players.
                FirstOrDefault(p => p.fName == fName && p.lName == lName && p.dob == dob)
                ?? createPlayer(fName, lName, dob);

            return currentPlayer;
        }

        private bool jsonLoaded = false;
        public bool jsonSaved = true; //will start true opon launch, but will be switch to false when a game is won/losed/reset to ensure player data is saved

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
    }
}
