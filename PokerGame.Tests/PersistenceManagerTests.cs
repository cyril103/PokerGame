using System;
using System.IO;
using PokerGame;
using Xunit;

namespace PokerGame.Tests
{
    public class PersistenceManagerTests
    {
        [Fact]
        public void Load_ReturnsDefaultWhenFileMissingOrCorrupt()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            string filePath = Path.Combine(tempDir, "savegame.json");

            var persistence = new PersistenceManager(filePath);
            var data = persistence.Load();
            Assert.Equal(100, data.Credits);

            // Write corrupt content
            File.WriteAllText(filePath, "{ not json");
            data = persistence.Load();
            Assert.Equal(100, data.Credits);
        }

        [Fact]
        public void Save_Then_Load_PersistsCredits()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            string filePath = Path.Combine(tempDir, "savegame.json");

            var persistence = new PersistenceManager(filePath);
            persistence.Save(250);

            var data = persistence.Load();
            Assert.Equal(250, data.Credits);
            Assert.True(data.LastPlayed <= DateTime.Now);
        }
    }
}
