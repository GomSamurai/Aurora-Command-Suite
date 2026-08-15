using System;
using System.Collections.Generic;
using System.Windows;
using AuroraDesignSuite.Services;

namespace AuroraDesignSuite.Views
{
    public partial class NewIndustrialOrderDialog : Window
    {
        private readonly DatabaseService _dbService;
        private readonly int _raceId;

        public NewIndustrialOrderDialog(DatabaseService dbService, int raceId)
        {
            InitializeComponent();
            _dbService = dbService;
            _raceId = raceId;

            InitializeInstallationsList();
        }

        private void InitializeInstallationsList()
        {
            var dbItems = _dbService.GetAvailablePlanetaryInstallations();
            var items = new List<string>();

            foreach (var name in dbItems)
            {
                string icon = "🏗️";
                if (name.Contains("Mine", StringComparison.OrdinalIgnoreCase)) icon = "🤖";
                else if (name.Contains("Refinery", StringComparison.OrdinalIgnoreCase) || name.Contains("Refuelling", StringComparison.OrdinalIgnoreCase)) icon = "⛽";
                else if (name.Contains("Research", StringComparison.OrdinalIgnoreCase)) icon = "🔬";
                else if (name.Contains("Financial", StringComparison.OrdinalIgnoreCase)) icon = "🏛️";
                else if (name.Contains("Spaceport", StringComparison.OrdinalIgnoreCase) || name.Contains("Shuttle", StringComparison.OrdinalIgnoreCase)) icon = "🌌";
                else if (name.Contains("Naval", StringComparison.OrdinalIgnoreCase) || name.Contains("Military", StringComparison.OrdinalIgnoreCase) || name.Contains("Academy", StringComparison.OrdinalIgnoreCase)) icon = "🏰";
                else if (name.Contains("Ordnance", StringComparison.OrdinalIgnoreCase) || name.Contains("Fighter", StringComparison.OrdinalIgnoreCase)) icon = "🚀";
                else if (name.Contains("Mass", StringComparison.OrdinalIgnoreCase)) icon = "⚡";
                else if (name.Contains("Terraform", StringComparison.OrdinalIgnoreCase)) icon = "🌍";
                else if (name.Contains("Convert", StringComparison.OrdinalIgnoreCase)) icon = "🔄";

                items.Add($"{icon} {name}");
            }

            CmbTargetInstallation.ItemsSource = items;
            if (items.Count > 0)
            {
                CmbTargetInstallation.SelectedIndex = 0;
            }
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (CmbTargetInstallation.SelectedItem is not string selectedBuilding)
            {
                MessageBox.Show("Por favor selecciona una instalación para fabricar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(TxtQuantity.Text, out double quantity) || quantity <= 0)
            {
                MessageBox.Show("Por favor ingresa una cantidad válida mayor a 0.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Strip emoji from target building name for clean DB description
            string cleanName = selectedBuilding;
            int spaceIdx = cleanName.IndexOf(' ');
            if (spaceIdx > 0 && spaceIdx < cleanName.Length - 1)
            {
                cleanName = cleanName.Substring(spaceIdx + 1).Trim();
            }

            if (_dbService.AddIndustrialProject(_raceId, cleanName, quantity, out string msg))
            {
                MessageBox.Show(msg, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
