namespace AuroraDesignSuite.Models
{
    public class ShipyardInfo
    {
        public string ShipyardName { get; set; } = "Naval Yard Alpha";
        public double SlipwayCapacityTons { get; set; } = 20000;
        public int SlipwayCount { get; set; } = 2;
        public double AnnualBuildRateBP { get; set; } = 1000;

        public double CalculateBuildTimeMonths(double shipCostBP)
        {
            if (AnnualBuildRateBP <= 0) return 0;
            return (shipCostBP / AnnualBuildRateBP) * 12.0;
        }

        public double CalculateRetoolCostBP(double currentShipCostBP, double newShipCostBP)
        {
            // Standard Aurora formula: Retool cost is ~25% of new ship cost or differential
            return newShipCostBP * 0.25;
        }

        public double CalculateRetoolTimeMonths(double retoolCostBP)
        {
            if (AnnualBuildRateBP <= 0) return 0;
            return (retoolCostBP / AnnualBuildRateBP) * 12.0;
        }
    }

    public class FleetCompositionItem
    {
        public ShipDesign Design { get; set; } = new ShipDesign();
        public int Count { get; set; } = 1;

        public double TotalTonnage => Design.TotalTonnage * Count;
        public double TotalCostBP => Design.TotalCostBP * Count;
        public double TotalFuelLiters => Design.TotalFuelLiters * Count;
        public double TotalMSP => Design.TotalMSP * Count;
        public int TotalCrew => Design.TotalCrewRequired * Count;
    }

    public class MissileBlueprint
    {
        public string MissileName { get; set; } = "Viper MK-I ASM";
        public double MissileSizeHS { get; set; } = 6.0; // Standard size 6 missile
        public double EngineSizeHS { get; set; } = 3.0;
        public double EnginePowerModifier { get; set; } = 1.0; // 0.1x to 3.0x
        public double WarheadMSP { get; set; } = 1.5;
        public double FuelHS { get; set; } = 1.0;
        public double AgilityHS { get; set; } = 0.5;

        // Computed
        public double SpeedKmS { get; set; }
        public double RangeMillionKm { get; set; }
        public double WarheadDamage { get; set; }
        public double HitChanceVs5000KmS { get; set; }
        public double TotalCostBP { get; set; }
        public MineralRequirement Minerals { get; set; } = new MineralRequirement();
    }
}
