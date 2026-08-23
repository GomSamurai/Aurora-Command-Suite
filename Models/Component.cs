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

        public double UnitTons => ComponentSize * 50.0;
        public string UnitWeightDisplay => $"{ComponentSize:N1} HS ({UnitTons:N0} t)";

        public string KeySpecDisplay
        {
            get
            {
                if (EnginePower > 0) return $"🔥 Potencia: {EnginePower:N0} EP";
                if (FuelCapacity > 0) return $"⛽ Capacidad: {FuelCapacity:N0} L";
                if (ActiveSensor > 0) return $"📡 Sensor: {ActiveSensor:N0}M km";
                if (PassiveSensor > 0) return $"📡 Sensibilidad: {PassiveSensor:N0}";
                if (ShieldStrength > 0) return $"🛡️ Escudo: {ShieldStrength:N0} HP";
                if (MaintSupplies > 0) return $"🛠️ Repuestos: {MaintSupplies:N0} MSP";
                if (JumpMaxHS > 0) return $"🌌 Salto Máx: {JumpMaxHS:N0} HS";
                if (MissileCapacity > 0) return $"🚀 Pañol: {MissileCapacity:N0} Misiles";
                if (CargoCapacity > 0) return $"📦 Carga: {CargoCapacity:N0} t";
                if (HangarCapacity > 0) return $"🛸 Hangar: {HangarCapacity:N0} HS";
                if (ComponentTypeID == 2 || TypeName == "Habitation" || ComponentName.ToLower().Contains("crew quarters")) return "🏠 Alojamiento: 50 Berths";
                return $"📐 {ComponentSize:N1} HS";
            }
        }

        public override string ToString() => $"{ComponentName} ({ComponentSize} HS, {Cost} BP)";
    }

    public class SelectedComponentItem
    {
        public Component Component { get; set; } = new Component();
        public int Quantity { get; set; } = 1;

        public string ComponentName => Component?.ComponentName ?? "Componente Naval";
        public string TypeName => Component?.TypeName ?? "Sistema";
        public string Name => Component?.ComponentName ?? "Componente Naval";

        public double UnitHS => Component?.ComponentSize ?? 0;
        public double UnitTons => UnitHS * 50.0;
        public string UnitWeightDisplay => $"{UnitHS:N1} HS ({UnitTons:N0} t)";

        public double TotalHS => UnitHS * Quantity;
        public double TotalTons => TotalHS * 50.0;
        public string TotalWeightDisplay => $"{TotalHS:N1} HS ({TotalTons:N0} t)";

        public string UnitWeightFormatted => $"{UnitHS:N1} HS ({UnitTons:N0} t)";
        public string TotalWeightFormatted => Quantity > 1 ? $" ➜ {TotalHS:N1} HS ({TotalTons:N0} t)" : "";
        public string CombinedWeightDisplay => Quantity > 1 
            ? $"{UnitHS:N1} HS ({UnitTons:N0} t) ➜ {TotalHS:N1} HS ({TotalTons:N0} t)" 
            : $"{UnitHS:N1} HS ({UnitTons:N0} t)";

        public double UnitCost => Component?.Cost ?? 0;
        public double TotalCost => UnitCost * Quantity;
        public double TotalCostBP => TotalCost;
        public string CostDisplay => Quantity > 1 ? $"{UnitCost:N1} / {TotalCost:N1} BP" : $"{TotalCost:N1} BP";

        public int UnitCrew => Component?.Crew ?? 0;
        public int TotalCrew => UnitCrew * Quantity;
        public string CrewDisplay => Quantity > 1 ? $"{UnitCrew:N0} / {TotalCrew:N0}" : $"{TotalCrew:N0}";

        public string KeySpecDisplay => Component?.KeySpecDisplay ?? "";

        public override string ToString() => Component?.ComponentName ?? "Componente Naval";
    }
}
