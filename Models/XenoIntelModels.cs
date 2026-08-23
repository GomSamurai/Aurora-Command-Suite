using System;
using System.Collections.Generic;

namespace AuroraDesignSuite.Models
{
    public class AlienRaceInfo
    {
        public int AlienRaceID { get; set; }
        public int ViewRaceID { get; set; }
        public string AlienRaceName { get; set; } = string.Empty;
        public string Abbrev { get; set; } = string.Empty;
        public int CommStatus { get; set; }
        public double DiplomaticPoints { get; set; }
        public bool HasTradeTreaty { get; set; }
        public bool HasTechTreaty { get; set; }
        public bool HasGeoTreaty { get; set; }
        public double DamageCaused { get; set; }

        public string CommStatusDisplay => CommStatus switch
        {
            0 => "❓ Idioma Alienígena Desconocido (En Traductores)",
            1 => "🟡 Comunicación Parcial Estabilizada",
            2 => "🟢 Xenodiplomacia Plena Establecida",
            _ => "🔴 Hostilidad y Bloqueo Diplomático"
        };

        public string ThreatLevelDisplay => DamageCaused > 0
            ? "🔴 AMENAZA ROJA (Ataques Previos Registrados)"
            : (DiplomaticPoints < 0 ? "🟠 Relaciones Hostiles / Tensión" : "🟢 Relaciones Estables / Monitoreo");

        public string TreatiesSummary => (HasTradeTreaty || HasTechTreaty || HasGeoTreaty)
            ? $"🤝 Tratados: {(HasTradeTreaty ? "Comercio " : "")}{(HasTechTreaty ? "Tecnología " : "")}{(HasGeoTreaty ? "Geominería" : "")}"
            : "📜 Sin Tratados Firmados";

        public List<AlienClassInfo> Classes { get; set; } = new List<AlienClassInfo>();

        public override string ToString() => $"{AlienRaceName} ({Abbrev}) - {ThreatLevelDisplay}";
    }

    public class AlienClassInfo
    {
        public int AlienClassID { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int MaxSpeedKmS { get; set; }
        public double ThermalSignature { get; set; }
        public int TCS { get; set; }
        public int ArmourStrength { get; set; }
        public int ShieldStrength { get; set; }
        public int ObservedShipCount { get; set; }

        public string SpeedDisplay => MaxSpeedKmS > 0 ? $"{MaxSpeedKmS:N0} km/s" : "Desconocida";
        public string ThermalDisplay => ThermalSignature > 0 ? $"{ThermalSignature:N0} TH" : "Sigilo / N/A";
        public string DefenseSummary => $"🛡️ Armadura Nivel {ArmourStrength} | Escudos {ShieldStrength}";
    }
}
