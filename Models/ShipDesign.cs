using System.Collections.Generic;

namespace AuroraDesignSuite.Models
{
    public class ShipDesign
    {
        public string ClassName { get; set; } = "Nuevas Golondrinas";
        public string HullType { get; set; } = "Cruiser";
        public bool IsMilitary { get; set; } = true;
        public int PlannedDeploymentMonths { get; set; } = 12;

        // Size & Movement
        public double TotalHS { get; set; }
        public double TotalTonnage => TotalHS * 50.0;
        public double TotalEnginePower { get; set; }
        public double MaxSpeedKmS { get; set; }
        public double ThermalSignature { get; set; }
        public double EMSignature { get; set; }

        // Fuel & Range
        public double TotalFuelLiters { get; set; }
        public double FuelConsumptionLitersPerHour { get; set; }
        public double RangeBillionKm { get; set; }
        public double RangeAU => RangeBillionKm / 0.14959787;
        public double RangeLightYears => RangeBillionKm / 9460.73;
        public double FlightDaysAtFullSpeed { get; set; }

        // Maintenance & Reliability
        public double TotalMSP { get; set; }
        public double EngineeringSpacesHS { get; set; }
        public double AnnualFailureRate { get; set; }
        public double MTBFMonths { get; set; }
        public double MaintenanceLifeYears { get; set; }

        // Defense & Armor
        public int ArmorThickness { get; set; } = 1;
        public int ArmorWidth { get; set; } = 1;
        public double ShieldStrength { get; set; }
        public double ShieldRechargeRate { get; set; }
        public double DamageControlRating { get; set; }

        // Jump & Support
        public bool HasJumpDrive { get; set; }
        public int MaxJumpSquadron { get; set; }
        public double MaxJumpDistanceLightYears { get; set; }
        public double HangarCapacityTons { get; set; }
        public double MagazineCapacity { get; set; }

        // Cost & Crew
        public double TotalCostBP { get; set; }
        public int TotalCrewRequired { get; set; }
        public int CrewQuartersProvidedHS { get; set; }

        // Minerals
        public MineralRequirement Minerals { get; set; } = new MineralRequirement();

        // Components List
        public List<SelectedComponentItem> Components { get; set; } = new List<SelectedComponentItem>();

        // Design Rule Warnings
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Suggestions { get; set; } = new List<string>();
    }
}
