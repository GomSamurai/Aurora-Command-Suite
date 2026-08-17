using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using AuroraDesignSuite.Models;
using AuroraDesignSuite.Services;

namespace AuroraDesignSuite.Views
{
    public partial class TimeEventsWidgetWindow : Window
    {
        private DatabaseService? _dbService;
        private int _currentRaceId = 784;
        
        private readonly DispatcherTimer _autoPassTimer;
        private readonly DispatcherTimer _eventPollTimer;

        private bool _isAutoPassRunning = false;
        private bool _isCompactMode = false;

        private double _selectedTimeStep = 86400; // Default 1 Day
        private double _autoPassIntervalSeconds = 1.0;

        public TimeEventsWidgetWindow()
        {
            InitializeComponent();

            _autoPassTimer = new DispatcherTimer();
            _autoPassTimer.Tick += AutoPassTimer_Tick;

            _eventPollTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            _eventPollTimer.Tick += (s, e) => LoadEvents();
            _eventPollTimer.Start();

            LiveSyncBridge.OnGameSyncReceived += HandleLiveSync;
        }

        public void InitializeWidget(DatabaseService dbService, int raceId)
        {
            _dbService = dbService;
            _currentRaceId = raceId;

            RefreshCalendar();
            LoadEvents();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        public void CloseWidget()
        {
            StopAutoPass();
            _eventPollTimer.Stop();
            LiveSyncBridge.OnGameSyncReceived -= HandleLiveSync;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            CloseWidget();
            base.OnClosing(e);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseWidget();
            Hide();
        }

        private void BtnPinTopmost_Click(object sender, RoutedEventArgs e)
        {
            Topmost = !Topmost;
            if (BtnPinTopmost != null)
            {
                if (Topmost)
                {
                    BtnPinTopmost.Content = "📌 TOP";
                    BtnPinTopmost.Foreground = (Brush)FindResource("AccentCyanBrush");
                    BtnPinTopmost.BorderBrush = (Brush)FindResource("AccentCyanBrush");
                }
                else
                {
                    BtnPinTopmost.Content = "📍 NORMAL";
                    BtnPinTopmost.Foreground = Brushes.Gray;
                    BtnPinTopmost.BorderBrush = Brushes.Gray;
                }
            }
        }

        private void BtnToggleCompact_Click(object sender, RoutedEventArgs e)
        {
            _isCompactMode = !_isCompactMode;
            if (_isCompactMode)
            {
                Height = 175;
                if (PnlEventsFilterHeader != null) PnlEventsFilterHeader.Visibility = Visibility.Collapsed;
                if (PnlEventListBorder != null) PnlEventListBorder.Visibility = Visibility.Collapsed;
                if (BtnToggleCompact != null) BtnToggleCompact.Content = "🗖 EXTENDER";
            }
            else
            {
                Height = 550;
                if (PnlEventsFilterHeader != null) PnlEventsFilterHeader.Visibility = Visibility.Visible;
                if (PnlEventListBorder != null) PnlEventListBorder.Visibility = Visibility.Visible;
                if (BtnToggleCompact != null) BtnToggleCompact.Content = "🗖 COMPACTO";
            }
        }

        private void SldOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Opacity = e.NewValue;
            if (TxtOpacityValue != null)
            {
                TxtOpacityValue.Text = $"{(int)(e.NewValue * 100)}%";
            }
        }

        private void RefreshCalendar()
        {
            if (_dbService == null) return;
            var timeInfo = _dbService.GetGameTimeInfo(_currentRaceId);
            string raceName = _dbService.GetRaceName(_currentRaceId);

            if (TxtCurrentDate != null) TxtCurrentDate.Text = timeInfo.FormattedCurrentDate;
            if (TxtLifetime != null) TxtLifetime.Text = $"Inicio: {timeInfo.StartYear} ({timeInfo.YearsElapsed:F1} a. vida)";
            if (TxtEmpireName != null) TxtEmpireName.Text = raceName;
        }

        private void LoadEvents()
        {
            if (_dbService == null || LstGameEvents == null) return;

            string categoryFilter = "Todas";
            if (CmbEventCategoryFilter?.SelectedItem is ComboBoxItem cItem)
            {
                string text = cItem.Content.ToString() ?? "";
                if (text.Contains("Combate")) categoryFilter = "Combate";
                else if (text.Contains("Investigación")) categoryFilter = "Investigación e Industria";
                else if (text.Contains("Flota")) categoryFilter = "Flota y Oficiales";
                else if (text.Contains("Exploración")) categoryFilter = "Exploración";
            }

            var events = _dbService.GetRecentGameEvents(_currentRaceId, 100, categoryFilter);
            LstGameEvents.ItemsSource = events;

            if (TxtEventCount != null)
            {
                TxtEventCount.Text = $"({events.Count} eventos en registro)";
            }
        }

