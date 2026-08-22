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
                    if (list != null && list.Count > 0) return list.OrderByDescending(x => x.CreatedAt).ToList();
                }
            }
            catch { }

            // Default seed entries with rich sub-tasks & project planning
            var defaults = new List<ImperialJournalEntry>
            {
                new ImperialJournalEntry
                {
                    Title = "📜 Directiva Imperial N° 1: Conversión Trans-Newtoniana de la Tierra",
                    Category = "🏭 Plan Industrial",
                    Priority = "Alta",
                    ProjectFolder = "📁 PROYECTO-SOL-CONVERSIÓN",
                    TargetDate = DateTime.Now.AddDays(90),
                    EstimatedDays = 90,
                    RequiredBP = 1500,
                    RequiredMinerals = "Duranium: 750t | Sorium: 500t | Neutronium: 300t",
                    Content = "• Prioridad absoluta: Reconvertir las 750 Minas Convencionales y Fábricas a modelos Trans-Newtonianos.\n• Construir 10 Minas Automatizadas para desplegar en el asteroide Ceres.\n• Asegurar reservas operativas de Sorium y Duranium.",
                    CreatedAt = DateTime.Now.AddDays(-2),
                    SubTasks = new List<ImperialSubTask>
                    {
                        new ImperialSubTask { Title = "Reconvertir 750 Minas Convencionales", IsDone = true, ProgressPercent = 100, ResourceAssignment = "750 Minas Tierra", Notes = "Completado en turno anterior." },
                        new ImperialSubTask { Title = "Construir 10 Minas Automatizadas", IsDone = false, ProgressPercent = 60, ResourceAssignment = "Fábricas Tierra", Notes = "6 de 10 minas terminadas." },
                        new ImperialSubTask { Title = "Desplegar cargueros a Ceres", IsDone = false, ProgressPercent = 0, ResourceAssignment = "Flota Comercial Sol", Notes = "Pendiente de finalización de minas." }
                    }
                },
                new ImperialJournalEntry
                {
                    Title = "⚔️ Plan de Reorganización Naval Sol",
                    Category = "⚔️ Misión Militar",
                    Priority = "Media",
                    ProjectFolder = "📁 DEFENSA-SECTOR-SOL",
                    TargetDate = DateTime.Now.AddDays(180),
                    EstimatedDays = 180,
                    RequiredBP = 3200,
                    RequiredMinerals = "Duranium: 2,500t | Gallicite: 1,800t | Tritium: 600t",
                    Content = "• Botar 4 Destructores de Misiles con radar Res 100 y cañones CIWS.\n• Establecer piquetes de vigilancia pasiva en los Puntos de Salto a Alfa Centauri.\n• Asignar oficiales navales de alto rango a la escuadra Sol.",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    SubTasks = new List<ImperialSubTask>
                    {
                        new ImperialSubTask { Title = "Botar 4 Destructores DDG Artemis", IsDone = false, ProgressPercent = 50, ResourceAssignment = "Astillero Orbital Sol #1", Notes = "2 naves construidas." },
                        new ImperialSubTask { Title = "Desplegar Piquetes de Vigilancia Pasiva", IsDone = true, ProgressPercent = 100, ResourceAssignment = "Punto Salto Alfa Centauri", Notes = "Estación Ojo Celestial desplegada." }
                    }
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
                sb.AppendLine($"**Carpeta:** {entry.ProjectFolder} | **Categoría:** {entry.Category} | **Prioridad:** {entry.PriorityBadge}");
                sb.AppendLine($"**Estado:** {entry.FormattedStatus} | **Progreso Global:** {entry.OverallProgressPercent:F0}%");
                sb.AppendLine($"**Fecha de Creación:** {entry.FormattedDate} | **Fecha Límite:** {(entry.TargetDate.HasValue ? entry.TargetDate.Value.ToString("dd/MM/yyyy") : "Sin Límite")}");
                
                if (!string.IsNullOrWhiteSpace(entry.RequiredMinerals) || entry.RequiredBP > 0)
                {
                    sb.AppendLine($"**Presupuesto Requerido:** {entry.RequiredBP:N0} BP | {entry.RequiredMinerals}");
                }

                sb.AppendLine("\n### 📝 Instrucciones Tácticas:");
                sb.AppendLine(entry.Content);

                if (entry.SubTasks != null && entry.SubTasks.Count > 0)
                {
                    sb.AppendLine("\n### 📋 Desglose de Sub-tareas:");
                    foreach (var sub in entry.SubTasks)
                    {
                        string check = sub.IsDone ? "[x]" : "[ ]";
                        sb.AppendLine($"- {check} **{sub.Title}** ({sub.ProgressPercent:F0}%) - *Asignado:* {sub.ResourceAssignment} | *Notas:* {sub.Notes}");
                    }
                }

                sb.AppendLine("\n---");
            }

            return sb.ToString();
        }
    }
}
