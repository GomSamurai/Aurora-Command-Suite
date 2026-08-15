using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using AuroraDesignSuite.Models;

namespace AuroraDesignSuite.Views
{
    public partial class NewShipyardOrderDialog : Window
    {
        public ShipyardComplexInfo Shipyard { get; }
        public ShipClassSimpleInfo? SelectedClass => CmbShipClasses.SelectedItem as ShipClassSimpleInfo;
        public string UnitName => TxtUnitName.Text.Trim();

        public NewShipyardOrderDialog(ShipyardComplexInfo shipyard, List<ShipClassSimpleInfo> availableClasses)
        {
            InitializeComponent();
            Shipyard = shipyard;

            LblTargetShipyard.Text = $"Astillero Destino: {shipyard.ShipyardName} ({shipyard.CapacityTons:N0} t | {shipyard.TypeDisplay})";
            CmbShipClasses.ItemsSource = availableClasses;

            if (availableClasses.Count > 0)
            {
                CmbShipClasses.SelectedIndex = 0;
            }
        }

        private void CmbShipClasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedClass == null) return;

            LblTonnageReq.Text = $"{SelectedClass.Tonnage:N0} Tons";
            LblCostReq.Text = $"{SelectedClass.CostBP:N0} BP";
            TxtUnitName.Text = $"S.M.S. {SelectedClass.ClassName}-01";

            if (SelectedClass.Tonnage > Shipyard.CapacityTons)
            {
                BdrCapacityWarning.Visibility = Visibility.Visible;
            }
            else
            {
                BdrCapacityWarning.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedClass == null)
            {
                MessageBox.Show("Selecciona una clase de nave válida.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(UnitName))
            {
                MessageBox.Show("Ingresa un nombre para la unidad en construcción.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
