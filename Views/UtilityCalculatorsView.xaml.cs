using System;
using System.Windows.Controls;
using AuroraDesignSuite.Models;

namespace AuroraDesignSuite.Views
{
    public partial class UtilityCalculatorsView : UserControl
    {
        public UtilityCalculatorsView()
        {
            InitializeComponent();
            RecalculateAll();
        }

        private void OnDistanceKmChanged(object sender, TextChangedEventArgs e)
        {
            RecalculateDistance();
        }

        private void OnTravelInputChanged(object sender, TextChangedEventArgs e)
        {
            RecalculateTravel();
        }

        private void OnTerraformInputChanged(object sender, TextChangedEventArgs e)
        {
            RecalculateTerraform();
        }

        private void RecalculateAll()
        {
            RecalculateDistance();
            RecalculateTravel();
            RecalculateTerraform();
        }

        private void RecalculateDistance()
        {
            if (LblValAu == null || LblValLightDays == null || LblValLightYears == null) return;

            if (double.TryParse(TxtDistKm?.Text, out double km) && km > 0)
            {
                var (au, lightSec, lightDays, lightYears, parsecs) = TravelCalculatorEngine.ConvertDistance(km);
                LblValAu.Text = $"{au:N2} AU";
                LblValLightDays.Text = $"{lightDays:N2} Días-Luz";
                LblValLightYears.Text = $"{lightYears:F6} AL";
            }
            else
            {
                LblValAu.Text = "0 AU";
                LblValLightDays.Text = "0 Días-Luz";
                LblValLightYears.Text = "0 AL";
            }
        }

        private void RecalculateTravel()
        {
            if (LblTravelHours == null || LblTravelDays == null || LblTravelMonths == null) return;

            double.TryParse(TxtTravelDistBillion?.Text, out double distBillionKm);
            double.TryParse(TxtTravelSpeedKmS?.Text, out double speedKmS);

            if (distBillionKm > 0 && speedKmS > 0)
            {
                double distAU = distBillionKm * 1000000000.0 / TravelCalculatorEngine.AU_IN_KM;
                var (hours, days, months, percentC, fuelLiters) = TravelCalculatorEngine.CalculateTravelTime(distAU, speedKmS);
                LblTravelHours.Text = $"{hours:N1} Horas";
                LblTravelDays.Text = $"{days:N2} Días";
                LblTravelMonths.Text = $"{months:N2} Meses";
            }
            else
            {
                LblTravelHours.Text = "0 Horas";
                LblTravelDays.Text = "0 Días";
                LblTravelMonths.Text = "0 Meses";
            }
        }

        private void RecalculateTerraform()
        {
            if (LblPressDiff == null || LblTerraformRate == null || LblTerraformYears == null) return;

            double.TryParse(TxtCurrentPress?.Text, out double curPress);
            double.TryParse(TxtTargetPress?.Text, out double targetPress);
            int.TryParse(TxtTerraformModules?.Text, out int modules);

            if (modules > 0)
            {
                var (pressDiff, tempDiff, annualRate, years, months) = TravelCalculatorEngine.CalculateTerraformingTime(curPress, targetPress, 15.0, 15.0, modules, 0.05);

                LblPressDiff.Text = $"{pressDiff:F2} atm";
                LblTerraformRate.Text = $"{annualRate:F2} atm/año";
                LblTerraformYears.Text = $"{years:F2} Años";
            }
            else
            {
                LblPressDiff.Text = "0 atm";
                LblTerraformRate.Text = "0 atm/año";
                LblTerraformYears.Text = "0 Años";
            }
        }
    }
}
