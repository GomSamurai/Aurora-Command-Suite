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
        public string SoriumDisplay => $"{SoriumTons:N0} t (Sorium Crudo)";

        public string ReserveStatus => FuelLiters switch
        {
            > 10000000 => "🟢 RESERVAS EXCELENTES (Suficiente para flota pesada)",
            > 1000000 => "🟡 RESERVAS ADECUADAS (Operaciones regulares)",
            > 100000 => "🟠 RESERVAS BAJAS (Atención requerida)",
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

        public double FuelPercentage => MaxFuelLiters > 0.001 
            ? Math.Max(0.0, Math.Min(100.0, (CurrentFuelLiters / MaxFuelLiters) * 100.0)) 
            : 0.0;

        public string FuelBarDisplay => MaxFuelLiters > 0.001 
            ? $"{FuelPercentage:N1}% ({CurrentFuelLiters:N0} / {MaxFuelLiters:N0} L)"
            : "0 L (Sin Tanques Instalados)";

        public string StatusDisplay => MaxFuelLiters <= 0.001 
            ? "⚪ N/A (Sin Tanques de Combustible)"
            : FuelPercentage switch
            {
                > 75.0 => "🟢 Tanques Llenos",
                > 30.0 => "🟡 Combustible Operativo",
                > 10.0 => "🟠 Alerta Nivel Bajo (Reabastecimiento)",
                _ => "🔴 CRÍTICO / SIN COMBUSTIBLE (A la deriva)"
            };
    }
}