        private void BtnTimeStep_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tagStr && double.TryParse(tagStr, out double seconds))
            {
                ExecuteTimeStep(seconds);
            }
        }

        private void ExecuteTimeStep(double seconds)
        {
            if (_dbService == null) return;

            bool success = WindowBridge.SendTimeStepToGame(seconds, _dbService, _currentRaceId, out string statusMsg);

            RefreshCalendar();
            LoadEvents();

            var recentEvents = _dbService.GetRecentGameEvents(_currentRaceId, 1, "Todas");
            bool hasInterrupt = recentEvents.Count > 0 && (recentEvents[0].IsCombat || recentEvents[0].IsInterrupt);

            if (hasInterrupt && _isAutoPassRunning && ChkStopOnCombat?.IsChecked == true)
            {
                StopAutoPass();
                MessageBox.Show("🔴 El Modo Auto-Avanzar se ha detenido automáticamente debido a un evento de Alerta Roja o Combate en Aurora 4X.", "Interrupción de Seguridad", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnToggleAutoPass_Click(object sender, RoutedEventArgs e)
        {
            if (_isAutoPassRunning)
            {
                StopAutoPass();
            }
            else
            {
                StartAutoPass();
            }
        }

        private void StartAutoPass()
        {
            _isAutoPassRunning = true;
            _autoPassTimer.Interval = TimeSpan.FromSeconds(_autoPassIntervalSeconds);
            _autoPassTimer.Start();

            if (BtnToggleAutoPass != null)
            {
                BtnToggleAutoPass.Content = "⏸️ PAUSAR AUTO";
                BtnToggleAutoPass.Background = (Brush)FindResource("CardHeaderBrush");
                BtnToggleAutoPass.Foreground = (Brush)FindResource("AccentAmberBrush");
                BtnToggleAutoPass.BorderBrush = (Brush)FindResource("AccentAmberBrush");
            }
            if (TxtStatusPill != null)
            {
                TxtStatusPill.Text = "▶️ AUTO-AVANZANDO";
                TxtStatusPill.Foreground = (Brush)FindResource("AccentAmberBrush");
            }
        }

        private void StopAutoPass()
        {
            _isAutoPassRunning = false;
            _autoPassTimer.Stop();

            if (BtnToggleAutoPass != null)
            {
                BtnToggleAutoPass.Content = "▶️ AUTO-AVANZAR";
                BtnToggleAutoPass.Background = System.Windows.Media.Brushes.DarkGreen;
                BtnToggleAutoPass.Foreground = System.Windows.Media.Brushes.Lime;
                BtnToggleAutoPass.BorderBrush = System.Windows.Media.Brushes.Lime;
            }
            if (TxtStatusPill != null)
            {
                TxtStatusPill.Text = "🟢 EN TIEMPO REAL";
                TxtStatusPill.Foreground = (Brush)FindResource("AccentGreenBrush");
            }
        }

        private void AutoPassTimer_Tick(object? sender, EventArgs e)
        {
            ExecuteTimeStep(_selectedTimeStep);
        }

        private void CmbAutoSpeed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbAutoSpeed?.SelectedItem is ComboBoxItem item)
            {
                int index = CmbAutoSpeed.SelectedIndex;
                _autoPassIntervalSeconds = index switch
                {
                    0 => 0.5,
                    1 => 1.0,
                    2 => 2.0,
                    3 => 5.0,
                    _ => 1.0
                };

                if (_isAutoPassRunning)
                {
                    _autoPassTimer.Interval = TimeSpan.FromSeconds(_autoPassIntervalSeconds);
                }
            }
        }

        private void CmbAutoStep_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbAutoStep?.SelectedItem is ComboBoxItem item)
            {
                int index = CmbAutoStep.SelectedIndex;
                _selectedTimeStep = index switch
                {
                    0 => 5,       // +5 Seg
                    1 => 3600,    // +1 Hora
                    2 => 86400,   // +1 Día
                    3 => 432000,  // +5 Días
                    4 => 2592000, // +30 Días
                    _ => 86400
                };
            }
        }

        private void CmbEventCategoryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadEvents();
        }

        private void BtnRefreshEvents_Click(object sender, RoutedEventArgs e)
        {
            RefreshCalendar();
            LoadEvents();
        }

        private void HandleLiveSync(string topic)
        {
            Dispatcher.Invoke(() =>
            {
                RefreshCalendar();
                LoadEvents();
            });
        }
    }
}
