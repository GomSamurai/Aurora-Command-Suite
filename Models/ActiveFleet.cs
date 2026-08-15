using System.Collections.Generic;

namespace AuroraDesignSuite.Models
{
    public class ActiveShip
    {
        public int ShipID { get; set; }
        public string ShipName { get; set; } = string.Empty;
        public int HullNumber { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public double Tonnage { get; set; }
        public double FuelLiters { get; set; }
        public double MaxFuelLiters { get; set; }
        public double CrewMorale { get; set; }
        public double CurrentMSP { get; set; }
        public bool ActiveSensorsOn { get; set; }
        public bool ShieldsActive { get; set; }

        public string DisplayName => $"{ShipName} (#{HullNumber}) - {ClassName} ({Tonnage:N0} t)";
        public string FuelDisplay => $"{FuelLiters:N0} / {MaxFuelLiters:N0} L";
        public string MoraleDisplay => $"{CrewMorale:F0}%";
    }

    public class ActiveFleet
    {
        public int FleetID { get; set; }
        public string FleetName { get; set; } = string.Empty;
        public int RaceID { get; set; }
        public string SystemName { get; set; } = "Sol";
        public double SpeedKmS { get; set; }
        public double TotalFuelLiters { get; set; }
        public double MaxFuelLiters { get; set; }
        public int ShipCount => Ships.Count;
        public double TotalTonnage { get; set; }

        public List<ActiveShip> Ships { get; set; } = new List<ActiveShip>();

        public override string ToString() => $"{FleetName} ({ShipCount} Naves, {SystemName})";
    }
}
