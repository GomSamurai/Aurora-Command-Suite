using System;

namespace AuroraDesignSuite.Models
{
    public static class TravelCalculatorEngine
    {
        public const double AU_IN_KM = 149597870.7; // 1 AU in km
        public const double LIGHT_YEAR_IN_KM = 9460730472580.8; // 1 LY in km
        public const double PARSEC_IN_KM = 30856775814913.7; // 1 Parsec in km
        public const double SPEED_OF_LIGHT_KMS = 299792.458; // c in km/s

        public static (double au, double lightSeconds, double lightDays, double lightYears, double parsecs) ConvertDistance(double km)
        {
            if (km <= 0) return (0, 0, 0, 0, 0);

            double au = km / AU_IN_KM;
            double lightSeconds = km / SPEED_OF_LIGHT_KMS;
            double lightDays = lightSeconds / 86400.0;
            double lightYears = km / LIGHT_YEAR_IN_KM;
            double parsecs = km / PARSEC_IN_KM;

            return (au, lightSeconds, lightDays, lightYears, parsecs);
        }

        public static (double hours, double days, double months, double percentC, double fuelLiters) CalculateTravelTime(double distanceAU, double speedKmS, double fuelBurnRate = 1.0)
        {
            if (speedKmS <= 0 || distanceAU <= 0) return (0, 0, 0, 0, 0);

            double totalKm = distanceAU * AU_IN_KM;
            double seconds = totalKm / speedKmS;
            double hours = seconds / 3600.0;
            double days = hours / 24.0;
            double months = days / 30.4375;
            double percentC = (speedKmS / SPEED_OF_LIGHT_KMS) * 100.0;

            // Fuel estimation (liters)
            double fuelLiters = hours * speedKmS * 0.05 * fuelBurnRate;

            return (hours, days, months, percentC, fuelLiters);
        }

        public static (double pressDiff, double tempDiff, double annualRate, double years, double months) CalculateTerraformingTime(double currentAtmos, double targetAtmos, double currentTemp, double targetTemp, int terraformers, double ratePerModuleYear = 0.01)
        {
            if (terraformers <= 0 || ratePerModuleYear <= 0) return (0, 0, 0, 999, 999 * 12);

            double pressDiff = Math.Abs(targetAtmos - currentAtmos);
            double tempDiff = Math.Abs(targetTemp - currentTemp);
            double annualRate = terraformers * ratePerModuleYear;
            double years = (pressDiff + (tempDiff / 10.0)) / annualRate;
            double months = years * 12.0;

            return (pressDiff, tempDiff, annualRate, years, months);
        }

        public static (int craterDepth, int surfaceWidth, bool penetratesArmor, string breachStatus) CalculateArmorPenetration(double damagePoints, int armorLayers)
        {
            if (damagePoints <= 0) return (0, 0, false, "Sin Daño");

            int depth = (int)Math.Floor(Math.Sqrt(damagePoints));
            int width = (int)Math.Ceiling(damagePoints / Math.Max(1, depth));
            bool penetrates = depth >= armorLayers;
            string status = penetrates ? "💥 BRECHA DE BLINDAJE: Daño directo a componentes internos" : $"🛡️ BLINDAJE RESISTE: Restan {armorLayers - depth} capas de blindaje intactas";

            return (depth, width, penetrates, status);
        }

        public static (double rangeKm, double rangeAU, string detectionStatus) CalculateSensorRange(double sensorSizeHS, double resolutionRes, double enemySignatureTH)
        {
            if (sensorSizeHS <= 0 || enemySignatureTH <= 0) return (0, 0, "Sensor Inactivo");

            double baseRangeKm = sensorSizeHS * 1000000.0 * Math.Sqrt(enemySignatureTH / Math.Max(1.0, resolutionRes));
            double rangeAU = baseRangeKm / AU_IN_KM;
            string status = rangeAU >= 1.0 ? $"📡 Cobertura Sistema: {rangeAU:F2} AU" : $"📡 Detección Cercana: {baseRangeKm:N0} km";

            return (baseRangeKm, rangeAU, status);
        }

        public static (double voyages, double fleetSizeReq, double totalFuelConvoy, double totalDays) CalculateCargoLogistics(double totalCargoTons, double shipCapacityTons, double speedKmS, double distanceAU)
        {
            if (shipCapacityTons <= 0 || totalCargoTons <= 0 || speedKmS <= 0) return (0, 0, 0, 0);

            double voyages = Math.Ceiling(totalCargoTons / shipCapacityTons);
            double singleTripKm = distanceAU * AU_IN_KM;
            double roundTripKm = singleTripKm * 2.0;
            double hoursPerTrip = roundTripKm / (speedKmS * 3600.0);
            double totalDays = (hoursPerTrip / 24.0) * voyages;
            double totalFuelConvoy = (hoursPerTrip * speedKmS * 0.08) * voyages;

            return (voyages, Math.Min(voyages, 10), totalFuelConvoy, totalDays);
        }
    }
}
