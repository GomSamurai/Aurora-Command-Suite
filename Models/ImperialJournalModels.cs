using System;

namespace AuroraDesignSuite.Models
{
    public class ImperialJournalEntry
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = "⚔️ Misión Militar";
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Priority { get; set; } = "Media";
        public bool IsCompleted { get; set; } = false;

        public string FormattedDate => CreatedAt.ToString("dd/MM/yyyy HH:mm");
        public string PriorityBadge => Priority switch
        {
            "Alta" => "🔴 Prioridad Alta",
            "Crítica" => "💥 Crítica",
            "Baja" => "🟢 Prioridad Baja",
            _ => "🟡 Prioridad Media"
        };
    }
}
