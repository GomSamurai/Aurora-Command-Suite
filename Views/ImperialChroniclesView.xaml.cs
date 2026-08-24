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
        private ImperialChroniclesTelemetry _telemetry = new ImperialChroniclesTelemetry();

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
            _telemetry = _dbService.GetImperialChroniclesTelemetry(_raceId);

            UpdateTelemetryDashboard();
            ApplyFilter();
        }

        private void UpdateTelemetryDashboard()
        {
            if (_telemetry == null) return;

            if (TxtKpiTotalEvents != null) TxtKpiTotalEvents.Text = $"{_telemetry.TotalEvents:N0} Eventos";
            if (TxtKpiResearch != null) TxtKpiResearch.Text = $"{_telemetry.ResearchEvents + _telemetry.IndustryEvents:N0} Hitos";
            if (TxtKpiResearchSub != null) TxtKpiResearchSub.Text = $"{_telemetry.ResearchPercent + _telemetry.IndustryPercent:F1}% del Registro Histórico";

            if (TxtKpiExploration != null) TxtKpiExploration.Text = $"{_telemetry.ExplorationEvents:N0} Prospecciones";
            if (TxtKpiExplorationSub != null) TxtKpiExplorationSub.Text = $"{_telemetry.ExplorationPercent:F1}% del Registro Histórico";

            if (TxtKpiOfficers != null) TxtKpiOfficers.Text = $"{_telemetry.OfficerEvents:N0} Decretos";
            if (TxtKpiOfficersSub != null) TxtKpiOfficersSub.Text = $"{_telemetry.OfficerPercent:F1}% del Registro Histórico";

            // Progress Bars
            if (PbarDistResearch != null) PbarDistResearch.Value = _telemetry.ResearchPercent;
            if (TxtDistResearchVal != null) TxtDistResearchVal.Text = $"{_telemetry.ResearchPercent:F0}%";

            if (PbarDistExploration != null) PbarDistExploration.Value = _telemetry.ExplorationPercent;
            if (TxtDistExplorationVal != null) TxtDistExplorationVal.Text = $"{_telemetry.ExplorationPercent:F0}%";

            if (PbarDistOfficers != null) PbarDistOfficers.Value = _telemetry.OfficerPercent;
            if (TxtDistOfficersVal != null) TxtDistOfficersVal.Text = $"{_telemetry.OfficerPercent:F0}%";

            if (PbarDistIndustry != null) PbarDistIndustry.Value = _telemetry.IndustryPercent;
            if (TxtDistIndustryVal != null) TxtDistIndustryVal.Text = $"{_telemetry.IndustryPercent:F0}%";

            if (PbarDistCombat != null) PbarDistCombat.Value = _telemetry.CombatPercent;
            if (TxtDistCombatVal != null) TxtDistCombatVal.Text = $"{_telemetry.CombatPercent:F0}%";

            if (TxtHeroTech != null) TxtHeroTech.Text = string.IsNullOrEmpty(_telemetry.TopTechName) ? "General" : _telemetry.TopTechName;
            if (TxtHeroOfficer != null) TxtHeroOfficer.Text = string.IsNullOrEmpty(_telemetry.TopHeroName) ? "Alto Mando Imperial" : _telemetry.TopHeroName;
        }

        private void ApplyFilter()
        {
            if (DgEvents == null) return;

            string query = TxtSearchChronicle?.Text?.Trim().ToLower() ?? "";
            string catFilter = (CmbCategoryFilter?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

            var filtered = _allEvents.Where(e =>
            {
                bool matchesQuery = string.IsNullOrEmpty(query) ||
                                    e.CategoryName.ToLower().Contains(query) ||
                                    e.MessageText.ToLower().Contains(query) ||
                                    e.TranslatedMessageText.ToLower().Contains(query) ||
                                    e.FormattedDateDisplay.ToLower().Contains(query);

                bool matchesCategory = true;
                if (!string.IsNullOrEmpty(catFilter) && !catFilter.Contains("Todas"))
                {
                    matchesCategory = e.CategoryIcon.ToLower().Contains(catFilter.ToLower()) ||
                                      e.CategoryName.ToLower().Contains(catFilter.ToLower());
                }

                return matchesQuery && matchesCategory;
            }).ToList();

            DgEvents.ItemsSource = filtered;

            if (TxtEventSummary != null)
            {
                TxtEventSummary.Text = $"📜 {filtered.Count:N0} Eventos Registrados en los Anales Imperiales";
            }
        }

        private void TxtSearchChronicle_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void CmbCategoryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void BtnRefresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RefreshData();
        }

        private void TxtFormattedMessage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is TextBlock tb && tb.DataContext is ImperialChronicleEvent ev)
            {
                ChronicleMessageFormatter.FormatMessageToTextBlock(tb, ev.TranslatedMessageText);
            }
        }
    }
}
