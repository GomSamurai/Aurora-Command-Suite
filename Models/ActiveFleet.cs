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

        public string CurrentActivity { get; set; } = "🛡️ Patrulla Orbital y Vigilancia Espacial";
        public double NearestColonyDistanceAU { get; set; } = 0.0;
        public string NearestColonyDisplay => NearestColonyDistanceAU <= 0.05 ? "📍 En Órbita Colonial (0.0 AU)" : $"📍 {NearestColonyDistanceAU:F2} AU de la Colonia";
        public string StrategicRecommendation { get; set; } = "🟢 Mantener postura defensiva. Nivel de combustible y suministros de mantenimiento en estado óptimo.";

        public FleetCommanderInfo AssignedCommander { get; set; } = new FleetCommanderInfo();

        public List<ActiveShip> Ships { get; set; } = new List<ActiveShip>();

        public override string ToString() => $"{FleetName} ({ShipCount} Naves, {SystemName})";
    }

    public class FleetCommanderInfo
    {
        public int CommanderID { get; set; }
        public bool HasCommander { get; set; } = false;
        public string Name { get; set; } = "⚠️ Sin Comandante (Flota Inactiva / Sin Naves)";
        public string RankName { get; set; } = "Oficial";
        public string RankAbbrev { get; set; } = "";
        public string FullTitleAndName => HasCommander ? (string.IsNullOrEmpty(RankName) ? Name : $"{RankName} {Name}") : "⚠️ Sin Comandante (Flota Inactiva / Sin Naves)";

        public int Seniority { get; set; } = 0;
        public double Loyalty { get; set; } = 100.0;
        public string HealthStatus { get; set; } = "Salud Normal";
        public int MilitaryKillsTons { get; set; } = 0;
        public int CommercialKillsTons { get; set; } = 0;

        public List<string> Traits { get; set; } = new List<string>();
        public string TraitsDisplay => Traits.Count > 0 ? string.Join(", ", Traits) : "Sin Rasgos Destacados";

        public string PrimaryBonusDisplay { get; set; } = "0% (Sin Naves Asignadas)";
        public string SecondaryBonusDisplay { get; set; } = "0% (Sin Naves Asignadas)";
        public List<string> AllBonuses { get; set; } = new List<string>();
        public string AllBonusesDisplay => AllBonuses.Count > 0 ? string.Join(" • ", AllBonuses) : "Ninguna (Agrupación vacía)";
    }
}
