using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace AuroraDesignSuite.Services
{
    public class CustomThemeData
    {
        public string ThemeName { get; set; } = string.Empty;
        public string Icon { get; set; } = "💾";
        
        public string BgDark { get; set; } = "#0B0E14";
        public string CardBg { get; set; } = "#131924";
        public string CardHeader { get; set; } = "#1B2333";
        
        public string TextPrimary { get; set; } = "#E6EDF3";
        public string TextSecondary { get; set; } = "#8B949E";
        
        public string AccentCyan { get; set; } = "#00F0FF";
        public string AccentAmber { get; set; } = "#FFB700";
        public string AccentGreen { get; set; } = "#00FF88";
        public string AccentRed { get; set; } = "#FF5555";
        public string AccentPurple { get; set; } = "#BF5AF2";
        public string BorderColor { get; set; } = "#30363D";

        public ThemeOption ToThemeOption()
        {
            string displayName = ThemeName.StartsWith("💾") ? ThemeName : $"💾 {ThemeName}";
            return new ThemeOption
            {
                Name = displayName,
                Icon = "💾",
                Category = "💾 Mis Temas Personalizados",
                IsHeader = false,
                IsCustom = true,
                BgDark = BgDark,
                CardBg = CardBg,
                CardHeader = CardHeader,
                TextPrimary = TextPrimary,
                TextSecondary = TextSecondary,
                AccentCyan = AccentCyan,
                AccentAmber = AccentAmber,
                AccentGreen = AccentGreen,
                AccentRed = AccentRed,
                AccentPurple = AccentPurple,
                BorderColor = BorderColor
            };
        }

        public static CustomThemeData FromThemeOption(ThemeOption theme, string customName = "")
        {
            string name = !string.IsNullOrWhiteSpace(customName) ? customName : theme.Name;
            name = name.Replace("───", "").Replace("👑", "").Replace("🌌", "").Replace("☀️", "").Replace("☯️", "").Replace("💎", "").Replace("💾", "").Trim();
            return new CustomThemeData
            {
                ThemeName = name,
                Icon = "💾",
                BgDark = theme.BgDark,
                CardBg = theme.CardBg,
                CardHeader = theme.CardHeader,
                TextPrimary = theme.TextPrimary,
                TextSecondary = theme.TextSecondary,
                AccentCyan = theme.AccentCyan,
                AccentAmber = theme.AccentAmber,
                AccentGreen = theme.AccentGreen,
                AccentRed = theme.AccentRed,
                AccentPurple = theme.AccentPurple,
                BorderColor = theme.BorderColor
            };
        }
    }

    public static class CustomThemeService
    {
        private static string GetFilePath()
        {
            string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AuroraDesignSuite");
            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }
            return Path.Combine(appDataFolder, "custom_themes.json");
        }

        public static List<CustomThemeData> LoadCustomThemesData()
        {
            try
            {
                string filePath = GetFilePath();
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var list = JsonSerializer.Deserialize<List<CustomThemeData>>(json);
                    if (list != null) return list;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading custom themes: {ex.Message}");
            }

            return new List<CustomThemeData>();
        }

        public static void SaveCustomTheme(CustomThemeData themeData)
        {
            try
            {
                var list = LoadCustomThemesData();
                list.RemoveAll(t => t.ThemeName.Equals(themeData.ThemeName, StringComparison.OrdinalIgnoreCase));
                list.Add(themeData);

                string json = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(GetFilePath(), json);

                SyncWithThemeManager();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving custom theme: {ex.Message}");
            }
        }

        public static bool DeleteCustomTheme(string themeName)
        {
            try
            {
                var list = LoadCustomThemesData();
                string cleanName = themeName.Replace("💾", "").Trim();
                int count = list.RemoveAll(t => t.ThemeName.Trim().Equals(cleanName, StringComparison.OrdinalIgnoreCase));
                if (count > 0)
                {
                    string json = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(GetFilePath(), json);
                    SyncWithThemeManager();
                    return true;
                }
            }
            catch { }
            return false;
        }

        public static void SyncWithThemeManager()
        {
            var customDataList = LoadCustomThemesData();
            ThemeManager.RegisterCustomThemes(customDataList.Select(d => d.ToThemeOption()).ToList());
        }

        public static bool ExportThemeToJson(CustomThemeData theme, string filePath, out string msg)
        {
            try
            {
                string json = JsonSerializer.Serialize(theme, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
                msg = $"✅ Tema personalizado '{theme.ThemeName}' exportado correctamente.";
                return true;
            }
            catch (Exception ex)
            {
                msg = $"Error al exportar tema: {ex.Message}";
                return false;
            }
        }

        public static CustomThemeData? ImportThemeFromJson(string filePath, out string msg)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    msg = "El archivo especificado no existe.";
                    return null;
                }

                string json = File.ReadAllText(filePath);
                var theme = JsonSerializer.Deserialize<CustomThemeData>(json);
                if (theme != null && !string.IsNullOrWhiteSpace(theme.ThemeName))
                {
                    msg = $"✅ Tema '{theme.ThemeName}' importado con éxito.";
                    return theme;
                }
                msg = "El archivo no contiene un formato de tema válido.";
            }
            catch (Exception ex)
            {
                msg = $"Error al importar tema: {ex.Message}";
            }
            return null;
        }
    }
}
