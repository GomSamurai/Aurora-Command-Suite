using System;
using System.Collections.Generic;

namespace AuroraDesignSuite.Models
{
    public class TerraformWorldInfo
    {
        public int PopulationID { get; set; }
        public int SystemBodyID { get; set; }
        public string PopName { get; set; } = string.Empty;
        public double ColonyCost { get; set; }
        public double SurfaceTempKelvin { get; set; }
        public double SurfaceTempCelsius => SurfaceTempKelvin - 273.15;
        public double AtmosPressure { get; set; }
        public double Gravity { get; set; }
        public double HydroExtent { get; set; }

        public string ColonyCostDisplay => ColonyCost <= 0.001 ? "🟢 0.0 (Habitable / Idílico)" : $"🟠 {ColonyCost:N2} CC";
        public string TempDisplay => $"{SurfaceTempCelsius:N1} °C ({SurfaceTempKelvin:N1} K)";
        public string PressureDisplay => $"{AtmosPressure:N3} atm";
        public string HydroDisplay => $"{HydroExtent:N0} % Cobertura de Agua";

        public List<AtmosphericGasInfo> Gases { get; set; } = new List<AtmosphericGasInfo>();

        public override string ToString() => $"{PopName} - {ColonyCostDisplay} ({TempDisplay})";
    }

    public class AtmosphericGasInfo
    {
        public int GasID { get; set; }
        public string GasName { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public double GasAtm { get; set; }
        public bool IsGHGas { get; set; }
        public bool IsAntiGHGas { get; set; }
        public bool IsDangerous { get; set; }
        public bool IsFrozenOut { get; set; }

        public string GasTypeDisplay => IsDangerous
            ? "🔴 Tóxico / Peligroso"
            : (IsGHGas ? "🔥 Invernadero (Calienta)" : (IsAntiGHGas ? "❄️ Anti-Invernadero (Enfría)" : "🟢 Inerte / Respirable"));

        public string StatusDisplay => IsFrozenOut ? "🧊 Congelado en Superficie" : "☁️ Gas Atmosférico Activo";
    }
}
