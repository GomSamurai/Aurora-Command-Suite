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

        private void DgBodies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgBodies.SelectedItem is SystemBodyInfo body)
            {
                _selectedBody = body;
                LblMineralsHeader.Text = $"💎 YACIMIENTOS MINERALES EN {body.Name.ToUpper()}";
                DgMinerals.ItemsSource = body.MineralDeposits;

                // Update Colonization Viability Card
                double cost = body.ColonyCost;
                if (LblColonyCostVal != null)
                {
                    LblColonyCostVal.Text = body.ColonyCostDisplay;
                    LblColonyCostVal.Foreground = cost == 0 ? System.Windows.Media.Brushes.SpringGreen : System.Windows.Media.Brushes.Gold;
                }

                if (LblGravSuitability != null)
                {
                    bool isGravOk = body.GravityG >= 0.1 && body.GravityG <= 3.0;
                    LblGravSuitability.Text = isGravOk ? $"Tolerable ({body.GravityG:F2} G)" : "🚫 Inhabitable (Fuerza G Extrema)";
                    LblGravSuitability.Foreground = isGravOk ? System.Windows.Media.Brushes.SpringGreen : System.Windows.Media.Brushes.Red;
                }

                if (LblAtmosReq != null)
                {
                    double diffAtmos = Math.Round(Math.Abs(body.AtmosPress - 1.0), 2);
                    LblAtmosReq.Text = diffAtmos == 0 ? "Ajuste Nulo (1.0 atm)" : $"{diffAtmos:F2} atm Requeridos";
                }

                if (LblTempReq != null)
                {
                    double diffTemp = Math.Round(15.0 - body.BaseTempC, 1);
                    LblTempReq.Text = diffTemp == 0 ? "Temperatura Estable" : $"{diffTemp:F1} °C de Calentamiento";
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
