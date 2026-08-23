using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using AuroraDesignSuite.Models;
using AuroraDesignSuite.Services;

namespace AuroraDesignSuite.Views
{
    public partial class OfficerMemorialView : UserControl
    {
        private DatabaseService? _dbService;
        private int _raceId;
        private List<MemorialOfficerInfo> _allOfficers = new List<MemorialOfficerInfo>();

        public OfficerMemorialView()
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

            _allOfficers = _dbService.GetMemorialOfficers(_raceId);
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            string query = TxtSearchOfficer?.Text?.Trim().ToLower() ?? "";
            var filtered = _allOfficers.Where(o =>
                string.IsNullOrEmpty(query) ||
                o.Name.ToLower().Contains(query) ||
                o.RankName.ToLower().Contains(query) ||
                o.StatusDisplay.ToLower().Contains(query)
            ).ToList();

            DgOfficers.ItemsSource = filtered;

            int totalMedals = filtered.Sum(o => o.TotalMedalsCount);
            if (TxtMemorialSummary != null)
            {
                TxtMemorialSummary.Text = $"{filtered.Count} Oficiales Registrados | {totalMedals} Medallas Concedidas";
            }

            if (filtered.Count > 0 && DgOfficers.SelectedItem == null)
            {
                DgOfficers.SelectedIndex = 0;
            }
        }

        private void DgOfficers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgOfficers.SelectedItem is not MemorialOfficerInfo selected) return;

            if (TxtOfficerNameHeader != null) TxtOfficerNameHeader.Text = $"🎖️ {selected.RankName} {selected.Name}";
            if (LblOfficerStatus != null) LblOfficerStatus.Text = selected.StatusDisplay;
            if (LblOfficerType != null) LblOfficerType.Text = selected.CommanderTypeDisplay;
            if (LblOfficerKills != null) LblOfficerKills.Text = selected.KillsSummaryDisplay;

            DgMedals.ItemsSource = selected.Medals;

            if (TxtCitationQuote != null)
            {
                if (selected.TotalMedalsCount > 0)
                {
                    TxtCitationQuote.Text = $"📜 'Distinguido oficial galardonado con {selected.TotalMedalsCount} condecoraciones imperiales. Condecoración destacada: {selected.MedalsSummary}.'";
                }
                else
                {
                    TxtCitationQuote.Text = $"📜 'Oficial de la Marina e Infraestructura Imperial. Expediente limpio en los archivos de servicio de la Corona.'";
                }
            }
        }

        private void TxtSearchOfficer_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void BtnRefresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RefreshData();
        }
    }
}
