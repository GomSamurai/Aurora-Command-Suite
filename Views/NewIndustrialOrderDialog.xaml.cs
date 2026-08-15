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
            var items = new List<string>
            {
                "🏗️ Fábrica de Construcción",
                "🤖 Mina Automatizada",
                "⛏️ Mina Convencional",
                "⛽ Refinería de Combustible",
                "🔬 Laboratorio de Investigación",
                "🏛️ Centro Financiero",
                "🌌 Puerto Espacial",
                "🏰 Cuartel General Naval",
                "🚀 Fábrica de Misiles",
                "🛩️ Fábrica de Cazas",
                "⚡ Lanzador de Masa (Mass Driver)",
                "🌍 Instalación de Terraformación"
            };
            CmbTargetInstallation.ItemsSource = items;
            CmbTargetInstallation.SelectedIndex = 0;
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
