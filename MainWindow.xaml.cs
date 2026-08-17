using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AuroraDesignSuite.Models;
using AuroraDesignSuite.Services;
using AuroraDesignSuite.Views;

namespace AuroraDesignSuite
{
    public partial class MainWindow : Window
    {
        private DatabaseService? _dbService;
        private FileSystemWatcher? _dbWatcher;
        private System.Windows.Threading.DispatcherTimer? _liveSyncTimer;
        private DateTime _lastDbWriteTime = DateTime.MinValue;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CmbThemeSelector.ItemsSource = ThemeManager.AvailableThemes;

            // Restore User Preferences
            var prefs = UserPreferencesService.LoadPreferences();
            if (prefs.WindowWidth > 600) Width = prefs.WindowWidth;
            if (prefs.WindowHeight > 400) Height = prefs.WindowHeight;
            if (prefs.IsMaximized) WindowState = WindowState.Maximized;

            var matchTheme = ThemeManager.AvailableThemes.FirstOrDefault(t => t.Name.Equals(prefs.SelectedTheme, StringComparison.OrdinalIgnoreCase));
            if (matchTheme != null)
            {
                CmbThemeSelector.SelectedItem = matchTheme;
            }
            else
            {
                CmbThemeSelector.SelectedIndex = 0;
            }

            Closing += MainWindow_Closing;

            string[] args = Environment.GetCommandLineArgs();
            string? dbArg = (args.Length > 1 && File.Exists(args[1])) ? args[1] : null;

            string[] candidatePaths = new[]
            {
                dbArg ?? "",
                prefs.LastDbPath,
                Path.Combine(Directory.GetCurrentDirectory(), "AuroraDB.db"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AuroraDB.db"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "AuroraDB.db"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "AuroraDB.db"),
                Path.Combine(Directory.GetCurrentDirectory(), "..", "AuroraDB.db"),
                @"c:\VSCODE\Aurora271Full\AuroraDB.db"
            };

            string dbPath = candidatePaths.FirstOrDefault(f => !string.IsNullOrEmpty(f) && File.Exists(f)) ?? @"c:\VSCODE\Aurora271Full\AuroraDB.db";
            LoadDatabasePath(dbPath);
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                _dbWatcher?.Dispose();
                _dbWatcher = null;
                _liveSyncTimer?.Stop();
                _liveSyncTimer = null;
                LiveSyncBridge.OnGameSyncReceived -= HandleLiveSyncEvent;

                if (_timeEventsWidgetWindow != null)
                {
                    _timeEventsWidgetWindow.CloseWidget();
                    _timeEventsWidgetWindow.Close();
                    _timeEventsWidgetWindow = null;
                }
            }
            catch { }

            try
            {
                var prefs = new UserPreferences
                {
                    WindowWidth = Width,
                    WindowHeight = Height,
                    IsMaximized = WindowState == WindowState.Maximized,
                    SelectedTheme = (CmbThemeSelector.SelectedItem as ThemeOption)?.Name ?? "Cyberpunk Obsidian",
                    SelectedEmpireId = (CmbGlobalEmpire.SelectedItem as Empire)?.RaceID ?? -1
                };
                UserPreferencesService.SavePreferences(prefs);
            }
            catch { }

            try
            {
                Application.Current.Shutdown();
            }
            catch { }
        }

