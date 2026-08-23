using System;
using System.Collections.Generic;

namespace AuroraDesignSuite.Models
{
    public class ColonyFuelStockpile
    {
        public int PopulationID { get; set; }
        public string PopName { get; set; } = string.Empty;
        public double FuelLiters { get; set; }
        public double SoriumTons { get; set; }

        public string FuelDisplay => $"{FuelLiters:N0} L";
        public string SoriumDisplay => $"{SoriumTons:N0} t (Mineral Sorium Crudo)";

        public string ReserveStatus => FuelLiters switch
        {
            > 10000000 => "🟢 RESERVAS EXCELENTES (Suficiente para operaciones de flota pesada)",
            > 1000000 => "🟡 RESERVAS ADECUADAS (Suficiente para patrullaje regular)",
            _ => "🔴 RESERVAS CRÍTICAS (Se recomienda refinería Sorium urgente)"
        };

        public override string ToString() => $"{PopName} - {FuelDisplay}";
    }

    public class ShipFuelStatus
    {
        public int ShipID { get; set; }
        public string ShipName { get; set; } = string.Empty;
        public string FleetName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public double CurrentFuelLiters { get; set; }
        public double MaxFuelLiters { get; set; }

        public double FuelPercentage => MaxFuelLiters > 0 ? Math.Min(100.0, (CurrentFuelLiters / MaxFuelLiters) * 100.0) : 100.0;
        public string FuelBarDisplay => $"{FuelPercentage:N1}% ({CurrentFuelLiters:N0} / {MaxFuelLiters:N0} L)";

        public string StatusDisplay => FuelPercentage switch
        {
            > 75.0 => "🟢 Tanques Llenos",
            > 30.0 => "🟡 Combustible Operativo",
            > 10.0 => "🟠 Alerta Nivel Bajo (Reabastecimiento Necesario)",
            _ => "🔴 CRÍTICO / SIN COMBUSTIBLE (Nave a la deriva)"
        };
    }
}
