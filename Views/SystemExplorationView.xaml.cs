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

            // Clear previous empire state cleanly
            _selectedSystem = null;
            _selectedBody = null;
            if (DgBodies != null) DgBodies.ItemsSource = null;
            if (DgJumpPoints != null) DgJumpPoints.ItemsSource = null;
            if (DgMinerals != null) DgMinerals.ItemsSource = null;

            var systems = _dbService.GetDiscoveredSystems(_currentRaceId);
            DgSystems.ItemsSource = systems;

            if (systems.Count > 0)
            {
                DgSystems.SelectedIndex = 0;
            }
            else
            {
                if (LblBodiesHeader != null) LblBodiesHeader.Text = "🪐 SITEMAS Y CUERPOS CELESTES (0 Descubiertos)";
                if (LblSysBodiesCount != null) LblSysBodiesCount.Text = "0 Cuerpos";
                if (LblSysJumpCount != null) LblSysJumpCount.Text = "0 Puntos";
            }

            // Survey Fleets
            var fleets = _dbService.GetEmpireFleetSummary(_currentRaceId);
            var surveyFleets = fleets.Where(f => f.FleetName.Contains("Survey") || f.FleetName.Contains("Explora")).ToList();
            DgSurveyFleets.ItemsSource = surveyFleets;
        }

        private void OnModeChanged(object sender, RoutedEventArgs e)
        {
            if (PnlProspection == null || PnlJumpPoints == null || PnlSurveyFleets == null || Pnl2DStarMap == null) return;

            if (BtnModeProspection?.IsChecked == true)
            {
                PnlProspection.Visibility = Visibility.Visible;
                Pnl2DStarMap.Visibility = Visibility.Collapsed;
                PnlJumpPoints.Visibility = Visibility.Collapsed;
                PnlSurveyFleets.Visibility = Visibility.Collapsed;
            }
            else if (BtnMode2DStarMap?.IsChecked == true)
            {
                PnlProspection.Visibility = Visibility.Collapsed;
                Pnl2DStarMap.Visibility = Visibility.Visible;
                PnlJumpPoints.Visibility = Visibility.Collapsed;
                PnlSurveyFleets.Visibility = Visibility.Collapsed;
                Render2DStarMap();
            }
            else if (BtnModeJumpPoints?.IsChecked == true)
            {
                PnlProspection.Visibility = Visibility.Collapsed;
                Pnl2DStarMap.Visibility = Visibility.Collapsed;
                PnlJumpPoints.Visibility = Visibility.Visible;
                PnlSurveyFleets.Visibility = Visibility.Collapsed;
            }
            else if (BtnModeSurveyFleets?.IsChecked == true)
            {
                PnlProspection.Visibility = Visibility.Collapsed;
                Pnl2DStarMap.Visibility = Visibility.Collapsed;
                PnlJumpPoints.Visibility = Visibility.Collapsed;
                PnlSurveyFleets.Visibility = Visibility.Visible;
            }
        }

        private void BtnRefreshStarMap_Click(object sender, RoutedEventArgs e)
        {
            Render2DStarMap();
        }

        private void BtnResetStarMapZoom_Click(object sender, RoutedEventArgs e)
        {
            Render2DStarMap();
        }

        private void Render2DStarMap()
        {
            if (StarMapCanvas == null || _dbService == null) return;
            StarMapCanvas.Children.Clear();

            var systems = _dbService.GetDiscoveredSystems(_currentRaceId);
            if (systems.Count == 0) return;

            int count = systems.Count;
            double centerX = 700;
            double centerY = 450;
            double radius = Math.Min(320, 100 + count * 35);

            Dictionary<int, Point> systemCoords = new Dictionary<int, Point>();

            for (int i = 0; i < count; i++)
            {
                var sys = systems[i];
                double angle = (2.0 * Math.PI * i) / count;
                if (i == 0)
                {
                    // Sol / Home System at center
                    systemCoords[sys.SystemID] = new Point(centerX, centerY);
                }
                else
                {
                    double x = centerX + radius * Math.Cos(angle);
                    double y = centerY + radius * Math.Sin(angle);
                    systemCoords[sys.SystemID] = new Point(x, y);
                }
            }

            // 1. Draw Jump Point Connection Lines
            HashSet<string> drawnLines = new HashSet<string>();
            foreach (var sys in systems)
            {
                if (!systemCoords.ContainsKey(sys.SystemID)) continue;
                Point p1 = systemCoords[sys.SystemID];

                foreach (var jp in sys.JumpPoints)
                {
                    if (jp.DestinationSystemID > 0 && systemCoords.ContainsKey(jp.DestinationSystemID))
                    {
                        string lineKey = Math.Min(sys.SystemID, jp.DestinationSystemID) + "-" + Math.Max(sys.SystemID, jp.DestinationSystemID);
                        if (!drawnLines.Contains(lineKey))
                        {
                            drawnLines.Add(lineKey);
                            Point p2 = systemCoords[jp.DestinationSystemID];

                            var line = new System.Windows.Shapes.Line
                            {
                                X1 = p1.X,
                                Y1 = p1.Y,
                                X2 = p2.X,
                                Y2 = p2.Y,
                                Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(180, 0, 240, 255)),
                                StrokeThickness = 2,
                                StrokeDashArray = jp.GateID > 0 ? null : new System.Windows.Media.DoubleCollection { 4, 4 }
                            };
                            StarMapCanvas.Children.Add(line);
                        }
                    }
                }
            }

            // 2. Draw System Nodes & Labels
            foreach (var sys in systems)
            {
                if (!systemCoords.ContainsKey(sys.SystemID)) continue;
                Point p = systemCoords[sys.SystemID];

                bool isHome = sys.SystemID == 1 || sys.SystemName.Equals("Sol", StringComparison.OrdinalIgnoreCase);

                double nodeSize = isHome ? 36 : 26;
                var ellipse = new System.Windows.Shapes.Ellipse
                {
                    Width = nodeSize,
                    Height = nodeSize,
                    Fill = new System.Windows.Media.SolidColorBrush(isHome ? System.Windows.Media.Color.FromArgb(255, 255, 176, 0) : System.Windows.Media.Color.FromArgb(255, 0, 240, 255)),
                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White),
                    StrokeThickness = 2,
                    ToolTip = $"🌌 Sistema: {sys.SystemName}\n🪐 Cuerpos Celestes: {sys.Bodies.Count}\n🌀 Puntos de Salto: {sys.JumpPoints.Count}"
                };

                Canvas.SetLeft(ellipse, p.X - nodeSize / 2);
                Canvas.SetTop(ellipse, p.Y - nodeSize / 2);
                StarMapCanvas.Children.Add(ellipse);

                // Label
                TextBlock lbl = new TextBlock
                {
                    Text = (isHome ? "👑 " : "⭐ ") + sys.SystemName + $" ({sys.Bodies.Count} Cuerpos)",
                    Foreground = new System.Windows.Media.SolidColorBrush(isHome ? System.Windows.Media.Color.FromArgb(255, 255, 176, 0) : System.Windows.Media.Colors.White),
                    FontWeight = FontWeights.Bold,
                    FontSize = 11,
                    Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 11, 16, 26)),
                    Padding = new Thickness(4, 2, 4, 2)
                };

                Canvas.SetLeft(lbl, p.X - 50);
                Canvas.SetTop(lbl, p.Y + nodeSize / 2 + 4);
                StarMapCanvas.Children.Add(lbl);
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