        private void LoadDatabasePath(string dbPath)
        {
            if (!File.Exists(dbPath)) return;

            _dbService = new DatabaseService(dbPath);
            if (_dbService.TestConnection(out _))
            {
                var empires = _dbService.GetEmpires();
                CmbGlobalEmpire.ItemsSource = empires;
                if (empires.Count > 0)
                {
                    var prefs = UserPreferencesService.LoadPreferences();
                    var savedEmp = empires.FirstOrDefault(x => x.RaceID == prefs.SelectedEmpireId);
                    CmbGlobalEmpire.SelectedItem = savedEmp ?? empires[0];
                }
                RefreshActiveTab();
                SetupLiveSyncWatcher(dbPath);
            }
            else
            {
                MessageBox.Show($"No se pudo conectar a la base de datos de Aurora 4X en:\n{dbPath}", "Error de Conexión BD", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SetupLiveSyncWatcher(string dbPath)
        {
            try
            {
                _dbWatcher?.Dispose();
                _dbWatcher = null;

                string dir = Path.GetDirectoryName(dbPath) ?? "";
                string filename = Path.GetFileName(dbPath);

                if (Directory.Exists(dir))
                {
                    _dbWatcher = new FileSystemWatcher(dir, filename)
                    {
                        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName,
                        EnableRaisingEvents = true
                    };
                    _dbWatcher.Changed += (s, e) => TriggerLiveRefresh();
                    _dbWatcher.Created += (s, e) => TriggerLiveRefresh();
                }

                _liveSyncTimer?.Stop();
                _liveSyncTimer = new System.Windows.Threading.DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(1500)
                };
                _liveSyncTimer.Tick += (s, e) => CheckDatabaseFileUpdate(dbPath);
                _liveSyncTimer.Start();

                LiveSyncBridge.OnGameSyncReceived -= HandleLiveSyncEvent;
                LiveSyncBridge.OnGameSyncReceived += HandleLiveSyncEvent;
            }
            catch { }
        }

        private void CheckDatabaseFileUpdate(string dbPath)
        {
            try
            {
                if (!File.Exists(dbPath)) return;
                RefreshActiveTab();
            }
            catch { }
        }

        private void HandleLiveSyncEvent(string action)
        {
            Dispatcher.Invoke(() => RefreshActiveTab());
        }

        private void TriggerLiveRefresh()
        {
            Dispatcher.InvokeAsync(() =>
            {
                RefreshActiveTab();
            }, System.Windows.Threading.DispatcherPriority.Background);
        }

        private void BtnManualSync_Click(object sender, RoutedEventArgs e)
        {
            if (_dbService == null) return;

            try
            {
                // 1. Clear SQLite Connection pools to bypass cached pages
                Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();

                // 2. Reload empire selector list with fresh names/titles from database
                int currentRaceId = (CmbGlobalEmpire.SelectedItem as Empire)?.RaceID ?? -1;
                var empires = _dbService.GetEmpires();
                CmbGlobalEmpire.ItemsSource = empires;

                if (empires.Count > 0)
                {
                    var savedEmp = empires.FirstOrDefault(x => x.RaceID == currentRaceId);
                    CmbGlobalEmpire.SelectedItem = savedEmp ?? empires[0];
                }

                // 3. Force complete refresh of current tab
                RefreshActiveTab();

                // 4. Visual Feedback
                if (BtnManualSync != null)
                {
                    BtnManualSync.Content = "✅ SINCRONIZADO";
                    var timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(1.5) };
                    timer.Tick += (s, ev) =>
                    {
                        BtnManualSync.Content = "🔄 SINCRONIZAR VIVO";
                        timer.Stop();
                    };
                    timer.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al sincronizar datos de la base de datos: {ex.Message}", "Error de Sincronización", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnChangeDb_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Selecciona la base de datos AuroraDB.db de tu carpeta de Aurora 4X",
                Filter = "Aurora 4X Database (AuroraDB.db)|AuroraDB.db|SQLite Database (*.db)|*.db|All Files (*.*)|*.*",
                FileName = "AuroraDB.db"
            };

            if (dlg.ShowDialog() == true)
            {
                LoadDatabasePath(dlg.FileName);
                MessageBox.Show($"✅ Base de datos reconectada con éxito:\n{dlg.FileName}", "Conexión BD Actualizada", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CmbThemeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbThemeSelector.SelectedItem is ThemeOption theme)
            {
                ThemeManager.ApplyTheme(theme);
            }
        }

        private void CmbGlobalEmpire_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source != CmbGlobalEmpire) return;
            RefreshActiveTab();
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // CRITICAL FIX: Prevent inner ComboBox/DataGrid SelectionChanged bubbling up to TabControl!
            if (e.Source != MainTabControl) return;
            RefreshActiveTab();
        }

        private void RefreshActiveTab()
        {
            if (CmbGlobalEmpire?.SelectedItem is not Empire emp || _dbService == null) return;

            int raceId = emp.RaceID;

            // Update Global Game Time & Start Year indicators
            var gameTime = _dbService.GetGameTimeInfo(raceId);
            if (TxtHeaderGameDate != null) TxtHeaderGameDate.Text = gameTime.FormattedCurrentDate;
            if (TxtHeaderStartYear != null) TxtHeaderStartYear.Text = gameTime.FormattedStartYear;

            if (TabAI != null && emp != null && !string.IsNullOrEmpty(emp.RaceName))
            {
                TabAI.ToolTip = $"Matriz Computacional de Inteligencia Táctica conectada al Imperio {emp.RaceName}";
            }

            if (MainTabControl.SelectedItem == TabBlueprint && ViewBlueprint != null && emp != null)
            {
                ViewBlueprint.SetSelectedEmpire(emp);
            }
            else if (MainTabControl.SelectedItem == TabFleet && ViewFleet != null && ViewBlueprint != null)
            {
                ViewBlueprint.Recalculate();
                ViewFleet.SetActiveBlueprint(ViewBlueprint.CurrentDesign);
                ViewFleet.LoadEmpireClasses(_dbService, raceId);
            }
            else if (MainTabControl.SelectedItem == TabEmpire && ViewEmpire != null)
            {
                ViewEmpire.LoadEmpireData(_dbService, raceId);
            }
            else if (MainTabControl.SelectedItem == TabResearch && ViewResearch != null)
            {
                ViewResearch.LoadResearchData(_dbService, raceId);
            }
            else if (MainTabControl.SelectedItem == TabActiveFleets && ViewActiveFleets != null)
            {
                ViewActiveFleets.LoadFleetsData(_dbService, raceId);
            }
            else if (MainTabControl.SelectedItem == TabColonies && ViewColonies != null)
            {
                ViewColonies.LoadColoniesData(_dbService, raceId);
            }
            else if (MainTabControl.SelectedItem == TabShipyards && ViewShipyards != null)
            {
                ViewShipyards.LoadShipyardsData(_dbService, raceId);
            }
            else if (MainTabControl.SelectedItem == TabCommanders && ViewCommanders != null)
            {
                ViewCommanders.LoadCommandersData(_dbService, raceId);
            }
            else if (MainTabControl.SelectedItem == TabIndustrial && ViewIndustrial != null)
            {
                ViewIndustrial.LoadData(_dbService, raceId);
            }
            else if (MainTabControl.SelectedItem == TabExploration && ViewExploration != null)
            {
                ViewExploration.LoadData(_dbService, raceId);
            }
            else if (MainTabControl.SelectedItem == TabMissile && ViewMissile != null)
            {
                ViewMissile.LoadData(_dbService, raceId);
            }
            else if (MainTabControl.SelectedItem == TabAI && ViewAI != null)
            {
                ViewAI.LoadData(_dbService, raceId);
            }
        }

        private void BtnNavGobierno_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedItem = TabEmpire;
        }

        private void BtnNavCiencia_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedItem = TabResearch;
        }

        private void BtnNavIngenieria_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedItem = TabBlueprint;
        }

        private void BtnNavNaval_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedItem = TabActiveFleets;
        }

        private void BtnNavIA_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedItem = TabAI;
        }

        private void BtnFocusGame_Click(object sender, RoutedEventArgs e)
        {
            if (!WindowBridge.FocusAuroraGame(out string status))
            {
                MessageBox.Show(status, "Enfoque de Aurora 4X", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private TimeEventsWidgetWindow? _timeEventsWidgetWindow;

        private void BtnOpenTimeEventsWidget_Click(object sender, RoutedEventArgs e)
        {
            if (_dbService == null) return;
            int raceId = (CmbGlobalEmpire.SelectedItem as Empire)?.RaceID ?? 784;

            if (_timeEventsWidgetWindow == null || !_timeEventsWidgetWindow.IsLoaded)
            {
                _timeEventsWidgetWindow = new TimeEventsWidgetWindow();
            }

            _timeEventsWidgetWindow.InitializeWidget(_dbService, raceId);
            _timeEventsWidgetWindow.Show();
            _timeEventsWidgetWindow.Activate();
        }
    }
}
