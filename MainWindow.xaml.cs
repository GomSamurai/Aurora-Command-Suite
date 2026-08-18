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

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string currentDir = Directory.GetCurrentDirectory();

            string[] candidatePaths = new[]
            {
                dbArg ?? "",
                // 1. Search local/parent directory relative to where Portable App is pasted (HIGHEST PRIORITY)
                Path.GetFullPath(Path.Combine(baseDir, "..", "..", "AuroraDB.db")),
                Path.GetFullPath(Path.Combine(baseDir, "..", "AuroraDB.db")),
                Path.GetFullPath(Path.Combine(baseDir, "AuroraDB.db")),
                Path.GetFullPath(Path.Combine(currentDir, "..", "..", "AuroraDB.db")),
                Path.GetFullPath(Path.Combine(currentDir, "..", "AuroraDB.db")),
                Path.GetFullPath(Path.Combine(currentDir, "AuroraDB.db")),
                // 2. Saved preference path
                prefs.LastDbPath,
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
                var prefs = UserPreferencesService.LoadPreferences();
                prefs.WindowWidth = Width;
                prefs.WindowHeight = Height;
                prefs.IsMaximized = WindowState == WindowState.Maximized;
                prefs.SelectedTheme = (CmbThemeSelector.SelectedItem as ThemeOption)?.Name ?? "Cyberpunk Obsidian";
                prefs.SelectedEmpireId = (CmbGlobalEmpire.SelectedItem as Empire)?.RaceID ?? -1;
                if (!string.IsNullOrEmpty(_dbService?.DbPath))
                {
                    prefs.LastDbPath = _dbService.DbPath;
                }
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

            dbPath = Path.GetFullPath(dbPath);
            _dbService = new DatabaseService(dbPath);
            if (_dbService.TestConnection(out _))
            {
                var empires = _dbService.GetEmpires();
                CmbGlobalEmpire.ItemsSource = empires;
                if (empires.Count > 0)
                {
                    var prefs = UserPreferencesService.LoadPreferences();
                    var activeEmpire = empires.FirstOrDefault(x => x.RaceID == prefs.SelectedEmpireId) 
                                      ?? empires.OrderByDescending(x => x.GameID).ThenByDescending(x => x.RaceID).FirstOrDefault();
                    CmbGlobalEmpire.SelectedItem = activeEmpire ?? empires[0];

                    prefs.LastDbPath = dbPath;
                    UserPreferencesService.SavePreferences(prefs);
                }

                if (BtnChangeDb != null)
                {
                    BtnChangeDb.ToolTip = $"📁 BD Activa:\n{dbPath}\n\nHaz clic para seleccionar otra BD.";
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
                    Interval = TimeSpan.FromMilliseconds(1000)
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

                var writeTime = File.GetLastWriteTimeUtc(dbPath);
                if (writeTime != _lastDbWriteTime)
                {
                    _lastDbWriteTime = writeTime;
                    Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
                    RefreshActiveTab();
                }
            }
            catch { }
        }

        private void HandleLiveSyncEvent(string action)
        {
            Dispatcher.Invoke(() =>
            {
                Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
                RefreshActiveTab();
            });
        }

        private void TriggerLiveRefresh()
        {
            Dispatcher.InvokeAsync(() =>
            {
                Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
                RefreshActiveTab();
            }, System.Windows.Threading.DispatcherPriority.Background);
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

            // Update Global Telemetry Header metrics
            var gameTime = _dbService.GetGameTimeInfo(raceId);
            if (TxtHeaderGameDate != null) TxtHeaderGameDate.Text = gameTime.FormattedCurrentDate;
            if (TxtHeaderStartYear != null) TxtHeaderStartYear.Text = gameTime.FormattedStartYear;

            double popM = _dbService.GetTotalEmpirePopulation(raceId);
            if (TxtHeaderEmpirePop != null) TxtHeaderEmpirePop.Text = $"{popM:N1} M habs";

            int colonies = _dbService.GetEmpireColonyCount(raceId);
            if (TxtHeaderColoniesCount != null) TxtHeaderColoniesCount.Text = $"{colonies} {(colonies == 1 ? "Colonia" : "Colonias")}";

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
