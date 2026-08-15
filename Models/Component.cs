using System.Collections.Generic;

namespace AuroraDesignSuite.Models
{
    public class Component
    {
        public int ComponentID { get; set; }
        public string ComponentName { get; set; } = string.Empty;
        public int ComponentTypeID { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public double ComponentSize { get; set; } // In HS (1 HS = 50 Tons)
        public double Cost { get; set; }
        public int Crew { get; set; }
        public double EnginePower { get; set; }
        public double FuelCapacity { get; set; } // Liters
        public double FuelEfficiency { get; set; }
        public double ActiveSensor { get; set; }
        public double PassiveSensor { get; set; }
        public double ShieldStrength { get; set; }
        public double JumpRating { get; set; }
        public int JumpMaxHS { get; set; }
        public int CargoCapacity { get; set; }
        public int MaintSupplies { get; set; } // MSP
        public double HangarCapacity { get; set; }
        public double MissileCapacity { get; set; }

        public Dictionary<string, double> MineralCosts { get; set; } = new Dictionary<string, double>();

        public override string ToString() => $"{ComponentName} ({ComponentSize} HS, {Cost} BP)";
    }

    public class SelectedComponentItem
    {
        public Component Component { get; set; } = new Component();
        public int Quantity { get; set; } = 1;

        public double TotalHS => Component.ComponentSize * Quantity;
        public double TotalCost => Component.Cost * Quantity;
        public int TotalCrew => Component.Crew * Quantity;
    }
}
