using System;

namespace AuroraDesignSuite.Models
{
    public class EmpireInfrastructureItem
    {
        public int InstallationID { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Amount { get; set; }
        public string Category { get; set; } = "Industrial";
        public double AnnualOutputBP { get; set; }

        public string AmountDisplay => $"{Amount:N0} Unidades";
        public string OutputDisplay => AnnualOutputBP > 0 ? $"{AnnualOutputBP:N0} BP / Año" : "N/A";
    }

    public class EmpireFleetSummaryItem
    {
        public int FleetID { get; set; }
        public string FleetName { get; set; } = string.Empty;
        public int ShipCount { get; set; }
        public string FlagshipName { get; set; } = "Sin Insignia";
        public double SpeedKmS { get; set; }
        public double FuelPercent { get; set; }
        public double MoralePercent { get; set; }
        public string SystemLocation { get; set; } = "Sol";

        public string ShipsDisplay => $"{ShipCount} Naves";
        public string SpeedDisplay => $"{SpeedKmS:N0} km/s";
        public string FuelDisplay => $"{FuelPercent:F0}%";
        public string MoraleDisplay => $"{MoralePercent:F0}%";
    }

    public class EmpireOfficerSummary
    {
        public int AdmiralsCount { get; set; }
        public int CaptainsCount { get; set; }
        public int GovernorsCount { get; set; }
        public int ScientistsCount { get; set; }

        public int TotalOfficers => AdmiralsCount + CaptainsCount + GovernorsCount + ScientistsCount;
    }
}
