using System;
using System.IO;

namespace AuroraDesignSuite.Services
{
    public static class ApiKeyManager
    {
        private static readonly string ConfigDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config");
        private static readonly string KeyFilePath = Path.Combine(ConfigDir, "gemini_api.config");

        public static string GetApiKey()
        {
            try
            {
                if (File.Exists(KeyFilePath))
                {
                    string key = File.ReadAllText(KeyFilePath).Trim();
                    return key;
                }
            }
            catch { }
            return string.Empty;
        }

        public static bool SaveApiKey(string apiKey)
        {
            try
            {
                Directory.CreateDirectory(ConfigDir);
                File.WriteAllText(KeyFilePath, apiKey.Trim());
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
