using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using AuroraDesignSuite.Models;
using AuroraDesignSuite.Services;

namespace AuroraDesignSuite.Views
{
    public partial class FuelLogisticsView : UserControl
    {
        private DatabaseService? _dbService;
        private int _raceId;
        private List<ColonyFuelStockpile> _allColonies = new List<ColonyFuelStockpile>();
        private List<ShipFuelStatus> _allShips = new List<ShipFuelStatus>();

        public FuelLogisticsView()
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

            _allColonies = _dbService.GetColonyFuelStockpiles(_raceId);
            _allShips = _dbService.GetShipFuelStatuses(_raceId);
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (DgColonies == null || DgShips == null) return;

            string query = TxtSearchFuel?.Text?.Trim().ToLower() ?? "";

            var filteredColonies = _allColonies.Where(c =>
                string.IsNullOrEmpty(query) ||
                c.PopName.ToLower().Contains(query)
            ).ToList();

            var filteredShips = _allShips.Where(s =>
                string.IsNullOrEmpty(query) ||
                s.ShipName.ToLower().Contains(query) ||
                s.FleetName.ToLower().Contains(query) ||
                s.ClassName.ToLower().Contains(query)
            ).ToList();

            DgColonies.ItemsSource = filteredColonies;
            DgShips.ItemsSource = filteredShips;

            if (TxtFuelSummary != null)
            {
                double totalColonyFuel = _allColonies.Sum(c => c.FuelLiters);
                double totalShipFuel = _allShips.Sum(s => s.CurrentFuelLiters);
                TxtFuelSummary.Text = $"⛽ Total Reservas: {totalColonyFuel + totalShipFuel:N0} L ({totalColonyFuel:N0} L en Colonias | {totalShipFuel:N0} L en Naves)";
            }
        }

        private void TxtSearchFuel_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void BtnRefresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RefreshData();
        }
    }
}
