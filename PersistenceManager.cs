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

        public PersistenceManager(string? filePathOverride = null)
        {
            if (!string.IsNullOrWhiteSpace(filePathOverride))
            {
                _filePath = filePathOverride;
                EnsureDirectory(Path.GetDirectoryName(_filePath));
                return;
            }

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string gameFolder = Path.Combine(appDataPath, "PokerGame");
            
            EnsureDirectory(gameFolder);

            _filePath = Path.Combine(gameFolder, "savegame.json");
        }

        private static void EnsureDirectory(string? folder)
        {
            if (string.IsNullOrWhiteSpace(folder)) return;
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
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
                var data = JsonSerializer.Deserialize<SaveData>(json);
                
                // Guard against null or invalid data
                if (data == null || data.Credits <= 0)
                {
                    return new SaveData { Credits = 100, LastPlayed = DateTime.Now };
                }

                return data;
            }
            catch
            {
                // If file is corrupted, return default
                return new SaveData { Credits = 100, LastPlayed = DateTime.Now };
            }
        }
    }
}
