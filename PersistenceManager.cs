using System;
using System.IO;
using System.Text.Json;

namespace PokerGame
{
    public class SaveData
    {
        public int Credits { get; set; }
        public DateTime LastPlayed { get; set; }
    }

    public class PersistenceManager
    {
        private readonly string _filePath;

        public PersistenceManager()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string gameFolder = Path.Combine(appDataPath, "PokerGame");
            
            if (!Directory.Exists(gameFolder))
            {
                Directory.CreateDirectory(gameFolder);
            }

            _filePath = Path.Combine(gameFolder, "savegame.json");
        }

        public void Save(int credits)
        {
            var data = new SaveData
            {
                Credits = credits,
                LastPlayed = DateTime.Now
            };

            string json = JsonSerializer.Serialize(data);
            File.WriteAllText(_filePath, json);
        }

        public SaveData Load()
        {
            if (!File.Exists(_filePath))
            {
                return new SaveData { Credits = 100, LastPlayed = DateTime.Now }; // Default
            }

            try
            {
                string json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<SaveData>(json);
            }
            catch
            {
                // If file is corrupted, return default
                return new SaveData { Credits = 100, LastPlayed = DateTime.Now };
            }
        }
    }
}
