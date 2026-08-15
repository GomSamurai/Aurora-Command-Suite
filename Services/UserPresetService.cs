using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using AuroraDesignSuite.Models;

namespace AuroraDesignSuite.Services
{
    public class UserPresetData
    {
        public string PresetName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public int PlannedDeploymentMonths { get; set; } = 12;
        public int ArmorThickness { get; set; } = 3;
        public int ArmorWidth { get; set; } = 10;
        public bool IsMilitary { get; set; } = true;
        public List<UserPresetComponentItem> Components { get; set; } = new List<UserPresetComponentItem>();
    }

    public class UserPresetComponentItem
    {
        public int ComponentID { get; set; }
        public string ComponentName { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }

    public static class UserPresetService
    {
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "user_presets.json");

        public static List<UserPresetData> LoadUserPresets()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    string json = File.ReadAllText(FilePath);
                    var list = JsonSerializer.Deserialize<List<UserPresetData>>(json);
                    return list ?? new List<UserPresetData>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading user presets: {ex.Message}");
            }
            return new List<UserPresetData>();
        }

        public static bool SaveUserPreset(UserPresetData preset, out string message)
        {
            try
            {
                var current = LoadUserPresets();
                current.RemoveAll(x => x.PresetName.Equals(preset.PresetName, StringComparison.OrdinalIgnoreCase));
                current.Add(preset);

                string json = JsonSerializer.Serialize(current, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, json);

                message = $"✅ Preset del usuario '{preset.PresetName}' guardado con éxito.";
                return true;
            }
            catch (Exception ex)
            {
                message = $"Error al guardar el preset: {ex.Message}";
                return false;
            }
        }
    }
}
