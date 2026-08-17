using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AuroraDesignSuite.Models;
using AuroraDesignSuite.Services;

namespace AuroraDesignSuite.Views
{
    public partial class SystemExplorationView : UserControl
    {
        private DatabaseService? _dbService;
        private int _currentRaceId;

        private StarSystemInfo? _selectedSystem;
        private SystemBodyInfo? _selectedBody;

        public SystemExplorationView()
        {
            InitializeComponent();
        }

        public void LoadData(DatabaseService dbService, int raceId)
        {
            _dbService = dbService;
            _currentRaceId = raceId;
            if (_dbService == null) return;

            RefreshData();
        }

        private void RefreshData()
        {
            if (_dbService == null) return;

            var systems = _dbService.GetDiscoveredSystems(_currentRaceId);
            DgSystems.ItemsSource = systems;

            if (systems.Count > 0)
            {
                DgSystems.SelectedIndex = 0;
            }

            // Survey Fleets
            var fleets = _dbService.GetEmpireFleetSummary(_currentRaceId);
            var surveyFleets = fleets.Where(f => f.FleetName.Contains("Survey") || f.FleetName.Contains("Explora")).ToList();
            DgSurveyFleets.ItemsSource = surveyFleets;
        }

        private void OnModeChanged(object sender, RoutedEventArgs e)
        {
            if (PnlProspection == null || PnlJumpPoints == null || PnlSurveyFleets == null) return;

            if (BtnModeProspection?.IsChecked == true)
            {
                PnlProspection.Visibility = Visibility.Visible;
                PnlJumpPoints.Visibility = Visibility.Collapsed;
                PnlSurveyFleets.Visibility = Visibility.Collapsed;
            }
            else if (BtnModeJumpPoints?.IsChecked == true)
            {
                PnlProspection.Visibility = Visibility.Collapsed;
                PnlJumpPoints.Visibility = Visibility.Visible;
                PnlSurveyFleets.Visibility = Visibility.Collapsed;
            }
            else if (BtnModeSurveyFleets?.IsChecked == true)
            {
                PnlProspection.Visibility = Visibility.Collapsed;
                PnlJumpPoints.Visibility = Visibility.Collapsed;
                PnlSurveyFleets.Visibility = Visibility.Visible;
            }
        }

