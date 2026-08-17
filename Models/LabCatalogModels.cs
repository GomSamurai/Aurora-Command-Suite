using System;

namespace AuroraDesignSuite.Models
{
    public class SavedMissileInfo
    {
        public int MissileID { get; set; }
        public string Name { get; set; } = string.Empty;
        public double SizeMSP { get; set; }
        public double SpeedKmS { get; set; }
        public double WarheadDamage { get; set; }
        public double MaxRangeBillionKm { get; set; }
        public double CostBP { get; set; }

        public string SizeDisplay => $"{SizeMSP:F1} MSP ({SizeMSP * 50:N0} t)";
        public string SpeedDisplay => $"{SpeedKmS:N0} km/s";
        public string DamageDisplay => $"{WarheadDamage:F1} Dmg";
        public string RangeDisplay => $"{MaxRangeBillionKm:F2} Millones km";
        public string CostDisplay => $"{CostBP:F2} BP";
    }

    public class SavedEngineInfo
    {
        public int ComponentID { get; set; }
        public string Name { get; set; } = string.Empty;
        public double SizeHS { get; set; }
        public double PowerEP { get; set; }
        public double FuelEfficiency { get; set; }
        public double ThermalSignature { get; set; }
        public double CostBP { get; set; }
        public bool IsCommercial { get; set; }

        public string TypeDisplay => IsCommercial ? "📦 Comercial" : "⚔️ Militar";
        public string SizeDisplay => $"{SizeHS:F1} HS ({SizeHS * 50:N0} t)";
        public string PowerDisplay => $"{PowerEP:N1} EP";
        public string ThermalDisplay => $"{ThermalSignature:N1} W";
        public string CostDisplay => $"{CostBP:F1} BP";
    }

    public class MissilePresetInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = "🚀 Misiles Antinave (ASM)";
        public double SizeMSP { get; set; } = 6.0;
        public double EnginePercent { get; set; } = 40.0;
        public double PowerMod { get; set; } = 2.0;
        public double WarheadMSP { get; set; } = 2.0;
        public double FuelMSP { get; set; } = 1.0;
        public double Agility { get; set; } = 5.0;

        public override string ToString() => $"{Name} ({SizeMSP:F1} MSP)";
    }

    public class EnginePresetInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = "⚡ Motores Militares de Alta Velocidad";
        public bool IsMilitary { get; set; } = true;
        public double SizeHS { get; set; } = 10.0;
        public double PowerMod { get; set; } = 1.25;
        public double ThermalReduction { get; set; } = 1.0;

        public override string ToString() => $"{Name} ({SizeHS:F1} HS)";
    }

    public class ResearchedTechItem
    {
        public int TechID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CategoryID { get; set; }
        public int TechTypeID { get; set; }
        public double AdditionalInfo { get; set; }
        public string Description { get; set; } = string.Empty;

        public override string ToString() => Name;
    }
}
