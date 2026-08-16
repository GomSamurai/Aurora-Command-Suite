using System;
using System.IO;

namespace AuroraDesignSuite.Services
{
    public static class ApiKeyManager
    {
        private static string GetConfigFilePath()
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config");
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, "gemini_api.config");
        }

        public static string GetApiKey()
        {
            try
            {
                string[] candidates = new[]
                {
                    GetConfigFilePath(),
                    Path.Combine(Directory.GetCurrentDirectory(), "config", "gemini_api.config"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "config", "gemini_api.config"),
                    Path.Combine(Directory.GetCurrentDirectory(), "..", "config", "gemini_api.config"),
                    @"c:\VSCODE\AuroraDesignSuite\config\gemini_api.config"
                };

                foreach (var path in candidates)
                {
                    if (File.Exists(path))
                    {
                        string key = File.ReadAllText(path).Trim();
                        if (!string.IsNullOrWhiteSpace(key)) return key;
                    }
                }
            }
            catch { }
            return string.Empty;
        }

        public static bool SaveApiKey(string apiKey)
        {
            try
            {
                string key = (apiKey ?? "").Trim();
                string path = GetConfigFilePath();
                File.WriteAllText(path, key);

                // Also save to secondary candidate paths if they exist
                try
                {
                    string cwdPath = Path.Combine(Directory.GetCurrentDirectory(), "config", "gemini_api.config");
                    if (Path.GetFullPath(cwdPath) != Path.GetFullPath(path))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(cwdPath)!);
                        File.WriteAllText(cwdPath, key);
                    }
                }
                catch { }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool HasApiKey()
        {
            return !string.IsNullOrWhiteSpace(GetApiKey());
        }
    }
}
