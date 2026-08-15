using System;
using System.Collections.Generic;

namespace AuroraDesignSuite.Models
{
    public class ShipyardTaskInfo
    {
        public int TaskID { get; set; }
        public int ShipyardID { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public double TotalBP { get; set; }
        public double CompletedBP { get; set; }
        public double ProgressPercent => TotalBP > 0 ? Math.Min(100.0, (CompletedBP / TotalBP) * 100.0) : 0;
        public string ProgressDisplay => $"{CompletedBP:N0} / {TotalBP:N0} BP";

        public double RemainingBP => Math.Max(0, TotalBP - CompletedBP);
        public double EstimatedDaysRemaining(double annualBPOutput)
        {
            if (annualBPOutput <= 0) return 999;
            double bpPerDay = annualBPOutput / 365.0;
            return Math.Round(RemainingBP / bpPerDay, 0);
        }

        // Mineral requirement estimates (standard ratio for shipyard tasks)
        public double DuraniumReq => TotalBP * 0.4;
        public double NeutroniumReq => TotalBP * 0.2;
        public double GalliciteReq => TotalBP * 0.25;
        public double UridiumReq => TotalBP * 0.15;
    }

    public class ShipyardComplexInfo
    {
        public int ShipyardID { get; set; }
        public string ShipyardName { get; set; } = string.Empty;
        public int Slipways { get; set; } = 1;
        public double CapacityTons { get; set; } = 5000;
        public int SYType { get; set; } = 1; // 1 = Naval, 2 = Commercial
        public double BuildSpeedBPPerYear { get; set; } = 500;
        public string AssignedCommander { get; set; } = "Ninguno Asignado";
        public double GovernorBonusPercent { get; set; } = 10.0;

        public string TypeDisplay => SYType == 1 ? "⚓ Astillero Naval" : "📦 Astillero Comercial";
        public string CapacityDisplay => $"{CapacityTons:N0} Tons ({Slipways} Gradas)";

        public List<ShipyardTaskInfo> Tasks { get; set; } = new List<ShipyardTaskInfo>();
        public int ActiveTasksCount => Tasks.Count;
        public int FreeSlipways => Math.Max(0, Slipways - ActiveTasksCount);
        public string StatusDisplay => FreeSlipways > 0 
            ? $"🟢 {FreeSlipways} Grada(s) Libre(s) de {Slipways}" 
            : $"🔴 {ActiveTasksCount} Grada(s) Ocupada(s)";

        public double TotalDuraniumNeeded
        {
            get
            {
                double sum = 0;
                foreach (var t in Tasks) sum += t.DuraniumReq * (1.0 - (t.ProgressPercent / 100.0));
                return Math.Round(sum, 1);
            }
        }

        public double TotalNeutroniumNeeded
        {
            get
            {
                double sum = 0;
                foreach (var t in Tasks) sum += t.NeutroniumReq * (1.0 - (t.ProgressPercent / 100.0));
                return Math.Round(sum, 1);
            }
        }

        public double TotalGalliciteNeeded
        {
            get
            {
                double sum = 0;
                foreach (var t in Tasks) sum += t.GalliciteReq * (1.0 - (t.ProgressPercent / 100.0));
                return Math.Round(sum, 1);
            }
        }

        public double TotalUridiumNeeded
        {
            get
            {
                double sum = 0;
                foreach (var t in Tasks) sum += t.UridiumReq * (1.0 - (t.ProgressPercent / 100.0));
                return Math.Round(sum, 1);
            }
        }
    }

    public class EmpireShipyardTelemetry
    {
        public double TotalNavalCapacityTons { get; set; }
        public double TotalCommercialCapacityTons { get; set; }
        public int TotalNavalSlipways { get; set; }
        public int TotalCommercialSlipways { get; set; }
        public int ActiveBuildTasks { get; set; }
        public int FreeSlipways { get; set; }
        public double TotalAnnualBPOutput { get; set; }
        public double GovernorBonusPercent { get; set; }
    }

    public class ShipClassSimpleInfo
    {
        public int ClassID { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public double SizeHS { get; set; }
        public double CostBP { get; set; }
        public bool IsMilitary { get; set; }
        public double MaxSpeedKmS { get; set; } = 1000.0;
        public double TotalFuelLiters { get; set; } = 50000;
        public int TotalCrewRequired { get; set; } = 50;
        public double TotalMSP { get; set; } = 100;
        public double ThermalSignature { get; set; } = 0;
        public double EMSignature { get; set; } = 0;

        public double Tonnage => SizeHS * 50.0;
        public double RangeBillionKm
        {
            get
            {
                if (MaxSpeedKmS <= 0 || TotalFuelLiters <= 0) return 10.0;
                // Standard fuel efficiency calculation
                double litersPerHour = Math.Max(1.0, ThermalSignature * 0.1);
                double hours = TotalFuelLiters / litersPerHour;
                return Math.Round((hours * MaxSpeedKmS * 3600.0) / 1_000_000_000.0, 1);
            }
        }

        public string DisplayText => $"{ClassName} ({Tonnage:N0} t | {CostBP:N0} BP)";
        public override string ToString() => DisplayText;
    }

    public class CommanderBonusItem
    {
        public string Description { get; set; } = string.Empty;
        public double ValuePercent { get; set; }
        public string ValueDisplay => $"+{ValuePercent:F1}%";
    }

    public class CommanderInfo
    {
        public int CommanderID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = "Oficial";
        public int CommanderType { get; set; } // 1=Scientist, 2=Naval, 3=Governor, 4=Ground
        public string TypeDisplay { get; set; } = "Oficial";
        public double Seniority { get; set; }
        public double PromotionScore { get; set; } = 50.0;
        public double LoyaltyRating { get; set; } = 100.0;
        public string AssignmentLocation { get; set; } = "Sin Asignar";
        public bool IsAssigned => !string.IsNullOrEmpty(AssignmentLocation) && !AssignmentLocation.Equals("Sin Asignar", StringComparison.OrdinalIgnoreCase);
        public string StatusDisplay => IsAssigned ? "🟢 Asignado" : "⚪ Disponible";

        public List<CommanderBonusItem> DetailedBonuses { get; set; } = new List<CommanderBonusItem>();

        public string PrimaryBonusDisplay
        {
            get
            {
                if (DetailedBonuses.Count > 0)
                {
                    var top = DetailedBonuses[0];
                    return $"{top.Description} ({top.ValueDisplay})";
                }
                return "+15% Eficiencia General";
            }
        }

        public string RoleIcon => CommanderType switch
        {
            1 => "🎓",
            2 => "⚓",
            3 => "🏛️",
            _ => "⚔️"
        };

        public override string ToString() => $"{RoleIcon} {Name} ({Title})";
    }
}
