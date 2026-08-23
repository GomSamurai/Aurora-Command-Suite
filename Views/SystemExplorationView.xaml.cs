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

        private bool _isInitializingMap = false;
        private Point _lastMousePosition;
        private bool _isDraggingMap = false;
        private double _currentZoom = 1.0;

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

            // Populate Map View Modes & System Selector Dropdown
            if (!_isInitializingMap && CmbStarMapViewMode != null)
            {
                _isInitializingMap = true;
                if (CmbStarMapViewMode.Items.Count == 0)
                {
                    CmbStarMapViewMode.Items.Add("🪐 Mapa de Sistema Solar y Órbitas");
                    CmbStarMapViewMode.Items.Add("🌌 Red Galáctica Interestelar (Jump Points)");
                    CmbStarMapViewMode.SelectedIndex = 0;
                }

                if (CmbStarMapSystemSelector != null)
                {
                    CmbStarMapSystemSelector.ItemsSource = systems;
                    CmbStarMapSystemSelector.DisplayMemberPath = "SystemName";
                    if (systems.Count > 0) CmbStarMapSystemSelector.SelectedIndex = 0;
                }
                _isInitializingMap = false;
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

        private void OnMapFilterChanged(object sender, RoutedEventArgs e)
        {
            if (_isInitializingMap) return;
            Render2DStarMap();
        }

        private void SldMapZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (StarMapScaleTransform == null) return;
            _currentZoom = e.NewValue;
            StarMapScaleTransform.ScaleX = _currentZoom;
            StarMapScaleTransform.ScaleY = _currentZoom;
        }

        private void BtnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (SldMapZoom != null) SldMapZoom.Value = Math.Min(5.0, SldMapZoom.Value + 0.25);
        }

        private void BtnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (SldMapZoom != null) SldMapZoom.Value = Math.Max(0.2, SldMapZoom.Value - 0.25);
        }

        private void BrdStarMapContainer_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (SldMapZoom == null) return;
            double zoomDelta = e.Delta > 0 ? 0.20 : -0.20;
            SldMapZoom.Value = Math.Clamp(SldMapZoom.Value + zoomDelta, 0.2, 5.0);
        }

        private void BrdStarMapContainer_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (BrdStarMapContainer == null) return;
            _isDraggingMap = true;
            _lastMousePosition = e.GetPosition(BrdStarMapContainer);
            BrdStarMapContainer.CaptureMouse();
        }

        private void BrdStarMapContainer_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!_isDraggingMap || StarMapTranslateTransform == null || BrdStarMapContainer == null) return;
            Point currentPos = e.GetPosition(BrdStarMapContainer);
            Vector delta = currentPos - _lastMousePosition;
            StarMapTranslateTransform.X += delta.X;
            StarMapTranslateTransform.Y += delta.Y;
            _lastMousePosition = currentPos;
        }

        private void BrdStarMapContainer_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_isDraggingMap && BrdStarMapContainer != null)
            {
                _isDraggingMap = false;
                BrdStarMapContainer.ReleaseMouseCapture();
            }
        }

        private void BtnRefreshStarMap_Click(object sender, RoutedEventArgs e)
        {
            Render2DStarMap();
        }

        private void BtnResetStarMapZoom_Click(object sender, RoutedEventArgs e)
        {
            if (SldMapZoom != null) SldMapZoom.Value = 1.0;
            if (StarMapTranslateTransform != null)
            {
                StarMapTranslateTransform.X = 0;
                StarMapTranslateTransform.Y = 0;
            }
            Render2DStarMap();
        }

        private System.Windows.Media.RadialGradientBrush CreateStarGlowBrush(System.Windows.Media.Color centerColor, System.Windows.Media.Color outerColor)
        {
            var brush = new System.Windows.Media.RadialGradientBrush
            {
                GradientOrigin = new Point(0.5, 0.5),
                Center = new Point(0.5, 0.5),
                RadiusX = 0.5,
                RadiusY = 0.5
            };
            brush.GradientStops.Add(new System.Windows.Media.GradientStop(centerColor, 0.0));
            brush.GradientStops.Add(new System.Windows.Media.GradientStop(centerColor, 0.35));
            brush.GradientStops.Add(new System.Windows.Media.GradientStop(outerColor, 1.0));
            return brush;
        }

        private void Render2DStarMap()
        {
            if (StarMapCanvas == null || _dbService == null) return;
            StarMapCanvas.Children.Clear();

            var systems = _dbService.GetDiscoveredSystems(_currentRaceId);
            if (systems.Count == 0) return;

            bool isOrbitalMode = CmbStarMapViewMode?.SelectedIndex == 0;

            bool showStars = ChkLayerStars?.IsChecked == true;
            bool showPlanets = ChkLayerPlanets?.IsChecked == true;
            bool showColonies = ChkLayerColonies?.IsChecked == true;
            bool showJumpPoints = ChkLayerJumpPoints?.IsChecked == true;
            bool showFleets = ChkLayerFleets?.IsChecked == true;
            bool showMinerals = ChkLayerMinerals?.IsChecked == true;

            double centerX = 1000;
            double centerY = 700;

            if (isOrbitalMode)
            {
                // ==========================================
                // MODE A: SOLAR SYSTEM & ORBITAL PLANETS MAP
                // ==========================================
                StarSystemInfo sys = (CmbStarMapSystemSelector?.SelectedItem as StarSystemInfo) ?? systems[0];

                // 1. Draw Sun / Central Star Corona with Radial Glow FX
                if (showStars)
                {
                    double glowSize = 180;
                    var sunGlow = new System.Windows.Shapes.Ellipse
                    {
                        Width = glowSize,
                        Height = glowSize,
                        Fill = CreateStarGlowBrush(
                            System.Windows.Media.Color.FromArgb(230, 255, 215, 60),
                            System.Windows.Media.Color.FromArgb(0, 255, 120, 0)),
                        ToolTip = $"☀️ Estrella Central: {sys.SystemName}\nClase Espectral: G2V Principal\nLuminosidad: 1.00 Sol\nCuerpos Orbitales: {sys.Bodies.Count}"
                    };
                    Canvas.SetLeft(sunGlow, centerX - glowSize / 2);
                    Canvas.SetTop(sunGlow, centerY - glowSize / 2);
                    StarMapCanvas.Children.Add(sunGlow);

                    double coreSize = 54;
                    var sunCore = new System.Windows.Shapes.Ellipse
                    {
                        Width = coreSize,
                        Height = coreSize,
                        Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 230, 100)),
                        Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 200)),
                        StrokeThickness = 2
                    };
                    Canvas.SetLeft(sunCore, centerX - coreSize / 2);
                    Canvas.SetTop(sunCore, centerY - coreSize / 2);
                    StarMapCanvas.Children.Add(sunCore);

                    TextBlock sunLabel = new TextBlock
                    {
                        Text = $"☀️ Sol ({sys.SystemName})",
                        Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 215, 0)),
                        FontWeight = FontWeights.Bold,
                        FontSize = 12,
                        Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(180, 10, 15, 25)),
                        Padding = new Thickness(6, 2, 6, 2)
                    };
                    Canvas.SetLeft(sunLabel, centerX - 45);
                    Canvas.SetTop(sunLabel, centerY + coreSize / 2 + 6);
                    StarMapCanvas.Children.Add(sunLabel);
                }

                // 2. Draw Orbit Rings & Planets
                if (showPlanets && sys.Bodies.Count > 0)
                {
                    int bodyCount = sys.Bodies.Count;
                    for (int i = 0; i < bodyCount; i++)
                    {
                        var body = sys.Bodies[i];

                        // Calculate dynamic orbital radius scale
                        double orbitRadius = 120 + (i + 1) * 65;
                        if (orbitRadius > 900) orbitRadius = 900;

                        // Concentric Orbit Ring
                        var orbitRing = new System.Windows.Shapes.Ellipse
                        {
                            Width = orbitRadius * 2,
                            Height = orbitRadius * 2,
                            Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(70, 0, 230, 255)),
                            StrokeThickness = 1.2,
                            StrokeDashArray = new System.Windows.Media.DoubleCollection { 4, 4 }
                        };
                        Canvas.SetLeft(orbitRing, centerX - orbitRadius);
                        Canvas.SetTop(orbitRing, centerY - orbitRadius);
                        StarMapCanvas.Children.Add(orbitRing);

                        // Position planet sphere along orbit ring using trigonometric distribution
                        double angle = (2.0 * Math.PI * i) / Math.Min(16, bodyCount) + (i * 0.45);
                        double px = centerX + orbitRadius * Math.Cos(angle);
                        double py = centerY + orbitRadius * Math.Sin(angle);

                        // Determine Visual Planet Colors based on habitability & body type
                        bool isEarthLike = body.ColonyCost == 0 || body.Name.Equals("Earth", StringComparison.OrdinalIgnoreCase) || body.Name.Equals("Tierra", StringComparison.OrdinalIgnoreCase);
                        bool isColonizable = body.ColonyCost > 0 && body.ColonyCost <= 2.5;
                        bool isGasGiant = body.BodyTypeName.Contains("Gas") || body.MassEarth > 10;
                        bool hasMinerals = body.MineralDeposits.Count > 0;
                        bool hasColony = isEarthLike || body.ColonyCost == 0;

                        System.Windows.Media.Color sphereColor = isEarthLike ? System.Windows.Media.Color.FromArgb(255, 0, 180, 255) :
                                                                (isColonizable ? System.Windows.Media.Color.FromArgb(255, 50, 205, 50) :
                                                                (isGasGiant ? System.Windows.Media.Color.FromArgb(255, 255, 140, 0) :
                                                                System.Windows.Media.Color.FromArgb(255, 180, 180, 190)));

                        double planetSize = isEarthLike ? 28 : (isGasGiant ? 34 : 20);

                        // Planet Glow Halo
                        var planetGlow = new System.Windows.Shapes.Ellipse
                        {
                            Width = planetSize + 14,
                            Height = planetSize + 14,
                            Fill = CreateStarGlowBrush(
                                System.Windows.Media.Color.FromArgb(140, sphereColor.R, sphereColor.G, sphereColor.B),
                                System.Windows.Media.Color.FromArgb(0, sphereColor.R, sphereColor.G, sphereColor.B))
                        };
                        Canvas.SetLeft(planetGlow, px - (planetSize + 14) / 2);
                        Canvas.SetTop(planetGlow, py - (planetSize + 14) / 2);
                        StarMapCanvas.Children.Add(planetGlow);

                        // Planet Sphere Core
                        var planetSphere = new System.Windows.Shapes.Ellipse
                        {
                            Width = planetSize,
                            Height = planetSize,
                            Fill = new System.Windows.Media.SolidColorBrush(sphereColor),
                            Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White),
                            StrokeThickness = 1.5,
                            ToolTip = $"🪐 Cuerpo: {body.Name}\nClase: {body.BodyTypeName}\nCosto Colonial: {body.ColonyCostDisplay}\nGravedad: {body.GravityDisplay}\nTemperatura: {body.TempDisplay}\nYacimientos Minerales: {body.MineralDeposits.Count}"
                        };
                        Canvas.SetLeft(planetSphere, px - planetSize / 2);
                        Canvas.SetTop(planetSphere, py - planetSize / 2);
                        StarMapCanvas.Children.Add(planetSphere);

                        // Planet Name Label
                        TextBlock planetLabel = new TextBlock
                        {
                            Text = $"🪐 {body.Name}",
                            Foreground = new System.Windows.Media.SolidColorBrush(isEarthLike ? System.Windows.Media.Color.FromArgb(255, 0, 240, 255) : System.Windows.Media.Colors.White),
                            FontWeight = FontWeights.Bold,
                            FontSize = 10.5,
                            Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(190, 8, 12, 20)),
                            Padding = new Thickness(4, 1, 4, 1)
                        };
                        Canvas.SetLeft(planetLabel, px - 35);
                        Canvas.SetTop(planetLabel, py + planetSize / 2 + 3);
                        StarMapCanvas.Children.Add(planetLabel);

                        // 3. Colony Badge Overlay
                        if (showColonies && hasColony)
                        {
                            TextBlock colonyBadge = new TextBlock
                            {
                                Text = "🏛️ [COLONIA IMPERIAL]",
                                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 50, 205, 50)),
                                FontWeight = FontWeights.Bold,
                                FontSize = 9.5,
                                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 5, 25, 10)),
                                Padding = new Thickness(4, 1, 4, 1)
                            };
                            Canvas.SetLeft(colonyBadge, px - 45);
                            Canvas.SetTop(colonyBadge, py - planetSize / 2 - 16);
                            StarMapCanvas.Children.Add(colonyBadge);
                        }

                        // 4. Mineral Survey Overlay
                        if (showMinerals && hasMinerals)
                        {
                            TextBlock mineralBadge = new TextBlock
                            {
                                Text = $"💎 {body.MineralDeposits.Count} Minerales",
                                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 215, 0)),
                                FontWeight = FontWeights.Bold,
                                FontSize = 9,
                                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(180, 25, 20, 5)),
                                Padding = new Thickness(3, 1, 3, 1)
                            };
                            Canvas.SetLeft(mineralBadge, px - 35);
                            Canvas.SetTop(mineralBadge, py + planetSize / 2 + 18);
                            StarMapCanvas.Children.Add(mineralBadge);
                        }
                    }
                }

                // 5. Draw Jump Points (Warp Nodes) around perimeter
                if (showJumpPoints && sys.JumpPoints.Count > 0)
                {
                    int jpCount = sys.JumpPoints.Count;
                    for (int j = 0; j < jpCount; j++)
                    {
                        var jp = sys.JumpPoints[j];
                        double jpAngle = (2.0 * Math.PI * j) / jpCount + 0.8;
                        double jpDistance = 650;

                        double jpx = centerX + jpDistance * Math.Cos(jpAngle);
                        double jpy = centerY + jpDistance * Math.Sin(jpAngle);

                        // Connecting Warp Energy Line
                        var warpLine = new System.Windows.Shapes.Line
                        {
                            X1 = centerX,
                            Y1 = centerY,
                            X2 = jpx,
                            Y2 = jpy,
                            Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(140, 255, 170, 0)),
                            StrokeThickness = 1.5,
                            StrokeDashArray = new System.Windows.Media.DoubleCollection { 6, 4 }
                        };
                        StarMapCanvas.Children.Add(warpLine);

                        // Jump Portal Node
                        double jpSize = 30;
                        var jpNode = new System.Windows.Shapes.Ellipse
                        {
                            Width = jpSize,
                            Height = jpSize,
                            Fill = CreateStarGlowBrush(
                                System.Windows.Media.Color.FromArgb(255, 255, 180, 0),
                                System.Windows.Media.Color.FromArgb(0, 255, 100, 0)),
                            Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White),
                            StrokeThickness = 2,
                            ToolTip = $"🌀 Punto de Salto #{jp.JumpPointID}\nDestino: {jp.DestinationSystemName}\nPuerta de Salto: {jp.GateDisplay}"
                        };
                        Canvas.SetLeft(jpNode, jpx - jpSize / 2);
                        Canvas.SetTop(jpNode, jpy - jpSize / 2);
                        StarMapCanvas.Children.Add(jpNode);

                        TextBlock jpLabel = new TextBlock
                        {
                            Text = $"🌀 JP -> {jp.DestinationSystemName}",
                            Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 180, 0)),
                            FontWeight = FontWeights.Bold,
                            FontSize = 10,
                            Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(190, 20, 15, 5)),
                            Padding = new Thickness(4, 1, 4, 1)
                        };
                        Canvas.SetLeft(jpLabel, jpx - 40);
                        Canvas.SetTop(jpLabel, jpy + jpSize / 2 + 2);
                        StarMapCanvas.Children.Add(jpLabel);
                    }
                }

                // 6. Draw Active Fleets in System
                if (showFleets)
                {
                    var fleets = _dbService.GetEmpireFleetSummary(_currentRaceId);
                    if (fleets.Count > 0)
                    {
                        for (int f = 0; f < Math.Min(3, fleets.Count); f++)
                        {
                            var fleet = fleets[f];
                            double fleetX = centerX + 220 + (f * 120);
                            double fleetY = centerY - 140 - (f * 60);

                            TextBlock fleetIcon = new TextBlock
                            {
                                Text = $"🛸 {fleet.FleetName} ({fleet.ShipCount} Naves)",
                                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 50, 205, 50)),
                                FontWeight = FontWeights.Bold,
                                FontSize = 10.5,
                                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 5, 20, 10)),
                                Padding = new Thickness(5, 2, 5, 2),
                                ToolTip = $"🛸 Escuadra: {fleet.FleetName}\nNaves: {fleet.ShipCount}\nVelocidad: {fleet.SpeedKmS:N0} km/s"
                            };
                            Canvas.SetLeft(fleetIcon, fleetX);
                            Canvas.SetTop(fleetIcon, fleetY);
                            StarMapCanvas.Children.Add(fleetIcon);
                        }
                    }
                }
            }
            else
            {
                // ==========================================
                // MODE B: GALACTIC WARP NETWORK MAP
                // ==========================================
                int count = systems.Count;
                double radius = Math.Min(420, 180 + count * 40);

                Dictionary<int, Point> systemCoords = new Dictionary<int, Point>();

                for (int i = 0; i < count; i++)
                {
                    var sys = systems[i];
                    double angle = (2.0 * Math.PI * i) / count;
                    if (i == 0)
                    {
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
                                    StrokeThickness = 2.5,
                                    StrokeDashArray = jp.GateID > 0 ? null : new System.Windows.Media.DoubleCollection { 4, 4 }
                                };
                                StarMapCanvas.Children.Add(line);
                            }
                        }
                    }
                }

                // 2. Draw System Nodes & Corona Halos
                foreach (var sys in systems)
                {
                    if (!systemCoords.ContainsKey(sys.SystemID)) continue;
                    Point p = systemCoords[sys.SystemID];

                    bool isHome = sys.SystemID == 1 || sys.SystemName.Equals("Sol", StringComparison.OrdinalIgnoreCase);

                    double nodeSize = isHome ? 42 : 32;

                    // Glowing Corona Halo
                    var sysGlow = new System.Windows.Shapes.Ellipse
                    {
                        Width = nodeSize + 24,
                        Height = nodeSize + 24,
                        Fill = CreateStarGlowBrush(
                            isHome ? System.Windows.Media.Color.FromArgb(200, 255, 180, 0) : System.Windows.Media.Color.FromArgb(180, 0, 230, 255),
                            System.Windows.Media.Color.FromArgb(0, 0, 0, 0))
                    };
                    Canvas.SetLeft(sysGlow, p.X - (nodeSize + 24) / 2);
                    Canvas.SetTop(sysGlow, p.Y - (nodeSize + 24) / 2);
                    StarMapCanvas.Children.Add(sysGlow);

                    var ellipse = new System.Windows.Shapes.Ellipse
                    {
                        Width = nodeSize,
                        Height = nodeSize,
                        Fill = new System.Windows.Media.SolidColorBrush(isHome ? System.Windows.Media.Color.FromArgb(255, 255, 180, 0) : System.Windows.Media.Color.FromArgb(255, 0, 230, 255)),
                        Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White),
                        StrokeThickness = 2,
                        ToolTip = $"🌌 Sistema Estelar: {sys.SystemName}\n🪐 Cuerpos Celestes: {sys.Bodies.Count}\n🌀 Puntos de Salto: {sys.JumpPoints.Count}"
                    };

                    Canvas.SetLeft(ellipse, p.X - nodeSize / 2);
                    Canvas.SetTop(ellipse, p.Y - nodeSize / 2);
                    StarMapCanvas.Children.Add(ellipse);

                    // System Label
                    TextBlock lbl = new TextBlock
                    {
                        Text = (isHome ? "👑 " : "⭐ ") + sys.SystemName + $" ({sys.Bodies.Count} Cuerpos)",
                        Foreground = new System.Windows.Media.SolidColorBrush(isHome ? System.Windows.Media.Color.FromArgb(255, 255, 180, 0) : System.Windows.Media.Colors.White),
                        FontWeight = FontWeights.Bold,
                        FontSize = 11.5,
                        Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 11, 16, 26)),
                        Padding = new Thickness(5, 2, 5, 2)
                    };

                    Canvas.SetLeft(lbl, p.X - 55);
                    Canvas.SetTop(lbl, p.Y + nodeSize / 2 + 4);
                    StarMapCanvas.Children.Add(lbl);
                }
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
