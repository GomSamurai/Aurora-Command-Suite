using System;

namespace AuroraDesignSuite.Models
{
    public class ColonyInfo
    {
        public int PopulationID { get; set; }
        public string PopName { get; set; } = string.Empty;
        public string SystemName { get; set; } = "Sol";
        public double PopulationMillions { get; set; }
        public bool IsCapital { get; set; }
        public int IndustrialCapacity { get; set; }
        public string TerraformStatus { get; set; } = "Estable";

        public MineralRequirement MineralStockpiles { get; set; } = new MineralRequirement();
        public double FuelStockpile { get; set; }

        public double TotalMineralsTonnage => MineralStockpiles.TotalCost;
        public string DisplayName => IsCapital ? $"★ {PopName} ({PopulationMillions:F2} M)" : $"{PopName} ({PopulationMillions:F2} M)";
    }

    public class MineralDetailItem
    {
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public double Amount { get; set; }
        public double PercentageOfEmpire { get; set; }
        public string GameUtility { get; set; } = string.Empty;
        public string Status { get; set; } = "✅ NOMINAL";
        public string StatusColor { get; set; } = "#55FF55";

        public string AmountDisplay => $"{Amount:N0} t";
        public string PercentageDisplay => $"{PercentageOfEmpire:F1}%";
    }

    public class ResearchProjectInfo
    {
        public int ProjectID { get; set; }
        public string TechName { get; set; } = string.Empty;
        public string ColonyName { get; set; } = string.Empty;
        public int FacilitiesCount { get; set; }
        public double RPAssigned { get; set; }
        public double RPRequired { get; set; }
        public string AssignedScientistName { get; set; } = "Sin Asignar";
        public string ScientistFieldDisplay { get; set; } = "General";

        public double ProgressPercent => RPRequired > 0 ? Math.Min(100.0, (RPAssigned / RPRequired) * 100.0) : 0;
        public string ProgressDisplay => $"{RPAssigned:N0} / {RPRequired:N0} RP ({ProgressPercent:F1}%)";
    }
}
