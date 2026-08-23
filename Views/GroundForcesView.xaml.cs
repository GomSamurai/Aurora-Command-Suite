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

                double shipsNeeded = Math.Ceiling(selected.TotalSizeTons / 5000.0);
                if (LblCalcShipsNeeded != null) LblCalcShipsNeeded.Text = $"{shipsNeeded:N0} Naves (Cap. 5.000t c/u)";

                if (TxtTacticalNote != null)
                {
                    TxtTacticalNote.Text = $"💡 La formación '{selected.Name}' desplegada en '{selected.LocationName}' consta de {selected.TotalUnits:N0} unidades de combate con un peso total de {selected.TotalSizeTons:N0} toneladas. Requiere un hangar de transporte de {selected.RequiredTroopTransportHS:N0} HS para movilización espacial.";
                }
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
