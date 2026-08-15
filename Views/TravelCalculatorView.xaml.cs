using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using AuroraDesignSuite.Models;

namespace AuroraDesignSuite.Views
{
    public partial class TravelCalculatorView : UserControl
    {
        public TravelCalculatorView()
        {
            InitializeComponent();
            RecalculateAll();
        }

        private void RecalculateAll()
        {
            RecalculateDistances();
            RecalculateKinematics();
            RecalculateTerraforming();
            RecalculateArmorPenetration();
            RecalculateSensorRange();
            RecalculateCargoLogistics();
        }

        private double ParseDouble(string input, double fallback = 0.0)
        {
            if (string.IsNullOrWhiteSpace(input)) return fallback;
            string clean = input.Replace(',', '.');
            return double.TryParse(clean, NumberStyles.Any, CultureInfo.InvariantCulture, out double val) ? val : fallback;
        }

        private int ParseInt(string input, int fallback = 0)
        {
            if (string.IsNullOrWhiteSpace(input)) return fallback;
            return int.TryParse(input.Trim(), out int val) ? val : fallback;
        }

        // --- 1. Distance Converter & Presets ---
        private void TxtDistanceKm_TextChanged(object sender, TextChangedEventArgs e)
        {
            RecalculateDistances();
        }

        private void RecalculateDistances()
        {
            if (TxtDistanceKm == null || LblValAU == null) return;

            double km = ParseDouble(TxtDistanceKm.Text, 149597870.7);
            var (au, lightSec, lightDays, lightYears, parsecs) = TravelCalculatorEngine.ConvertDistance(km);

            LblValAU.Text = $"{au:N2} AU";
            LblValLightSec.Text = $"{lightSec:N1} Seg";
            LblValLightDays.Text = $"{lightDays:N2} Días";
            LblValLightYears.Text = $"{lightYears:N6} AL";
        }

        private void BtnPresetMoon_Click(object sender, RoutedEventArgs e) => TxtDistanceKm.Text = "384400";
        private void BtnPresetSun_Click(object sender, RoutedEventArgs e) => TxtDistanceKm.Text = "149597870";
        private void BtnPresetNeptune_Click(object sender, RoutedEventArgs e) => TxtDistanceKm.Text = "4500000000";
        private void BtnPresetProxima_Click(object sender, RoutedEventArgs e) => TxtDistanceKm.Text = "40114000000000";

        // --- 2. Kinematics ---
        private void OnKinematicsInputChanged(object sender, TextChangedEventArgs e)
        {
            RecalculateKinematics();
        }

        private void RecalculateKinematics()
        {
            if (TxtTransitDistAU == null || LblTransitHours == null) return;

            double distAU = ParseDouble(TxtTransitDistAU.Text, 10.0);
            double speedKmS = ParseDouble(TxtTransitSpeedKmS.Text, 4500.0);

            var (hours, days, months, percentC, fuelLiters) = TravelCalculatorEngine.CalculateTravelTime(distAU, speedKmS);

            LblTransitHours.Text = $"{hours:N1} Horas";
            LblTransitDays.Text = $"{days:N2} Días";
            LblTransitPercentC.Text = $"{percentC:F2}% c";
        }

        // --- 3. Terraforming ---
        private void OnTerraformInputChanged(object sender, TextChangedEventArgs e)
        {
            RecalculateTerraforming();
        }

        private void RecalculateTerraforming()
        {
            if (TxtCurAtmos == null || LblTerraformYears == null) return;

            double curAtmos = ParseDouble(TxtCurAtmos.Text, 0.20);
            double tgtAtmos = ParseDouble(TxtTgtAtmos.Text, 1.00);
            int terraformers = ParseInt(TxtTerraformersCount.Text, 10);

            var (pressDiff, tempDiff, annualRate, years, months) = TravelCalculatorEngine.CalculateTerraformingTime(curAtmos, tgtAtmos, 15.0, 15.0, terraformers);

            LblTerraformRate.Text = $"{annualRate:F2} atm / Año";
            LblTerraformYears.Text = years < 999 ? $"{years:F2} Años ({months:F0} Meses)" : "Extremadamente Largo";
        }

        // --- 4. Armor Penetration ---
        private void OnArmorInputChanged(object sender, TextChangedEventArgs e)
        {
            RecalculateArmorPenetration();
        }

        private void RecalculateArmorPenetration()
        {
            if (TxtDamagePts == null || LblCraterDepth == null) return;

            double damage = ParseDouble(TxtDamagePts.Text, 16.0);
            int armorLayers = ParseInt(TxtArmorLayersCount.Text, 3);

            var (depth, width, penetrates, status) = TravelCalculatorEngine.CalculateArmorPenetration(damage, armorLayers);

            LblCraterDepth.Text = $"{depth} Capas de Profundidad";
            LblCraterWidth.Text = $"{width} Bloques de Ancho";
            LblArmorPenStatus.Text = status;
            LblArmorPenStatus.Foreground = penetrates ? System.Windows.Media.Brushes.SpringGreen : System.Windows.Media.Brushes.Gold;
        }

        // --- 5. Sensor Range ---
        private void OnSensorInputChanged(object sender, TextChangedEventArgs e)
        {
            RecalculateSensorRange();
        }

        private void RecalculateSensorRange()
        {
            if (TxtSensorHS == null || LblSensorRangeKm == null) return;

            double sensorHS = ParseDouble(TxtSensorHS.Text, 10.0);
            double sensorRes = ParseDouble(TxtSensorRes.Text, 20.0);
            double targetTH = ParseDouble(TxtTargetTH.Text, 500.0);

            var (rangeKm, rangeAU, status) = TravelCalculatorEngine.CalculateSensorRange(sensorHS, sensorRes, targetTH);

            LblSensorRangeKm.Text = $"{rangeKm:N0} km";
            LblSensorRangeAU.Text = $"{rangeAU:F2} AU";
        }

        // --- 6. Cargo Logistics ---
        private void OnCargoInputChanged(object sender, TextChangedEventArgs e)
        {
            RecalculateCargoLogistics();
        }

        private void RecalculateCargoLogistics()
        {
            if (TxtTotalCargoTons == null || LblCargoVoyages == null) return;

            double cargoTons = ParseDouble(TxtTotalCargoTons.Text, 50000.0);
            double shipCap = ParseDouble(TxtShipCapTons.Text, 10000.0);

            var (voyages, fleetSizeReq, totalFuel, totalDays) = TravelCalculatorEngine.CalculateCargoLogistics(cargoTons, shipCap, 4500.0, 10.0);

            LblCargoVoyages.Text = $"{voyages:N0} Viajes";
            LblCargoTotalDays.Text = $"{totalDays:N1} Días";
        }
    }
}
