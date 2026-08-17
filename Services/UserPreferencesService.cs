using System;
using System.IO;
using System.Text.Json;

namespace AuroraDesignSuite.Services
{
    public class UserPreferences
    {
        public double WindowWidth { get; set; } = 1400;
        public double WindowHeight { get; set; } = 850;
        public double WindowLeft { get; set; } = -1;
        public double WindowTop { get; set; } = -1;
        public bool IsMaximized { get; set; } = true;
        public string SelectedTheme { get; set; } = "Cyberpunk Obsidian";
        public int SelectedEmpireId { get; set; } = -1;
        public string LastDbPath { get; set; } = string.Empty;
    }

    public static class UserPreferencesService
    {
        private static readonly string PrefsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "AuroraCommandSuite",
            "user_preferences.json"
        );

        public static UserPreferences LoadPreferences()
        {
            try
            {
                if (File.Exists(PrefsFilePath))
                {
                    string json = File.ReadAllText(PrefsFilePath);
                    var prefs = JsonSerializer.Deserialize<UserPreferences>(json);
                    if (prefs != null) return prefs;
                }
            }
            catch { }

            return new UserPreferences();
        }

        public static void SavePreferences(UserPreferences prefs)
        {
            try
            {
                string dir = Path.GetDirectoryName(PrefsFilePath)!;
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                string json = JsonSerializer.Serialize(prefs, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(PrefsFilePath, json);
            }
            catch { }
        }
    }
}
