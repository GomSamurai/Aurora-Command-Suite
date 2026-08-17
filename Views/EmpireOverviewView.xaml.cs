using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AuroraDesignSuite.Models;
using AuroraDesignSuite.Services;

namespace AuroraDesignSuite.Views
{
    public partial class EmpireOverviewView : UserControl
    {
        private DatabaseService? _dbService;
        private int _currentRaceId;

        public EmpireOverviewView()
        {
            InitializeComponent();
        }

        public void LoadEmpireData(DatabaseService? dbService, int raceId)
        {
            if (dbService == null || LblEmpPop == null) return;
            _dbService = dbService;
            _currentRaceId = raceId;

            var colonies = dbService.GetColonies(raceId);
            var fleets = dbService.GetActiveFleets(raceId);
            var infrastructure = dbService.GetEmpireInfrastructure(raceId);
            var fleetSummary = dbService.GetEmpireFleetSummary(raceId);
            var officerSummary = dbService.GetOfficerSummary(raceId);
            var gameTime = dbService.GetGameTimeInfo(raceId);

            if (LblEmpGameDate != null) LblEmpGameDate.Text = gameTime.FormattedCurrentDate;
            if (LblEmpLifetime != null) LblEmpLifetime.Text = $"Fundación: {gameTime.StartYear} ({gameTime.YearsElapsed:F1} a. de vida)";

            double totalPop = colonies.Sum(c => c.PopulationMillions);
            int colonyCount = colonies.Count;
            int fleetCount = fleets.Count;
            int totalShips = fleets.Sum(f => f.ShipCount);
            double totalFuel = fleets.Sum(f => f.TotalFuelLiters) + colonies.Sum(c => c.MineralStockpiles.Sorium * 2500);

            LblEmpPop.Text = $"{totalPop:N2} Million";
            if (LblEmpCapital != null) LblEmpCapital.Text = colonyCount > 0 ? $"★ {colonies.First().PopName}" : "Sol";
            LblEmpFuel.Text = $"{totalFuel:N0} Liters";

            double totalRevenueBP = Math.Round(totalPop * 10.0, 0);
            LblEmpRevenue.Text = $"{totalRevenueBP:N0} BP / Year";

            // Aggregated Minerals
            var globalMinerals = new MineralRequirement();
            foreach (var col in colonies)
            {
                var m = col.MineralStockpiles;
                globalMinerals.Duranium += m.Duranium;
                globalMinerals.Sorium += m.Sorium;
                globalMinerals.Neutronium += m.Neutronium;
                globalMinerals.Corundium += m.Corundium;
                globalMinerals.Uridium += m.Uridium;
                globalMinerals.Gallicite += m.Gallicite;
                globalMinerals.Tritium += m.Tritium;
                globalMinerals.Boronide += m.Boronide;
            }

            var minList = new List<KeyValuePair<string, double>>
            {
                new KeyValuePair<string, double>("Duranium", globalMinerals.Duranium),
                new KeyValuePair<string, double>("Sorium", globalMinerals.Sorium),
                new KeyValuePair<string, double>("Neutronium", globalMinerals.Neutronium),
                new KeyValuePair<string, double>("Corundium", globalMinerals.Corundium),
                new KeyValuePair<string, double>("Uridium", globalMinerals.Uridium),
                new KeyValuePair<string, double>("Gallicite", globalMinerals.Gallicite),
                new KeyValuePair<string, double>("Tritium", globalMinerals.Tritium),
                new KeyValuePair<string, double>("Boronide", globalMinerals.Boronide)
            }.Where(x => x.Value > 0).ToList();

            IcEmpireMinerals.ItemsSource = minList;

            // Financial & Wealth
            double totalFinCentres = infrastructure.Where(i => i.Name.Contains("Financieros")).Sum(i => i.Amount);
            LblGip.Text = $"{totalRevenueBP:N0} BP / Year";
            LblTax.Text = $"{totalRevenueBP * 0.10:N0} BP / Year";
            LblFinCentres.Text = $"{totalFinCentres:N0} Centros Financieros";
            LblNetWealth.Text = $"+{totalRevenueBP * 0.10 + (totalFinCentres * 100):N0} BP (Superávit)";

            // Infrastructure DataGrid
            DgInfrastructure.ItemsSource = infrastructure;
            double totalFactories = infrastructure.Where(i => i.Name.Contains("Construcción") || i.Name.Contains("Convencional")).Sum(i => i.Amount);
            if (LblTotalFactories != null) LblTotalFactories.Text = $"Total Fábricas: {totalFactories:N0}";

            // Fleets DataGrid
            DgFleetsSummary.ItemsSource = fleetSummary;
            if (LblTotalFleetsCount != null) LblTotalFleetsCount.Text = $"Total Flotas: {fleetCount}";

            // Officer Corps Summary
            if (LblCaptainsCount != null) LblCaptainsCount.Text = $"{officerSummary.CaptainsCount} Oficiales";
            if (LblScientistsCount != null) LblScientistsCount.Text = $"{officerSummary.ScientistsCount} Científicos";
            if (LblGovsCount != null) LblGovsCount.Text = $"{officerSummary.GovernorsCount} Gobernadores";
            if (LblTotalOfficersCount != null) LblTotalOfficersCount.Text = $"{officerSummary.TotalOfficers} Oficiales";

            // Strategic Advisor
            string capitalName = colonies.Count > 0 ? colonies.First().PopName : "Sol";
            if (totalPop > 0)
            {
                LblAdvisorText.Text = $"💡 PLANIFICACIÓN ESTRATÉGICA IMPERIAL: Con {totalPop:N2} Millones de ciudadanos en {capitalName}, tu imperio opera {totalFactories:N0} Fábricas (Industria Convencional e Industrial). Se recomienda mantener la producción activa y diversificar la minería de Gallicite y Sorium.";
            }
            else
            {
                LblAdvisorText.Text = "💡 PLANIFICACIÓN ESTRATÉGICA IMPERIAL: Priorizar la investigación de motores de salto y sensores de prospección geológica para descubrir nuevos yacimientos de minerales exóticos.";
            }
        }

        private void BtnTaxDecree_Click(object sender, RoutedEventArgs e)
        {
            if (_dbService == null) return;
            if (_dbService.ExecuteEmpireDecree(_currentRaceId, "Tax", out string msg))
            {
                MessageBox.Show(msg, "Decreto Imperial Promulgado", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadEmpireData(_dbService, _currentRaceId);
            }
        }

        private void BtnProdDecree_Click(object sender, RoutedEventArgs e)
        {
            if (_dbService == null) return;
            if (_dbService.ExecuteEmpireDecree(_currentRaceId, "Production", out string msg))
            {
                MessageBox.Show(msg, "Decreto Imperial Promulgado", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadEmpireData(_dbService, _currentRaceId);
            }
        }

        private void BtnSupplyDecree_Click(object sender, RoutedEventArgs e)
        {
            if (_dbService == null) return;
            if (_dbService.ExecuteEmpireDecree(_currentRaceId, "Supply", out string msg))
            {
                MessageBox.Show(msg, "Auditoría de Suministros Completada", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadEmpireData(_dbService, _currentRaceId);
            }
        }
    }
}
