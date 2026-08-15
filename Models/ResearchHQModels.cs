using System;
using AuroraDesignSuite.Services;

namespace AuroraDesignSuite.Models
{
    public class TechTreeItemInfo
    {
        public int TechSystemID { get; set; }
        public string TechName { get; set; } = string.Empty;
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = "General";
        public double DevelopCost { get; set; }

        public string Description => TechDescriptionResolver.ResolveDescription(TechName, CategoryName);
        public string CostDisplay => $"{DevelopCost:N0} RP";
        public string FullTooltipDisplay => $"📌 {TechName}\n🏷️ Categoría: {CategoryName}\n💰 Costo: {DevelopCost:N0} RP\n\n📖 {Description}";
        public override string ToString() => $"{TechName} ({DevelopCost:N0} RP)";
    }

    public class ScientistInfo
    {
        public int CommanderID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ResSpecID { get; set; }
        public string FieldName { get; set; } = "General";
        public double BonusPercent { get; set; } = 25.0;
        public int MaxLabs { get; set; } = 10;
        public int Seniority { get; set; }
        public double Loyalty { get; set; }

        public string DisplayName => $"👨‍🔬 {Name} (+{BonusPercent:F0}% - {FieldName} | Max: {MaxLabs} Labs)";
        public override string ToString() => DisplayName;
    }
}
