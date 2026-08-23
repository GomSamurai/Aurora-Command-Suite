using System;
using System.Collections.Generic;

namespace AuroraDesignSuite.Models
{
    public class GroundFormation
    {
        public int FormationID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public int RaceID { get; set; }
        public int PopulationID { get; set; }
        public string LocationName { get; set; } = "En Órbita / Nave";
        public double TotalSizeTons { get; set; }
        public double TotalCostBP { get; set; }
        public int TotalUnits { get; set; }

        public double RequiredTroopTransportHS => Math.Ceiling(TotalSizeTons / 50.0);
        public string TransportRequirementDisplay => $"{TotalSizeTons:N0} t ({RequiredTroopTransportHS:N0} HS de Hangar Tropas)";

        public List<GroundFormationElement> Elements { get; set; } = new List<GroundFormationElement>();

        public override string ToString() => $"{Name} ({Abbreviation}) - {TotalSizeTons:N0} t";
    }

    public class GroundFormationElement
    {
        public int ElementID { get; set; }
        public int FormationID { get; set; }
        public int Units { get; set; }
        public int ClassID { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public double UnitSizeTons { get; set; }
        public double UnitCostBP { get; set; }
        public string BaseTypeName { get; set; } = string.Empty;
        public int Morale { get; set; }
        public double FortificationLevel { get; set; }

        public double TotalSizeTons => UnitSizeTons * Units;
        public double TotalCostBP => UnitCostBP * Units;

        public string UnitWeightDisplay => $"{UnitSizeTons:N1} t ({UnitSizeTons / 50.0:F1} HS)";
        public string TotalWeightDisplay => $"{TotalSizeTons:N0} t ({TotalSizeTons / 50.0:F1} HS)";
    }
}
