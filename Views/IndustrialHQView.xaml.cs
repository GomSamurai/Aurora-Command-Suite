using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AuroraDesignSuite.Models;
using AuroraDesignSuite.Services;

namespace AuroraDesignSuite.Views
{
    public partial class IndustrialHQView : UserControl
    {
        private DatabaseService? _dbService;
        private int _currentRaceId;
        private List<PopulationInstallationInfo> _insts = new List<PopulationInstallationInfo>();
        private List<IndustrialProjectInfo> _projects = new List<IndustrialProjectInfo>();

        public IndustrialHQView()
        {
            InitializeComponent();
        }

        public void LoadData(DatabaseService dbService, int raceId)
        {
            _dbService = dbService;
            _currentRaceId = raceId;
            if (_dbService == null) return;

            _insts = _dbService.GetPopulationInstallations(_currentRaceId);
            DgInstallations.ItemsSource = _insts;

            _projects = _dbService.GetIndustrialProjects(_currentRaceId);
            DgIndustrialProjects.ItemsSource = _projects;

            RecalculateIndustrialTelemetry();
            RecalculateCalculator();
        }

        private void BtnNewIndustrialOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_dbService == null) return;

            var dlg = new NewIndustrialOrderDialog(_dbService, _currentRaceId)
            {
                Owner = Window.GetWindow(this)
            };

            if (dlg.ShowDialog() == true)
            {
                LoadData(_dbService, _currentRaceId);
            }
        }

        private void BtnCancelProject_Click(object sender, RoutedEventArgs e)
        {
            if (_dbService == null) return;

            if (DgIndustrialProjects.SelectedItem is IndustrialProjectInfo proj)
            {
                var confirm = MessageBox.Show($"¿Estás seguro de que deseas cancelar el proyecto '{proj.Description}'?", "Confirmar Cancelación", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirm == MessageBoxResult.Yes)
                {
                    if (_dbService.DeleteIndustrialProject(proj.ProjectID, out string msg))
                    {
                        MessageBox.Show(msg, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData(_dbService, _currentRaceId);
                    }
                    else
                    {
                        MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor selecciona un proyecto industrial de la lista para cancelar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OnCalcInputChanged(object sender, TextChangedEventArgs e)
        {
            RecalculateCalculator();
        }

        private void RecalculateCalculator()
        {
            if (LblCalcAnnualBP == null || LblCalcDays == null || TxtTargetBP == null || TxtAssignedFactories == null) return;

            double.TryParse(TxtTargetBP.Text, out double targetBP);
            double.TryParse(TxtAssignedFactories.Text, out double factories);

            if (targetBP <= 0) targetBP = 120.0;
            if (factories <= 0) factories = 1.0;

            double annualBP = factories * 10.0; // 10 BP/year per factory standard
            double days = Math.Round((targetBP / annualBP) * 365.0, 0);
            double months = Math.Round(days / 30.4, 1);

            LblCalcAnnualBP.Text = $"{annualBP:N0} BP/Año";
            LblCalcDays.Text = $"{days:N0} Días ({months:F1} Meses)";
        }

        private void RecalculateIndustrialTelemetry()
        {
            if (LblTotalBPOutput == null || LblConstructionFactories == null || LblOrdnanceFactories == null || 
                LblFuelRefineries == null || LblMiningCapacity == null || IcIndustrialMinerals == null) return;

            double constructionFactories = 0;
            double ordnanceFactories = 0;
            double fuelRefineries = 0;
            double automatedMines = 0;
            double conventionalMines = 0;

            foreach (var inst in _insts)
            {
                if (inst.InstallationName.Contains("Construcción", StringComparison.OrdinalIgnoreCase))
                {
                    constructionFactories += inst.Amount;
                }
                else if (inst.InstallationName.Contains("Misiles", StringComparison.OrdinalIgnoreCase) || inst.InstallationName.Contains("Cazas", StringComparison.OrdinalIgnoreCase))
                {
                    ordnanceFactories += inst.Amount;
                }
                else if (inst.InstallationName.Contains("Refinería", StringComparison.OrdinalIgnoreCase))
                {
                    fuelRefineries += inst.Amount;
                }
                else if (inst.InstallationName.Contains("Automatizada", StringComparison.OrdinalIgnoreCase))
                {
                    automatedMines += inst.Amount;
                }
                else if (inst.InstallationName.Contains("Mina", StringComparison.OrdinalIgnoreCase))
                {
                    conventionalMines += inst.Amount;
                }
            }

            if (_dbService != null && LblTotalPopulation != null)
            {
                double totalPopM = _dbService.GetTotalEmpirePopulation(_currentRaceId);
                LblTotalPopulation.Text = $"{totalPopM:N1} Millones";

                double recCF = Math.Round(totalPopM / 5.0, 0);
                double recRef = Math.Round(totalPopM / 60.0, 0);

                if (LblAdviceConversion != null)
                {
                    LblAdviceConversion.Text = $"Tu imperio cuenta con {totalPopM:N0}M hab. El objetivo recomendado es de {recCF:N0} Fábricas de Construcción. Se aconseja convertir la Industria Convencional a Fábricas de Construcción para acelerar la expansión.";
                }

                if (LblAdviceRefinery != null)
                {
                    LblAdviceRefinery.Text = $"Se recomiendan {recRef:N0} Refinerías de Sorium para tu volumen de población ({totalPopM:N0}M) y sostenimiento de flota.";
                }
            }

            double annualBPOutput = (constructionFactories * 10.0) + (ordnanceFactories * 12.0);
            double totalMines = automatedMines + conventionalMines;

            LblTotalBPOutput.Text = $"{annualBPOutput:N0} BP / Año";
            LblConstructionFactories.Text = $"{constructionFactories:N0} Fábricas";
            LblOrdnanceFactories.Text = $"{ordnanceFactories:N0} Fábricas";
            LblFuelRefineries.Text = $"{fuelRefineries:N0} Refinerías";
            LblMiningCapacity.Text = $"{totalMines:N0} Minas ({automatedMines:N0} Auto / {conventionalMines:N0} Conv)";

            // Estimate construction mineral usage
            double totalProjectBP = _projects.Sum(p => p.Amount * 120.0);
            var minerals = new List<KeyValuePair<string, double>>
            {
                new KeyValuePair<string, double>("Duranium", totalProjectBP * 0.45),
                new KeyValuePair<string, double>("Neutronium", totalProjectBP * 0.20),
                new KeyValuePair<string, double>("Corundium", totalProjectBP * 0.15),
                new KeyValuePair<string, double>("Gallicite", totalProjectBP * 0.12),
                new KeyValuePair<string, double>("Uridium", totalProjectBP * 0.08)
            }.Where(x => x.Value > 0).ToList();

            IcIndustrialMinerals.ItemsSource = minerals;
        }
    }
}
