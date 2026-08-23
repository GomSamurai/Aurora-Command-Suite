using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using AuroraDesignSuite.Models;
using AuroraDesignSuite.Services;

namespace AuroraDesignSuite.Views
{
    public partial class ImperialChroniclesView : UserControl
    {
        private DatabaseService? _dbService;
        private int _raceId;
        private List<ImperialChronicleEvent> _allEvents = new List<ImperialChronicleEvent>();

        public ImperialChroniclesView()
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

            _allEvents = _dbService.GetImperialChronicleEvents(_raceId);
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (DgEvents == null) return;

            string query = TxtSearchChronicle?.Text?.Trim().ToLower() ?? "";

            var filtered = _allEvents.Where(e =>
                string.IsNullOrEmpty(query) ||
                e.CategoryName.ToLower().Contains(query) ||
                e.MessageText.ToLower().Contains(query) ||
                e.FormattedDateDisplay.ToLower().Contains(query)
            ).ToList();

            DgEvents.ItemsSource = filtered;

            if (TxtEventSummary != null)
            {
                TxtEventSummary.Text = $"📜 {filtered.Count} Eventos Registrados en los Anales Imperiales";
            }
        }

        private void TxtSearchChronicle_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void BtnRefresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RefreshData();
        }
    }
}
