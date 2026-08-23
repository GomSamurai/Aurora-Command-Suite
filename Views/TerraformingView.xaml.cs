using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using AuroraDesignSuite.Models;
using AuroraDesignSuite.Services;

namespace AuroraDesignSuite.Views
{
    public partial class TerraformingView : UserControl
    {
        private DatabaseService? _dbService;
        private int _raceId;
        private List<TerraformWorldInfo> _allWorlds = new List<TerraformWorldInfo>();

        public TerraformingView()
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

            _allWorlds = _dbService.GetTerraformWorlds(_raceId);
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (DgWorlds == null) return;

            string query = TxtSearchWorld?.Text?.Trim().ToLower() ?? "";
            var filtered = _allWorlds.Where(w =>
                string.IsNullOrEmpty(query) ||
                w.PopName.ToLower().Contains(query) ||
                w.ColonyCostDisplay.ToLower().Contains(query)
            ).ToList();

            DgWorlds.ItemsSource = filtered;

            if (TxtTerraformSummary != null)
            {
                int terraformable = filtered.Count(w => w.ColonyCost > 0);
                TxtTerraformSummary.Text = $"{filtered.Count} Colonias Imperiales | {terraformable} Requieren Terraformación";
            }

            if (filtered.Count > 0 && DgWorlds.SelectedItem == null)
            {
                DgWorlds.SelectedIndex = 0;
            }
        }

        private void DgWorlds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgWorlds.SelectedItem is not TerraformWorldInfo selected) return;

            if (TxtWorldHeader != null) TxtWorldHeader.Text = $"🌍 {selected.PopName} - INFORME TÉCNICO KLIMA-ATMO";
            if (LblColonyCost != null) LblColonyCost.Text = selected.ColonyCostDisplay;
            if (LblGravity != null) LblGravity.Text = $"{selected.Gravity:N2} G";
            if (LblHydro != null) LblHydro.Text = selected.HydroDisplay;

            DgGases.ItemsSource = selected.Gases;

            RecalculateSimulation();
        }

        private void SliderInstallations_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (TxtInstallationsCount != null)
            {
                TxtInstallationsCount.Text = $"{(int)SliderInstallations.Value} Instalaciones";
            }
            RecalculateSimulation();
        }

        private void DgGases_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RecalculateSimulation();
        }

        private void RecalculateSimulation()
        {
            if (DgWorlds?.SelectedItem is not TerraformWorldInfo selected) return;

            int installations = SliderInstallations != null ? (int)SliderInstallations.Value : 20;
            // Base terraforming rate per installation in Aurora: ~0.001 atm/yr base
            double annualRate = installations * 0.0010;

            if (TxtSimRate != null)
            {
                TxtSimRate.Text = $"⚡ Tasa de Inyección: {annualRate:N4} atm / año ({installations} Instalaciones en Operación)";
            }

            if (TxtSimTime != null)
            {
                if (installations <= 0)
                {
                    TxtSimTime.Text = "⚠️ Sin instalaciones activas. Asigne complejos de terraformación a la colonia para iniciar el proceso.";
                }
                else if (DgGases?.SelectedItem is AtmosphericGasInfo selectedGas)
                {
                    double gasAtm = selectedGas.GasAtm;
                    double gasYears = annualRate > 0 ? (gasAtm / annualRate) : 999.0;
                    string actionStr = selectedGas.IsDangerous ? "Eliminar / Neutralizar" : "Ajustar / Inyectar";
                    TxtSimTime.Text = $"🎯 OBJETIVO SELECCIONADO: {actionStr} '{selectedGas.GasName}' ({gasAtm:N3} atm) ➔ ~{gasYears:N1} Años Imperiales ({gasYears * 12:N0} Meses)";
                }
                else if (selected.ColonyCost <= 0.001)
                {
                    TxtSimTime.Text = "🟢 ¡Planeta Idílico! No requiere modificaciones atmosféricas adicionales.";
                }
                else
                {
                    double targetAtmChange = Math.Max(0.10, selected.ColonyCost * 0.25);
                    double estimatedYears = annualRate > 0 ? (targetAtmChange / annualRate) : 999.0;
                    TxtSimTime.Text = $"⏳ Tiempo Estimado para Atmósfera Respirable (Reducción Global de CC): ~{estimatedYears:N1} Años Imperiales ({estimatedYears * 12:N0} Meses)";
                }
            }
        }

        private void TxtSearchWorld_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void BtnRefresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RefreshData();
        }
    }
}
