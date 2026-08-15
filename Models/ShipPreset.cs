using System.Collections.Generic;

namespace AuroraDesignSuite.Models
{
    public class PresetComponentRef
    {
        public int ComponentID { get; set; }
        public string ComponentName { get; set; } = string.Empty;
        public string TypeName { get; set; } = "General";
        public int Quantity { get; set; } = 1;
        public double ComponentSize { get; set; } = 1.0;
        public double Cost { get; set; } = 1.0;
    }

    public class ShipPreset
    {
        public string PresetName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ArmorThickness { get; set; } = 3;
        public int ArmorWidth { get; set; } = 10;
        public bool IsMilitary { get; set; } = true;
        public List<PresetComponentRef> Components { get; set; } = new List<PresetComponentRef>();

        public override string ToString() => PresetName;
    }

    public class UserBlueprint
    {
        public string BlueprintID { get; set; } = System.Guid.NewGuid().ToString();
        public string ClassName { get; set; } = "Clase Personalizada";
        public int PlannedDeploymentMonths { get; set; } = 12;
        public int ArmorThickness { get; set; } = 3;
        public int ArmorWidth { get; set; } = 10;
        public bool IsMilitary { get; set; } = true;
        public double TotalTonnage { get; set; }
        public double TotalCostBP { get; set; }
        public List<SelectedComponentItem> Components { get; set; } = new List<SelectedComponentItem>();
    }

    public class TechTreeItem
    {
        public int TechSystemID { get; set; }
        public string TechName { get; set; } = string.Empty;
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = "General";
        public double DevelopCost { get; set; }
        public string Description { get; set; } = string.Empty;

        public string DisplayName => $"{TechName} ({DevelopCost:N0} RP)";
    }
}
