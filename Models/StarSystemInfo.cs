using System;
using System.Collections.Generic;

namespace AuroraDesignSuite.Models
{
    public class StarSystemInfo
    {
        public int SystemID { get; set; }
        public int SystemNumber { get; set; }
        public string SystemName { get; set; } = string.Empty;
        public int StarCount { get; set; }
        public double AbundanceModifier { get; set; }
        public int DiscoveredBodiesCount { get; set; }

        public List<SystemBodyInfo> Bodies { get; set; } = new List<SystemBodyInfo>();
        public List<JumpPointInfo> JumpPoints { get; set; } = new List<JumpPointInfo>();

        public override string ToString() => SystemName;
    }

    public class SystemBodyInfo
    {
        public int SystemBodyID { get; set; }
        public int SystemID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string BodyTypeName { get; set; } = string.Empty;
        public double RadiusKm { get; set; }
        public double GravityG { get; set; }
        public double BaseTempK { get; set; }
        public double SurfaceTempK { get; set; }
        public double AtmosPress { get; set; }
        public bool GroundMineralSurvey { get; set; }
        public double RecordedColonyCost { get; set; } = -1;

        // Advanced Astrophysics & Environmental Properties
        public double Density { get; set; }
        public double MassEarth { get; set; }
        public double EscapeVelRel { get; set; }
        public double OrbitalDistAU { get; set; }
        public double YearHours { get; set; }
        public double DayValueHours { get; set; }
        public bool TidalLock { get; set; }
        public int TectonicActivity { get; set; }
        public double MagneticField { get; set; }
        public double HydroExt { get; set; }
        public double Albedo { get; set; }
        public double GHFactor { get; set; }
        public double RadiationLevel { get; set; }
        public double DustLevel { get; set; }
        public int RuinID { get; set; }
        public int AbandonedFactories { get; set; }

        public List<MineralDepositInfo> MineralDeposits { get; set; } = new List<MineralDepositInfo>();

        // Surface Temperature converted from Kelvin to Celsius
        public double SurfaceTempC => SurfaceTempK > 0 ? (SurfaceTempK - 273.15) : (BaseTempK > 0 ? (BaseTempK - 273.15) : 15.0);
        public double SurfaceTempKelvin => SurfaceTempK > 0 ? SurfaceTempK : BaseTempK;
        public double OrbitalPeriodDays => YearHours / 24.0;
        public double OrbitalPeriodYears => OrbitalPeriodDays / 365.25;

        public string GravityDisplay => $"{GravityG:F2} G";
        public string TempDisplay => $"{SurfaceTempC:F1} °C";
        public string AtmosDisplay => $"{AtmosPress:F2} atm";
        public string RadiusDisplay => $"{RadiusKm:N0} km";
        public string MassDisplay => MassEarth >= 0.01 ? $"{MassEarth:F2} M⊕" : $"{MassEarth:F4} M⊕";
        public string OrbitalDistDisplay => OrbitalDistAU >= 1000 ? $"{OrbitalDistAU / 149597870:F2} UA" : $"{OrbitalDistAU:F2} UA";
        public string YearDisplay => OrbitalPeriodYears >= 1.0 ? $"{OrbitalPeriodYears:F1} Años ({OrbitalPeriodDays:N0} d)" : $"{OrbitalPeriodDays:F1} Días";
        public string DayDisplay => DayValueHours >= 24 ? $"{DayValueHours / 24.0:F1} d ({DayValueHours:N0} h)" : $"{DayValueHours:F1} h";
        public string HydroDisplay => HydroExt > 0 ? $"{HydroExt:F1}% Océanos" : "0% (Seco)";
        public string EscapeVelDisplay => $"{EscapeVelRel * 11.186:F1} km/s";
        public string MagneticFieldDisplay => MagneticField >= 1.0 ? "🛡️ Fuerte (1.0)" : (MagneticField > 0 ? $"⚠️ Débil ({MagneticField:F1})" : "🚫 Ausente");
        public string TidalLockDisplay => TidalLock ? "🔒 Acoplado" : "🔄 Libre";
        public string RuinsDisplay => RuinID > 0 ? "🏛️ Detectadas" : "Sin Ruinas";
        public string FactoriesDisplay => AbandonedFactories > 0 ? $"🏭 {AbandonedFactories} Fábricas" : "Ninguna";
        public string SurveyStatusDisplay => GroundMineralSurvey ? "✅ Completa" : "⚠️ Pendiente";

        public double ColonyCost
        {
            get
            {
                // If an existing colony explicitly records ColonyCost in AuroraDB.db:
                if (RecordedColonyCost >= 0)
                {
                    return Math.Round(RecordedColonyCost, 2);
                }

                // Uninhabitable gravity for standard terrestrial life (< 0.10 G or > 1.90 G)
                if (GravityG < 0.10 || GravityG > 1.90) return 10.0;

                double tempK = SurfaceTempK > 0 ? SurfaceTempK : BaseTempK;
                // Standard Human / Terrestrial Species Tolerances in Kelvin:
                // Min Temp = 263.03 K (-10.12°C), Max Temp = 311.03 K (+37.88°C), Dev = 24.0 K
                double minTempK = 263.03;
                double maxTempK = 311.03;
                double tempDevK = 24.0;

                double tempCost = 0.0;
                if (tempK > 0)
                {
                    if (tempK < minTempK)
                    {
                        tempCost = (minTempK - tempK) / tempDevK;
                    }
                    else if (tempK > maxTempK)
                    {
                        tempCost = (tempK - maxTempK) / tempDevK;
                    }
                }

                double atmosCost = 0.0;
                if (AtmosPress == 0)
                {
                    atmosCost = 1.0;
                }
                else if (Math.Abs(AtmosPress - 1.0) > 0.5)
                {
                    atmosCost = Math.Abs(AtmosPress - 1.0) * 0.5;
                }

                return Math.Round(Math.Min(10.0, Math.Max(0.0, tempCost + atmosCost)), 2);
            }
        }

        public string ColonyCostDisplay => ColonyCost == 0 ? "★ 0.00 (Ideal / Terrestre)" : $"{ColonyCost:F2} (Hábitat Requerido)";

        public string DepositsSummary
        {
            get
            {
                if (MineralDeposits.Count == 0) return "Sin yacimientos prospectados";
                return $"{MineralDeposits.Count} Minerales Encontrados";
            }
        }

        public override string ToString() => $"{Name} ({ColonyCostDisplay})";
    }

    public class MineralDepositInfo
    {
        public int MaterialID { get; set; }
        public string MineralName { get; set; } = string.Empty;
        public double Amount { get; set; }
        public double Accessibility { get; set; }

        public string AccessibilityDisplay => $"{Accessibility:F2} (Acceso)";
        public string AmountDisplay => $"{Amount:N0} Toneladas";
    }

    public class JumpPointInfo
    {
        public int JumpPointID { get; set; }
        public int SystemID { get; set; }
        public string DestinationSystemName { get; set; } = "Desconocido";
        public bool HasJumpGate { get; set; }
        public bool SurveyDone { get; set; }

        public string GateDisplay => HasJumpGate ? "🌌 Puerta de Salto Activa" : "🌀 Punto Natural";
        public string StatusDisplay => SurveyDone ? "✅ Escaneado" : "⚠️ Pendiente de Prospección";
    }
}
