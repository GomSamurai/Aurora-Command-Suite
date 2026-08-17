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

            // Imperial Identity & Emblems
            var empireDetails = dbService.GetFullEmpireDetails(raceId);
            if (TxtEmpireName != null) TxtEmpireName.Text = empireDetails.RaceName;
            if (TxtEmpireTitle != null) TxtEmpireTitle.Text = empireDetails.RaceTitle;
            if (TxtSpeciesName != null) TxtSpeciesName.Text = empireDetails.SpeciesName;

            // Load Flag & Portrait Combo Options
            var flags = dbService.GetAvailableFlags();
            if (CmbFlagPic != null)
            {
                CmbFlagPic.ItemsSource = flags;
                if (flags.Contains(empireDetails.FlagPic)) CmbFlagPic.SelectedItem = empireDetails.FlagPic;
            }

            var portraits = dbService.GetAvailablePortraits();
            if (CmbRacePic != null)
            {
                CmbRacePic.ItemsSource = portraits;
                if (portraits.Contains(empireDetails.RacePic)) CmbRacePic.SelectedItem = empireDetails.RacePic;
            }

            UpdatePreviewImages(empireDetails.FlagPath, empireDetails.PortraitPath);

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

        private void UpdatePreviewImages(string flagPath, string portraitPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(flagPath) && System.IO.File.Exists(flagPath) && ImgFlag != null)
                {
                    var bmp = new System.Windows.Media.Imaging.BitmapImage();
                    bmp.BeginInit();
                    bmp.UriSource = new Uri(flagPath, UriKind.Absolute);
                    bmp.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    bmp.EndInit();
                    ImgFlag.Source = bmp;
                }

                if (!string.IsNullOrEmpty(portraitPath) && System.IO.File.Exists(portraitPath) && ImgPortrait != null)
                {
                    var bmp = new System.Windows.Media.Imaging.BitmapImage();
                    bmp.BeginInit();
                    bmp.UriSource = new Uri(portraitPath, UriKind.Absolute);
                    bmp.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    bmp.EndInit();
                    ImgPortrait.Source = bmp;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading preview images: {ex.Message}");
            }
        }

        private void CmbFlagPic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dbService == null || CmbFlagPic?.SelectedItem is not string flagFile) return;
            string dbDir = System.IO.Path.GetDirectoryName(_dbService.DbPath) ?? @"C:\VSCODE\Aurora271Full";
            string flagPath = System.IO.Path.Combine(dbDir, "Flags", flagFile);
            UpdatePreviewImages(flagPath, null!);
        }

        private void CmbRacePic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dbService == null || CmbRacePic?.SelectedItem is not string raceFile) return;
            string dbDir = System.IO.Path.GetDirectoryName(_dbService.DbPath) ?? @"C:\VSCODE\Aurora271Full";
            string racePath = System.IO.Path.Combine(dbDir, "Races", raceFile);
            UpdatePreviewImages(null!, racePath);
        }

        private void BtnSaveEmpireDetails_Click(object sender, RoutedEventArgs e)
        {
            if (_dbService == null) return;
            var emp = _dbService.GetFullEmpireDetails(_currentRaceId);
            if (TxtEmpireName != null) emp.RaceName = TxtEmpireName.Text;
            if (TxtEmpireTitle != null) emp.RaceTitle = TxtEmpireTitle.Text;
            if (TxtSpeciesName != null) emp.SpeciesName = TxtSpeciesName.Text;
            if (CmbFlagPic?.SelectedItem is string flagFile) emp.FlagPic = flagFile;
            if (CmbRacePic?.SelectedItem is string raceFile) emp.RacePic = raceFile;

            if (_dbService.UpdateEmpireDetails(emp))
            {
                MessageBox.Show("✅ ¡Identidad, Bandera y Retrato Imperial guardados con éxito en AuroraDB.db!\n\nTodos los cambios están reflejados en directo. Abre o pulsa 'Refrescar Suite' en Aurora 4X para ver los nuevos emblemas en tu juego.", "Identidad Imperial Sincronizada", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadEmpireData(_dbService, _currentRaceId);
            }
            else
            {
                MessageBox.Show("❌ No se pudieron guardar los detalles del Imperio en AuroraDB.db.", "Error de Guardado", MessageBoxButton.OK, MessageBoxImage.Error);
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
