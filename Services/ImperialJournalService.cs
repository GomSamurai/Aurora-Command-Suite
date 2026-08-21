using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AuroraDesignSuite.Models;
using Newtonsoft.Json;

namespace AuroraDesignSuite.Services
{
    public static class ImperialJournalService
    {
        private static readonly string StoragePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "ImperialJournalNotes.json");

        public static List<ImperialJournalEntry> LoadEntries()
        {
            try
            {
                if (File.Exists(StoragePath))
                {
                    string json = File.ReadAllText(StoragePath);
                    var list = JsonConvert.DeserializeObject<List<ImperialJournalEntry>>(json);
                    if (list != null) return list.OrderByDescending(x => x.CreatedAt).ToList();
                }
            }
            catch { }

            // Default seed entries
            var defaults = new List<ImperialJournalEntry>
            {
                new ImperialJournalEntry
                {
                    Title = "📜 Directiva Imperial N° 1: Conversión Trans-Newtoniana de la Tierra",
                    Category = "🏭 Plan Industrial",
                    Priority = "Alta",
                    Content = "• Prioridad absoluta: Reconvertir las 750 Minas Convencionales y Fábricas a modelos Trans-Newtonianos.\n• Construir 10 Minas Automatizadas para desplegar en el asteroide Ceres.\n• Asegurar reservas de Sorium y Duranium.",
                    CreatedAt = DateTime.Now.AddDays(-2)
                },
                new ImperialJournalEntry
                {
                    Title = "⚔️ Plan de Reorganización Naval Sol",
                    Category = "⚔️ Misión Militar",
                    Priority = "Media",
                    Content = "• Botar 4 Destructores de Misiles con radar Res 100 y cañones CIWS.\n• Establecer piquetes de vigilancia pasiva en los Puntos de Salto a Alfa Centauri.\n• Asignar oficiales navales de alto rango a la escuadra Sol.",
                    CreatedAt = DateTime.Now.AddDays(-1)
                }
            };
            SaveEntries(defaults);
            return defaults;
        }

        public static void SaveEntries(List<ImperialJournalEntry> entries)
        {
            try
            {
                string dir = Path.GetDirectoryName(StoragePath) ?? "";
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                string json = JsonConvert.SerializeObject(entries, Formatting.Indented);
                File.WriteAllText(StoragePath, json);
            }
            catch { }
        }

        public static string ExportToMarkdown(List<ImperialJournalEntry> entries)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("# 📜 DIARIO DE MANDO Y BITÁCORA IMPERIAL DEL EMPERADOR");
            sb.AppendLine($"*Fecha de Exportación: {DateTime.Now:dd/MM/yyyy HH:mm:ss}*\n");
            sb.AppendLine("---");

            foreach (var entry in entries)
            {
                sb.AppendLine($"## {entry.Title}");
                sb.AppendLine($"**Categoría:** {entry.Category} | **Prioridad:** {entry.PriorityBadge} | **Fecha:** {entry.FormattedDate}");
                sb.AppendLine($"**Estado:** {(entry.IsCompleted ? "✅ Completada" : "⏳ En Curso")}\n");
                sb.AppendLine(entry.Content);
                sb.AppendLine("\n---");
            }

            return sb.ToString();
        }
    }
}
