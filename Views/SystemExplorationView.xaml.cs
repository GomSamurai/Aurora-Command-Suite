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

        private void CenterMapOnScreen(double targetCanvasX = 1000, double targetCanvasY = 700)
        {
            if (BrdStarMapContainer == null || StarMapScaleTransform == null || StarMapTranslateTransform == null || SldMapZoom == null) return;

            double containerWidth = BrdStarMapContainer.ActualWidth > 0 ? BrdStarMapContainer.ActualWidth : 1200;
            double containerHeight = BrdStarMapContainer.ActualHeight > 0 ? BrdStarMapContainer.ActualHeight : 750;

            _isInitializingMap = true;
            _currentZoom = 1.0;
            SldMapZoom.Value = 1.0;
            _isInitializingMap = false;

            StarMapScaleTransform.ScaleX = 1.0;
            StarMapScaleTransform.ScaleY = 1.0;

            StarMapTranslateTransform.X = (containerWidth / 2.0) - targetCanvasX;
            StarMapTranslateTransform.Y = (containerHeight / 2.0) - targetCanvasY;
        }

        private void SldMapZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isInitializingMap || StarMapScaleTransform == null || StarMapTranslateTransform == null || BrdStarMapContainer == null) return;

            double oldZoom = _currentZoom;
            double newZoom = e.NewValue;
            if (Math.Abs(newZoom - oldZoom) < 0.0001) return;

            double containerWidth = BrdStarMapContainer.ActualWidth > 0 ? BrdStarMapContainer.ActualWidth : 1200;
            double containerHeight = BrdStarMapContainer.ActualHeight > 0 ? BrdStarMapContainer.ActualHeight : 750;
            Point centerPos = new Point(containerWidth / 2.0, containerHeight / 2.0);

            double scaleRatio = newZoom / oldZoom;
            _currentZoom = newZoom;

            StarMapTranslateTransform.X = centerPos.X - (centerPos.X - StarMapTranslateTransform.X) * scaleRatio;
            StarMapTranslateTransform.Y = centerPos.Y - (centerPos.Y - StarMapTranslateTransform.Y) * scaleRatio;

            StarMapScaleTransform.ScaleX = newZoom;
            StarMapScaleTransform.ScaleY = newZoom;

            Render2DStarMap();
        }

        private void BtnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (SldMapZoom != null) SldMapZoom.Value = Math.Min(10.0, SldMapZoom.Value * 1.3);
        }

        private void BtnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (SldMapZoom != null) SldMapZoom.Value = Math.Max(0.02, SldMapZoom.Value / 1.3);
        }

        private void BrdStarMapContainer_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (SldMapZoom == null || StarMapScaleTransform == null || StarMapTranslateTransform == null || BrdStarMapContainer == null) return;

            Point mousePos = e.GetPosition(BrdStarMapContainer);

            double oldZoom = _currentZoom;
            double zoomFactor = e.Delta > 0 ? 1.2 : (1.0 / 1.2);
            double newZoom = Math.Clamp(oldZoom * zoomFactor, 0.02, 10.0);

            if (Math.Abs(newZoom - oldZoom) < 0.0001) return;

            double scaleRatio = newZoom / oldZoom;
            _currentZoom = newZoom;

            // Anchor Zoom precisely around mouse cursor position!
            StarMapTranslateTransform.X = mousePos.X - (mousePos.X - StarMapTranslateTransform.X) * scaleRatio;
            StarMapTranslateTransform.Y = mousePos.Y - (mousePos.Y - StarMapTranslateTransform.Y) * scaleRatio;

            StarMapScaleTransform.ScaleX = newZoom;
            StarMapScaleTransform.ScaleY = newZoom;

            _isInitializingMap = true;
            SldMapZoom.Value = newZoom;
            _isInitializingMap = false;

            Render2DStarMap();
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
            CenterMapOnScreen();
            Render2DStarMap();
        }

        private void BtnResetStarMapZoom_Click(object sender, RoutedEventArgs e)
        {
            CenterMapOnScreen();
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

        private void ApplyInverseScale(UIElement element)
        {
            double invScale = 1.0 / Math.Max(0.02, _currentZoom);
            // Clamp inverse scale so text and badges stay crisp and comfortable (between 0.25x and 3.5x)
            invScale = Math.Min(3.5, Math.Max(0.25, invScale));
            var scaleTransform = new System.Windows.Media.ScaleTransform(invScale, invScale);
            element.RenderTransform = scaleTransform;
            element.RenderTransformOrigin = new Point(0.5, 0.5);
        }

        private Point GetBodyCanvasCoordinates(SystemBodyInfo body, double centerX, double centerY, out double orbitRadius)
        {
            // 1. Calculate Real Angle from database Bearing or Xcor/Ycor
            double angleRad = (body.Bearing * Math.PI) / 180.0;
            if (Math.Abs(body.Xcor) > 0.001 || Math.Abs(body.Ycor) > 0.001)
            {
                angleRad = Math.Atan2(-body.Ycor, body.Xcor);
            }

            // 2. Calculate Real Astronomical Distance Scale (in AU)
            double distAU = body.OrbitalDistAU;
            if (distAU <= 0 && (Math.Abs(body.Xcor) > 0 || Math.Abs(body.Ycor) > 0))
            {
                distAU = Math.Sqrt(body.Xcor * body.Xcor + body.Ycor * body.Ycor) / 149597870.7;
            }

            // 3. Dynamic Radius Scaling per Astronomical Region
            if (distAU <= 0) distAU = 1.0;

            if (distAU <= 2.2)
            {
                // Terrestrial Planets (Mercury, Venus, Earth, Mars)
                orbitRadius = 110 + (distAU * 240);
            }
            else if (distAU <= 4.5)
            {
                // Main Asteroid Belt (Ceres, Vesta, Pallas...)
                orbitRadius = 640 + (distAU - 2.2) * 110;
            }
            else if (distAU <= 35.0)
            {
                // Gas Giants & Outer Planets (Jupiter, Saturn, Uranus, Neptune)
                orbitRadius = 900 + Math.Log(distAU - 3.5) * 340;
            }
            else
            {
                // Trans-Neptunian Objects, Kuiper Belt & Oort Cloud (Pluto, Eris, Sedna, Makemake...)
                orbitRadius = 1750 + Math.Log(distAU - 30.0) * 380;
            }

            double px = centerX + orbitRadius * Math.Cos(angleRad);
            double py = centerY - orbitRadius * Math.Sin(angleRad);

            return new Point(px, py);
        }

        private void Render2DStarMap()
        {
            if (StarMapCanvas == null || _dbService == null) return;
            StarMapCanvas.Children.Clear();

            var systems = _dbService.GetDiscoveredSystems(_currentRaceId);
            if (systems.Count == 0) return;

            // Auto-Center if translation was uninitialized
            if (StarMapTranslateTransform != null && StarMapTranslateTransform.X == 0 && StarMapTranslateTransform.Y == 0)
            {
                CenterMapOnScreen(1000, 700);
            }

            bool isOrbitalMode = CmbStarMapViewMode?.SelectedIndex == 0;

            bool showStars = ChkLayerStars?.IsChecked == true;
            bool showPlanets = ChkLayerPlanets?.IsChecked == true;
            bool showMoons = ChkLayerMoons?.IsChecked == true;
            bool showAsteroids = ChkLayerAsteroids?.IsChecked == true;
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
                    ApplyInverseScale(sunLabel);
                    Canvas.SetLeft(sunLabel, centerX - 45);
                    Canvas.SetTop(sunLabel, centerY + coreSize / 2 + 6);
                    StarMapCanvas.Children.Add(sunLabel);

                    // 1b. Render Companion Stars (Binary / Trinary systems: Sol B, Proxima, etc.)
                    if (sys.Stars.Count > 1)
                    {
                        for (int s = 1; s < sys.Stars.Count; s++)
                        {
                            var companionStar = sys.Stars[s];
                            double starAngle = (companionStar.Bearing * Math.PI) / 180.0;
                            if (starAngle == 0) starAngle = (2.0 * Math.PI * s) / sys.Stars.Count;

                            double starDistance = companionStar.OrbitalDistance > 0 ? (800 + Math.Log(companionStar.OrbitalDistance + 1.0) * 350) : (750 + s * 150);

                            double starX = centerX + starDistance * Math.Cos(starAngle);
                            double starY = centerY - starDistance * Math.Sin(starAngle);

                            double compGlow = 140;
                            var compStarGlow = new System.Windows.Shapes.Ellipse
                            {
                                Width = compGlow,
                                Height = compGlow,
                                Fill = CreateStarGlowBrush(
                                    System.Windows.Media.Color.FromArgb(200, 255, 100, 100),
                                    System.Windows.Media.Color.FromArgb(0, 255, 0, 0)),
                                ToolTip = $"☀️ {companionStar.Name} ({companionStar.StarTypeDisplay})\nLuminosidad: {companionStar.Luminosity:F2} Sol"
                            };
                            Canvas.SetLeft(compStarGlow, starX - compGlow / 2);
                            Canvas.SetTop(compStarGlow, starY - compGlow / 2);
                            StarMapCanvas.Children.Add(compStarGlow);

                            double compCore = 40;
                            var compStarCore = new System.Windows.Shapes.Ellipse
                            {
                                Width = compCore,
                                Height = compCore,
                                Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 150, 150)),
                                Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White),
                                StrokeThickness = 1.5
                            };
                            Canvas.SetLeft(compStarCore, starX - compCore / 2);
                            Canvas.SetTop(compStarCore, starY - compCore / 2);
                            StarMapCanvas.Children.Add(compStarCore);

                            TextBlock compLabel = new TextBlock
                            {
                                Text = $"☀️ {companionStar.Name} ({companionStar.StarTypeDisplay})",
                                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 150, 150)),
                                FontWeight = FontWeights.Bold,
                                FontSize = 11,
                                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(190, 25, 10, 10)),
                                Padding = new Thickness(5, 2, 5, 2)
                            };
                            ApplyInverseScale(compLabel);
                            Canvas.SetLeft(compLabel, starX - 45);
                            Canvas.SetTop(compLabel, starY + compCore / 2 + 4);
                            StarMapCanvas.Children.Add(compLabel);
                        }
                    }
                }

                // 2. Separate Primary Planets vs Moons vs Asteroids/Comets
                Dictionary<int, Point> planetPositions = new Dictionary<int, Point>();
                var mainPlanets = sys.Bodies.Where(b => !b.IsMoon && !b.IsAsteroidOrComet).ToList();
                var moonBodies = sys.Bodies.Where(b => b.IsMoon && !b.IsAsteroidOrComet).ToList();
                var asteroidBodies = sys.Bodies.Where(b => b.IsAsteroidOrComet).ToList();

                if (showPlanets && mainPlanets.Count > 0)
                {
                    int bodyCount = mainPlanets.Count;
                    for (int i = 0; i < bodyCount; i++)
                    {
                        var body = mainPlanets[i];

                        // Calculate Real Coordinates & Concentric Orbit Radius from DB
                        Point p = GetBodyCanvasCoordinates(body, centerX, centerY, out double orbitRadius);

                        // Concentric Orbit Ring around Central Sun
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

                        double px = p.X;
                        double py = p.Y;

                        // Store planet position for satellite moon docking
                        planetPositions[body.SystemBodyID] = new Point(px, py);

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

                        // Planet Name Label with Inverse Scale
                        TextBlock planetLabel = new TextBlock
                        {
                            Text = $"🪐 {body.Name}",
                            Foreground = new System.Windows.Media.SolidColorBrush(isEarthLike ? System.Windows.Media.Color.FromArgb(255, 0, 240, 255) : System.Windows.Media.Colors.White),
                            FontWeight = FontWeights.Bold,
                            FontSize = 10.5,
                            Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(190, 8, 12, 20)),
                            Padding = new Thickness(4, 1, 4, 1)
                        };
                        ApplyInverseScale(planetLabel);
                        Canvas.SetLeft(planetLabel, px - 35);
                        Canvas.SetTop(planetLabel, py + planetSize / 2 + 3);
                        StarMapCanvas.Children.Add(planetLabel);

                        // Colony Badge Overlay
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
                            ApplyInverseScale(colonyBadge);
                            Canvas.SetLeft(colonyBadge, px - 45);
                            Canvas.SetTop(colonyBadge, py - planetSize / 2 - 16);
                            StarMapCanvas.Children.Add(colonyBadge);
                        }

                        // Mineral Survey Overlay
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
                            ApplyInverseScale(mineralBadge);
                            Canvas.SetLeft(mineralBadge, px - 35);
                            Canvas.SetTop(mineralBadge, py + planetSize / 2 + 18);
                            StarMapCanvas.Children.Add(mineralBadge);
                        }
                    }
                }

                // 3. Render Natural Satellites / Moons (Grouped per Parent Planet to avoid clutter)
                if (showMoons && moonBodies.Count > 0)
                {
                    var bodyLookup = sys.Bodies.ToDictionary(b => b.SystemBodyID, b => b);
                    var moonsByParent = moonBodies.GroupBy(m => m.ParentBodyID).ToDictionary(g => g.Key, g => g.ToList());

                    foreach (var kvp in moonsByParent)
                    {
                        int parentId = kvp.Key;
                        var parentMoons = kvp.Value;
                        SystemBodyInfo? parentBody = (parentId > 0 && bodyLookup.ContainsKey(parentId)) ? bodyLookup[parentId] : null;

                        Point parentPos = (parentId > 0 && planetPositions.ContainsKey(parentId))
                            ? planetPositions[parentId]
                            : (planetPositions.Values.Count > 0 ? planetPositions.Values.First() : new Point(centerX + 200, centerY + 200));

                        for (int mIdx = 0; mIdx < parentMoons.Count; mIdx++)
                        {
                            var moon = parentMoons[mIdx];

                            double dx = (parentBody != null) ? (moon.Xcor - parentBody.Xcor) : moon.Xcor;
                            double dy = (parentBody != null) ? (moon.Ycor - parentBody.Ycor) : moon.Ycor;

                            double moonAngleRad = (moon.Bearing * Math.PI) / 180.0;
                            if (Math.Abs(dx) > 0.001 || Math.Abs(dy) > 0.001)
                            {
                                moonAngleRad = Math.Atan2(-dy, dx);
                            }
                            else if (moon.Bearing == 0)
                            {
                                moonAngleRad = (2.0 * Math.PI * mIdx) / Math.Max(1, parentMoons.Count) + (mIdx * 0.35);
                            }

                            double moonOrbitRadius = 35 + ((mIdx % 4) * 16) + ((mIdx / 4) * 22);

                            // Draw Sub-Orbit Ring for parent planet
                            if (mIdx % 4 == 0)
                            {
                                var moonRing = new System.Windows.Shapes.Ellipse
                                {
                                    Width = moonOrbitRadius * 2,
                                    Height = moonOrbitRadius * 2,
                                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(60, 180, 180, 200)),
                                    StrokeThickness = 0.8,
                                    StrokeDashArray = new System.Windows.Media.DoubleCollection { 2, 2 }
                                };
                                Canvas.SetLeft(moonRing, parentPos.X - moonOrbitRadius);
                                Canvas.SetTop(moonRing, parentPos.Y - moonOrbitRadius);
                                StarMapCanvas.Children.Add(moonRing);
                            }

                            double mx = parentPos.X + moonOrbitRadius * Math.Cos(moonAngleRad);
                            double my = parentPos.Y - moonOrbitRadius * Math.Sin(moonAngleRad);

                            double moonSize = 8;
                            var moonSphere = new System.Windows.Shapes.Ellipse
                            {
                                Width = moonSize,
                                Height = moonSize,
                                Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 190, 195, 205)),
                                Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White),
                                StrokeThickness = 0.8,
                                ToolTip = $"🌕 Satélite / Luna: {moon.Name}\nClase: {moon.BodyTypeName}\nGravedad: {moon.GravityDisplay}\nTemperatura: {moon.TempDisplay}"
                            };
                            Canvas.SetLeft(moonSphere, mx - moonSize / 2);
                            Canvas.SetTop(moonSphere, my - moonSize / 2);
                            StarMapCanvas.Children.Add(moonSphere);

                            // LOD Rule: Show text label ONLY if zoomed in or if planet has <= 3 moons
                            if (_currentZoom >= 1.0 || parentMoons.Count <= 3)
                            {
                                TextBlock moonLabel = new TextBlock
                                {
                                    Text = $"🌕 {moon.Name}",
                                    Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(220, 220, 220, 220)),
                                    FontWeight = FontWeights.Normal,
                                    FontSize = 8.5,
                                    Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(170, 10, 10, 15)),
                                    Padding = new Thickness(3, 1, 3, 1)
                                };
                                ApplyInverseScale(moonLabel);
                                Canvas.SetLeft(moonLabel, mx - 18);
                                Canvas.SetTop(moonLabel, my + moonSize / 2 + 2);
                                StarMapCanvas.Children.Add(moonLabel);
                            }
                        }
                    }
                }

                // 4. Render Asteroids & Comets (Green Dots at REAL Database Astronomical Coordinates!)
                if (showAsteroids && asteroidBodies.Count > 0)
                {
                    for (int aIdx = 0; aIdx < asteroidBodies.Count; aIdx++)
                    {
                        var ast = asteroidBodies[aIdx];

                        Point astPoint = GetBodyCanvasCoordinates(ast, centerX, centerY, out double astOrbitRadius);
                        double ax = astPoint.X;
                        double ay = astPoint.Y;

                        double astSize = 5;
                        var astDot = new System.Windows.Shapes.Ellipse
                        {
                            Width = astSize,
                            Height = astSize,
                            Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 255, 127)), // Green dot
                            ToolTip = $"☄️ Asteroide / Cometa: {ast.Name}\nDistancia Orbital: {ast.OrbitalDistDisplay}\nYacimientos: {ast.MineralDeposits.Count}"
                        };
                        Canvas.SetLeft(astDot, ax - astSize / 2);
                        Canvas.SetTop(astDot, ay - astSize / 2);
                        StarMapCanvas.Children.Add(astDot);

                        // LOD Rule: Show asteroid label only when zoomed in close (_currentZoom >= 1.5)
                        if (_currentZoom >= 1.5)
                        {
                            TextBlock astLabel = new TextBlock
                            {
                                Text = ast.Name,
                                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(220, 0, 255, 127)),
                                FontSize = 8.0,
                                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(160, 5, 15, 10)),
                                Padding = new Thickness(2, 1, 2, 1)
                            };
                            ApplyInverseScale(astLabel);
                            Canvas.SetLeft(astLabel, ax - 15);
                            Canvas.SetTop(astLabel, ay + astSize / 2 + 1);
                            StarMapCanvas.Children.Add(astLabel);
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
                        double jpDistance = 1500;

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
                        ApplyInverseScale(jpLabel);
                        Canvas.SetLeft(jpLabel, jpx - 40);
                        Canvas.SetTop(jpLabel, jpy + jpSize / 2 + 2);
                        StarMapCanvas.Children.Add(jpLabel);
                    }
                }

                // 6. Draw Active Fleets with Trajectory Vector & Speed (FILTER: ShipCount > 0 ONLY!)
                if (showFleets)
                {
                    var fleets = _dbService.GetEmpireFleetSummary(_currentRaceId).Where(f => f.ShipCount > 0).ToList();
                    if (fleets.Count > 0)
                    {
                        for (int f = 0; f < fleets.Count; f++)
                        {
                            var fleet = fleets[f];

                            // Anchor fleet near Earth or Primary Planet
                            double fleetX = centerX + 180 + (f * 120);
                            double fleetY = centerY - 120 - (f * 60);

                            if (planetPositions.Count > 0)
                            {
                                Point targetPlanet = planetPositions.Values.First();
                                fleetX = targetPlanet.X + 40 + (f * 50);
                                fleetY = targetPlanet.Y - 40 - (f * 35);
                            }

                            // Render Directional Trajectory Vector Line if Moving (SpeedKmS > 0)
                            if (fleet.SpeedKmS > 0)
                            {
                                double bearingRad = (fleet.Bearing * Math.PI) / 180.0;
                                double vectorLen = Math.Min(120, 50 + (fleet.SpeedKmS / 100.0));
                                double endX = fleetX + vectorLen * Math.Cos(bearingRad);
                                double endY = fleetY + vectorLen * Math.Sin(bearingRad);

                                var vectorLine = new System.Windows.Shapes.Line
                                {
                                    X1 = fleetX,
                                    Y1 = fleetY,
                                    X2 = endX,
                                    Y2 = endY,
                                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 50, 205, 50)),
                                    StrokeThickness = 2.0,
                                    StrokeDashArray = new System.Windows.Media.DoubleCollection { 6, 3 }
                                };
                                StarMapCanvas.Children.Add(vectorLine);
                            }

                            TextBlock fleetIcon = new TextBlock
                            {
                                Text = $"🛸 {fleet.FleetName} ({fleet.ShipCount} Naves) [{fleet.HeadingDisplay}]",
                                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 50, 205, 50)),
                                FontWeight = FontWeights.Bold,
                                FontSize = 10.5,
                                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(210, 5, 25, 10)),
                                Padding = new Thickness(6, 3, 6, 3),
                                ToolTip = $"🛸 Escuadra: {fleet.FleetName}\nNaves Activas: {fleet.ShipCount}\nNave Insignia: {fleet.FlagshipName}\nVelocidad: {fleet.SpeedKmS:N0} km/s\nRumbo: {fleet.Bearing:F0}°\nUbicación: {fleet.SystemLocation}"
                            };
                            ApplyInverseScale(fleetIcon);
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

                    ApplyInverseScale(lbl);
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
