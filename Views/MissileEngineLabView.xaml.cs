using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AuroraDesignSuite.Models;
using AuroraDesignSuite.Services;

namespace AuroraDesignSuite.Views
{
    public partial class MissileEngineLabView : UserControl
    {
        private DatabaseService? _dbService;
        private int _currentRaceId;

        private List<CustomProjectItem> _allProjects = new List<CustomProjectItem>();
        private List<CustomProjectItem> _appUserPresets = new List<CustomProjectItem>();

        public MissileEngineLabView()
        {
            InitializeComponent();
        }

        public void LoadData(DatabaseService dbService, int raceId)
        {
            _dbService = dbService;
            _currentRaceId = raceId;
            RefreshCatalogData();
            CalculateCurrentProjectSpecs();
        }

        public void LoadLabData(DatabaseService dbService, int raceId) => LoadData(dbService, raceId);

        private void OnModeChanged(object sender, RoutedEventArgs e)
        {
            if (PnlProjectDesigner == null || PnlProjectCatalog == null) return;

            if (BtnModeDesigner != null && BtnModeDesigner.IsChecked == true)
            {
                PnlProjectDesigner.Visibility = Visibility.Visible;
                PnlProjectCatalog.Visibility = Visibility.Collapsed;
            }
            else
            {
                PnlProjectDesigner.Visibility = Visibility.Collapsed;
                PnlProjectCatalog.Visibility = Visibility.Visible;
                RefreshCatalogData();
            }
        }

        private void OnCategoryChanged(object sender, RoutedEventArgs e)
        {
            if (TxtProjectName == null) return;

            if (RbCatSensors?.IsChecked == true)
            {
                TxtProjectName.Text = "Active Sensor Buscar AS8-R100";
            }
            else if (RbCatWeapons?.IsChecked == true)
            {
                TxtProjectName.Text = "Láser Focalizado de Frecuencia 15cm";
            }
            else if (RbCatMissiles?.IsChecked == true)
            {
                TxtProjectName.Text = "Misil Antibuque Víbora MK-I";
            }
            else if (RbCatTurrets?.IsChecked == true)
            {
                TxtProjectName.Text = "Torreta Doble Gauss R400-100";
            }
            else if (RbCatEngines?.IsChecked == true)
            {
                TxtProjectName.Text = "Motor de Impulso Ion Militar 500";
            }
            else if (RbCatGround?.IsChecked == true)
            {
                TxtProjectName.Text = "Formación de Tanques de Asalto Pesados";
            }

            CalculateCurrentProjectSpecs();
        }

        private void OnParamChanged(object sender, SelectionChangedEventArgs e) => CalculateCurrentProjectSpecs();
        private void OnParamChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => CalculateCurrentProjectSpecs();

        private void CalculateCurrentProjectSpecs()
        {
            if (LblSpecSize == null || LblSpecCostRP == null || LblSpecCostBP == null || IcProjectMinerals == null) return;

            double hs = SldSensorSize != null ? SldSensorSize.Value : 1.0;
            double res = SldSensorRes != null ? SldSensorRes.Value : 100.0;
            double powerMod = SldPowerMod != null ? SldPowerMod.Value : 1.0;

            if (LblValSensorSize != null) LblValSensorSize.Text = $"{hs:F1} HS ({hs * 50.0:N0} t)";
            if (LblValSensorRes != null) LblValSensorRes.Text = $"Res {res:F0} ({res * 50.0:N0}t)";
            if (LblValPowerMod != null) LblValPowerMod.Text = $"{powerMod:F2}x";

            // Live range calculation: Sensor Range = Strength * Size * SQRT(Res) * 10000 km
            double strength = 2.0 * powerMod;
            double maxRangeKm = strength * hs * Math.Sqrt(res) * 40000.0;
            double costRP = Math.Round(hs * strength * 50.0, 0);
            double costBP = Math.Round(costRP / 50.0, 1);
            int crew = (int)Math.Max(1, hs * 2);
            int htk = (int)Math.Max(1, hs);

            LblSpecSize.Text = $"{hs:F1} HS ({hs * 50.0:N0} t)";
            LblSpecCostRP.Text = $"{costRP:N0} RP";
            LblSpecCostBP.Text = $"{costBP:F1} BP";
            LblSpecCrew.Text = $"{crew} Personas";
            LblSpecHTK.Text = $"{htk} HTK";
            LblSpecPerformance.Text = $"{maxRangeKm / 1_000_000.0:F2} Mkm";

            LblSpecDescription.Text = $"Potencia Sensor: {strength:F1} | Mod. Sensibilidad: 50% | Resolución {res:F0} | Alcance Máximo vs {res * 50.0:N0}t: {maxRangeKm / 1_000_000.0:F2} Mkm | Rango vs 1000t: {maxRangeKm / 5_000_000.0:F2} Mkm";

            // Minerals
            var minerals = new Dictionary<string, double>
            {
                { "Uridium (Circuitos de Control)", Math.Round(costBP * 0.6, 1) },
                { "Corbonite (Estructura y Chasis)", Math.Round(costBP * 0.4, 1) }
            };
            IcProjectMinerals.ItemsSource = minerals;
        }

