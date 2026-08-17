using System.Collections.Generic;

namespace AuroraDesignSuite.Models
{
    public enum ProjectSource
    {
        Aurora4XGame,
        AppUserPreset
    }

    public class CustomProjectItem
    {
        public int ProjectID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = "General";
        public ProjectSource Source { get; set; } = ProjectSource.Aurora4XGame;
        public double DevelopmentCostRP { get; set; }
        public double BuildCostBP { get; set; }
        public double SizeHS { get; set; }
        public double SizeTons => SizeHS * 50.0;
        public int Crew { get; set; }
        public int HTK { get; set; } = 1;
        public string SpecificationsSummary { get; set; } = string.Empty;
        public Dictionary<string, double> MineralRequirements { get; set; } = new Dictionary<string, double>();

        public string SourceBadge => Source == ProjectSource.Aurora4XGame ? "🎮 Aurora 4X (Juego)" : "💻 Creado en App";
        public string SourceColor => Source == ProjectSource.Aurora4XGame ? "#00E5FF" : "#FFD700";
        public string CostDisplay => DevelopmentCostRP > 0 ? $"{DevelopmentCostRP:N0} RP" : $"{BuildCostBP:N1} BP";
        public string SizeDisplay => $"{SizeHS:F1} HS ({SizeTons:N0} t)";
    }
}
