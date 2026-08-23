using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using AuroraDesignSuite.Models;
using AuroraDesignSuite.Services;

namespace AuroraDesignSuite.Views
{
    public partial class GroundForcesView : UserControl
    {
        private DatabaseService? _dbService;
        private int _raceId;
        private List<GroundFormation> _allFormations = new List<GroundFormation>();

        public GroundForcesView()
        {
            InitializeComponent();
        }

        public void LoadData(DatabaseService dbService, int raceId)
        {
            _dbService = dbService;
            _raceId = raceId;
            RefreshData();
        }

        public void RefreshData()
        {
            if (_dbService == null || _raceId <= 0) return;

            _allFormations = _dbService.GetGroundFormations(_raceId);
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (DgFormations == null) return;

            string query = TxtSearchFormation?.Text?.Trim().ToLower() ?? "";
            var filtered = _allFormations.Where(f =>
                string.IsNullOrEmpty(query) ||
                f.Name.ToLower().Contains(query) ||
                f.Abbreviation.ToLower().Contains(query) ||
                f.LocationName.ToLower().Contains(query)
            ).ToList();

            DgFormations.ItemsSource = filtered;

            double totalTons = filtered.Sum(f => f.TotalSizeTons);
            int totalUnits = filtered.Sum(f => f.TotalUnits);
            if (TxtSummaryBanner != null)
            {
                TxtSummaryBanner.Text = $"{filtered.Count} Formaciones | {totalUnits:N0} Tropas | {totalTons:N0} t Totales";
            }

            if (filtered.Count > 0 && DgFormations.SelectedItem == null)
            {
                DgFormations.SelectedIndex = 0;
            }
        }

        private void DgFormations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgFormations.SelectedItem is not GroundFormation selected) return;

            if (_dbService != null)
            {
                var elements = _dbService.GetGroundFormationElements(selected.FormationID);
                DgElements.ItemsSource = elements;

                if (TxtSelectedFormationHeader != null)
                {
                    TxtSelectedFormationHeader.Text = $"🔍 {selected.Name} ({selected.Abbreviation}) - COMPOSICIÓN TÁCTICA";
                }

                if (LblCalcTotalTons != null) LblCalcTotalTons.Text = $"{selected.TotalSizeTons:N0} t";
                if (LblCalcTroopHS != null) LblCalcTroopHS.Text = $"{selected.RequiredTroopTransportHS:N0} HS ({selected.RequiredTroopTransportHS * 50:N0} t)";

                double selectedCapacityTons = 5000.0;
                if (CmbTransportCapacity?.SelectedIndex == 1) selectedCapacityTons = 10000.0;
                else if (CmbTransportCapacity?.SelectedIndex == 2) selectedCapacityTons = 25000.0;
                else if (CmbTransportCapacity?.SelectedIndex == 3) selectedCapacityTons = 50000.0;

                double shipsNeeded = Math.Ceiling(selected.TotalSizeTons / selectedCapacityTons);
                if (LblCalcShipsNeeded != null) LblCalcShipsNeeded.Text = $"{shipsNeeded:N0} Naves ({selectedCapacityTons:N0}t cap. c/u)";

                if (TxtTacticalNote != null)
                {
                    TxtTacticalNote.Text = $"💡 La formación '{selected.Name}' desplegada en '{selected.LocationName}' consta de {selected.TotalUnits:N0} unidades de combate con un peso total de {selected.TotalSizeTons:N0} toneladas. Requiere {shipsNeeded:N0} naves de transporte (capacidad {selectedCapacityTons:N0} t / {selectedCapacityTons / 50:N0} HS por nave) para movilización espacial.";
                }
            }
        }

        private void CmbTransportCapacity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgFormations?.SelectedItem is GroundFormation)
            {
                DgFormations_SelectionChanged(DgFormations, null!);
            }
        }

        private void TxtSearchFormation_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void BtnRefresh_Click(object System, System.Windows.RoutedEventArgs e)
        {
            RefreshData();
        }
    }
}