        private void BtnCreateProjectInDB_Click(object sender, RoutedEventArgs e)
        {
            if (_dbService == null)
            {
                MessageBox.Show("Conexión con AuroraDB.db no disponible.", "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string name = TxtProjectName != null ? TxtProjectName.Text : "Nuevo Proyecto";
            double hs = SldSensorSize != null ? SldSensorSize.Value : 1.0;
            double costRP = Math.Round(hs * 100.0, 0);
            double costBP = Math.Round(costRP / 50.0, 1);

            string category = "📡 Sensores";
            if (RbCatWeapons?.IsChecked == true) category = "💥 Armas Energía";
            else if (RbCatMissiles?.IsChecked == true) category = "🚀 Misiles";
            else if (RbCatTurrets?.IsChecked == true) category = "🛡️ Torretas";
            else if (RbCatEngines?.IsChecked == true) category = "⚡ Motores";
            else if (RbCatGround?.IsChecked == true) category = "⚔️ Terrestre";

            var newProject = new CustomProjectItem
            {
                Name = name,
                Category = category,
                Source = ProjectSource.Aurora4XGame,
                DevelopmentCostRP = costRP,
                BuildCostBP = costBP,
                SizeHS = hs,
                Crew = (int)Math.Max(1, hs * 2),
                HTK = (int)Math.Max(1, hs),
                SpecificationsSummary = LblSpecDescription != null ? LblSpecDescription.Text : "Proyecto personalizado"
            };

            bool success = _dbService.CreateCustomProjectInDatabase(_currentRaceId, newProject, out string msg);
            if (success)
            {
                MessageBox.Show(msg, "Proyecto Creado", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshCatalogData();
            }
            else
            {
                MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSaveAppPreset_Click(object sender, RoutedEventArgs e)
        {
            string name = TxtProjectName != null ? TxtProjectName.Text : "Preset Usuario";
            double hs = SldSensorSize != null ? SldSensorSize.Value : 1.0;
            double costRP = Math.Round(hs * 100.0, 0);
            double costBP = Math.Round(costRP / 50.0, 1);

            string category = "📡 Sensores";
            if (RbCatWeapons?.IsChecked == true) category = "💥 Armas Energía";
            else if (RbCatMissiles?.IsChecked == true) category = "🚀 Misiles";

            var userPreset = new CustomProjectItem
            {
                ProjectID = _appUserPresets.Count + 900000,
                Name = $"⭐ {name}",
                Category = category,
                Source = ProjectSource.AppUserPreset,
                DevelopmentCostRP = costRP,
                BuildCostBP = costBP,
                SizeHS = hs,
                Crew = (int)Math.Max(1, hs * 2),
                HTK = (int)Math.Max(1, hs),
                SpecificationsSummary = "Preset guardado por el usuario en la aplicación Aurora Command Suite."
            };

            _appUserPresets.Add(userPreset);
            MessageBox.Show($"⭐ Preset '{name}' guardado con éxito en la app.", "Preset Guardado", MessageBoxButton.OK, MessageBoxImage.Information);
            RefreshCatalogData();
        }

        private void RefreshCatalogData()
        {
            if (_dbService == null) return;

            _allProjects = _dbService.GetCustomProjects(_currentRaceId);
            _allProjects.AddRange(_appUserPresets);

            ApplyCatalogFilter();
        }

        private void CmbCatalogSourceFilter_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyCatalogFilter();

        private void ApplyCatalogFilter()
        {
            if (DgProjectCatalog == null || LblCatalogTotalCount == null) return;

            int filterIndex = CmbCatalogSourceFilter != null ? CmbCatalogSourceFilter.SelectedIndex : 0;
            IEnumerable<CustomProjectItem> filtered = _allProjects;

            if (filterIndex == 1) // Only Game
            {
                filtered = _allProjects.Where(p => p.Source == ProjectSource.Aurora4XGame);
            }
            else if (filterIndex == 2) // Only App
            {
                filtered = _allProjects.Where(p => p.Source == ProjectSource.AppUserPreset);
            }

            var list = filtered.ToList();
            DgProjectCatalog.ItemsSource = list;
            LblCatalogTotalCount.Text = $"Proyectos Cargados: {list.Count}";
        }
    }
}