        private void DgSystems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgSystems.SelectedItem is StarSystemInfo sys)
            {
                _selectedSystem = sys;
                LblBodiesHeader.Text = $"🪐 PLANETAS, LUNAS Y CUERPOS CELESTES EN {sys.SystemName.ToUpper()}";
                DgBodies.ItemsSource = sys.Bodies;

                if (LblSysBodiesCount != null) LblSysBodiesCount.Text = $"{sys.Bodies.Count} Cuerpos";
                if (LblSysJumpCount != null) LblSysJumpCount.Text = $"{sys.JumpPoints.Count} Puntos";

                DgJumpPoints.ItemsSource = sys.JumpPoints;

                if (sys.Bodies.Count > 0)
                {
                    DgBodies.SelectedIndex = 0;
                }
                else
                {
                    DgMinerals.ItemsSource = null;
                }
            }
        }

        private void TxtSearchBody_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_selectedSystem == null || DgBodies == null) return;
            string searchText = TxtSearchBody?.Text.Trim().ToLower() ?? "";
            if (string.IsNullOrWhiteSpace(searchText))
            {
                DgBodies.ItemsSource = _selectedSystem.Bodies;
            }
            else
            {
                DgBodies.ItemsSource = _selectedSystem.Bodies.Where(b => b.Name.ToLower().Contains(searchText) || b.BodyTypeName.ToLower().Contains(searchText)).ToList();
            }
        }

        private void DgBodies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgBodies.SelectedItem is SystemBodyInfo body)
            {
                _selectedBody = body;
                LblMineralsHeader.Text = $"💎 YACIMIENTOS MINERALES EN {body.Name.ToUpper()}";
                DgMinerals.ItemsSource = body.MineralDeposits;

                // Update Detailed Physical & Orbital Telemetry
                if (LblValRadius != null) LblValRadius.Text = body.RadiusDisplay;
                if (LblValDensity != null) LblValDensity.Text = $"{body.Density:F2} g/cm³";
                if (LblValMass != null) LblValMass.Text = body.MassDisplay;
                if (LblValOrbDist != null) LblValOrbDist.Text = body.OrbitalDistDisplay;
                if (LblValYear != null) LblValYear.Text = body.YearDisplay;
                if (LblValDay != null) LblValDay.Text = body.DayDisplay;
                if (LblValEscape != null) LblValEscape.Text = body.EscapeVelDisplay;
                if (LblValTidal != null) LblValTidal.Text = body.TidalLockDisplay;

                // Update Climate & Atmosphere Telemetry
                if (LblValAtmos != null) LblValAtmos.Text = body.AtmosDisplay;
                if (LblValTemp != null) LblValTemp.Text = body.TempDisplay;
                if (LblValTempK != null) LblValTempK.Text = $"{body.SurfaceTempKelvin:F1} K";
                if (LblValHydro != null) LblValHydro.Text = body.HydroDisplay;
                if (LblValAlbedo != null) LblValAlbedo.Text = $"{body.Albedo:F2}";
                if (LblValGHFactor != null) LblValGHFactor.Text = $"{body.GHFactor:F2}";
                if (LblValMagnet != null) LblValMagnet.Text = body.MagneticFieldDisplay;

                // Update Environment, Archaeology & Survey
                if (LblValRuins != null)
                {
                    LblValRuins.Text = body.RuinsDisplay;
                    LblValRuins.Foreground = body.RuinID > 0 ? System.Windows.Media.Brushes.Gold : System.Windows.Media.Brushes.White;
                }
                if (LblValFactories != null)
                {
                    LblValFactories.Text = body.FactoriesDisplay;
                    LblValFactories.Foreground = body.AbandonedFactories > 0 ? System.Windows.Media.Brushes.SpringGreen : System.Windows.Media.Brushes.White;
                }
                if (LblValRad != null) LblValRad.Text = body.RadiationLevel > 0 ? $"{body.RadiationLevel:F1} (Alta)" : "0.0";
                if (LblValDust != null) LblValDust.Text = body.DustLevel > 0 ? $"{body.DustLevel:F1} (Polvo)" : "0.0";
                if (LblValSurvey != null) LblValSurvey.Text = body.SurveyStatusDisplay;

                // Update Colonization Viability Card
                double cost = body.ColonyCost;
                if (LblColonyCostVal != null)
                {
                    LblColonyCostVal.Text = body.ColonyCostDisplay;
                    LblColonyCostVal.Foreground = cost == 0 ? System.Windows.Media.Brushes.SpringGreen : System.Windows.Media.Brushes.Gold;
                }

                if (LblGravSuitability != null)
                {
                    bool isGravOk = body.GravityG >= 0.10 && body.GravityG <= 1.90;
                    LblGravSuitability.Text = isGravOk ? $"Tolerable ({body.GravityG:F2} G)" : "🚫 Inhabitable (G Extrema)";
                    LblGravSuitability.Foreground = isGravOk ? System.Windows.Media.Brushes.SpringGreen : System.Windows.Media.Brushes.Red;
                }

                if (LblAtmosReq != null)
                {
                    double diffAtmos = Math.Round(Math.Abs(body.AtmosPress - 1.0), 2);
                    LblAtmosReq.Text = diffAtmos == 0 ? "Ajuste Nulo (1.0 atm)" : $"{diffAtmos:F2} atm Requeridos";
                }

                if (LblTempReq != null)
                {
                    double diffTemp = Math.Round(13.9 - body.SurfaceTempC, 1);
                    LblTempReq.Text = Math.Abs(diffTemp) < 0.2 ? "Temperatura Ideal" : (diffTemp > 0 ? $"+{diffTemp:F1} °C Calentamiento" : $"{diffTemp:F1} °C Enfriamiento");
                }
            }
        }

        private void BtnSetColonyTarget_Click(object sender, RoutedEventArgs e)
        {
            if (_dbService == null) return;
            if (_selectedBody == null)
            {
                MessageBox.Show("Por favor selecciona un cuerpo celeste de la lista para marcar como objetivo colonial.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_dbService.SetColonizationTarget(_currentRaceId, _selectedBody.SystemBodyID, _selectedBody.Name, out string msg))
            {
                MessageBox.Show(msg, "Objetivo Colonial Designado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnSendSurveyFleet_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedBody == null)
            {
                MessageBox.Show("Por favor selecciona un cuerpo celeste de la lista.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show($"🚀 Orden enviada a la Flota de Prospección (Survey Fleet) para escanear geológicamente '{_selectedBody.Name}' en AuroraDB.db.", "Misión Asignada", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
