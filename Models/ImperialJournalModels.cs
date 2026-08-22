using System;
using System.Collections.Generic;
using System.Linq;

namespace AuroraDesignSuite.Models
{
    public class ImperialSubTask
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Title { get; set; } = string.Empty;
        public bool IsDone { get; set; } = false;
        public double ProgressPercent { get; set; } = 0.0;
        public string ResourceAssignment { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }

    public class ImperialJournalEntry
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = "⚔️ Misión Militar";
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Priority { get; set; } = "Media";
        public bool IsCompleted { get; set; } = false;

        // Project Planner & Hierarchy Extensions
        public string ProjectFolder { get; set; } = "📁 General";
        public DateTime? TargetDate { get; set; }
        public double EstimatedDays { get; set; } = 30;
        public double RequiredBP { get; set; } = 0;
        public string RequiredMinerals { get; set; } = string.Empty;

        public List<ImperialSubTask> SubTasks { get; set; } = new List<ImperialSubTask>();

        // Computed Properties & UI Helpers
        public string FormattedDate => CreatedAt.ToString("dd/MM/yyyy HH:mm");

        public double OverallProgressPercent
        {
            get
            {
                if (IsCompleted) return 100.0;
                if (SubTasks == null || SubTasks.Count == 0) return 0.0;
                return Math.Round(SubTasks.Average(t => t.IsDone ? 100.0 : Math.Clamp(t.ProgressPercent, 0, 100)), 1);
            }
        }

        public string SubTaskSummary
        {
            get
            {
                if (SubTasks == null || SubTasks.Count == 0)
                {
                    return IsCompleted ? "1/1 Tareas (100%)" : "0 Tareas";
                }
                int done = SubTasks.Count(t => t.IsDone);
                return $"{done}/{SubTasks.Count} Tareas ({OverallProgressPercent:F0}%)";
            }
        }

        public string PriorityBadge => Priority switch
        {
            "Alta" => "🔴 Prioridad Alta",
            "Crítica" => "💥 Crítica",
            "Baja" => "🟢 Prioridad Baja",
            _ => "🟡 Prioridad Media"
        };

        public string FormattedStatus => IsCompleted ? "✅ COMPLETADA" : (OverallProgressPercent > 0 ? $"⏳ {OverallProgressPercent:F0}% EN CURSO" : "📝 PENDIENTE");

        public string CardBackgroundHex => IsCompleted ? "#0D3728" : "#161E2E";
        public string CardBorderHex => IsCompleted ? "#10B981" : "#2D3748";
        public string TitleForegroundHex => IsCompleted ? "#34D399" : "#38BDF8";
        public string StatusBadgeColorHex => IsCompleted ? "#10B981" : (OverallProgressPercent > 0 ? "#F59E0B" : "#9CA3AF");
    }
}
