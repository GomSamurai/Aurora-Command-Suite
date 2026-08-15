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
        public double BaseTempC { get; set; }
        public double AtmosPress { get; set; }
        public bool GroundMineralSurvey { get; set; }

        public List<MineralDepositInfo> MineralDeposits { get; set; } = new List<MineralDepositInfo>();

        public double ColonyCost
        {
            get
            {
                if (GravityG < 0.1 || GravityG > 3.0) return 10.0; // Uninhabitable gravity
                double tempCost = Math.Abs(BaseTempC - 15.0) / 20.0;
                double atmosCost = Math.Abs(AtmosPress - 1.0) * 1.5;
                if (BaseTempC >= 0 && BaseTempC <= 30 && AtmosPress >= 0.5 && AtmosPress <= 1.5 && GravityG >= 0.8 && GravityG <= 1.2)
                    return 0.0; // Perfect Earth-like world!
                return Math.Round(Math.Min(10.0, Math.Max(0.0, tempCost + atmosCost)), 2);
            }
        }

        public string ColonyCostDisplay => ColonyCost == 0 ? "★ 0.00 (Ideal / Terrestre)" : $"{ColonyCost:F2} (Hábitat Requerido)";
        public string GravityDisplay => $"{GravityG:F2} G";
        public string TempDisplay => $"{BaseTempC:F1} °C";
        public string AtmosDisplay => $"{AtmosPress:F2} atm";

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
