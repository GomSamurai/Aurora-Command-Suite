using System;
using System.Media;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace AuroraDesignSuite.Views
{
    public enum AlertType
    {
        Critical,
        Advice,
        Achievement,
        Report
    }

    public partial class AITacticalOverlayWindow : Window
    {
        private DispatcherTimer? _dismissTimer;
        private double _totalSeconds = 10.0;
        private double _elapsedSeconds = 0.0;

        public AITacticalOverlayWindow()
        {
            InitializeComponent();
        }

        public void ShowAlert(string message, AlertType type, double autoDismissSeconds = 10.0, bool playSound = true)
        {
            _totalSeconds = autoDismissSeconds;
            _elapsedSeconds = 0.0;
            TxtMessage.Text = message;
            TxtHeaderTime.Text = DateTime.Now.ToString("HH:mm:ss");

            Color mainColor;
            Color bgOverlay;

            switch (type)
            {
                case AlertType.Critical:
                    mainColor = (Color)ColorConverter.ConvertFromString("#FF4444");
                    bgOverlay = (Color)ColorConverter.ConvertFromString("#F51B0D0D");
                    TxtCategoryBadge.Text = "🚨 ALERTA CRÍTICA IMPERIAL";
                    TxtIcon.Text = "🚨";
                    break;

                case AlertType.Achievement:
                    mainColor = (Color)ColorConverter.ConvertFromString("#FFD700");
                    bgOverlay = (Color)ColorConverter.ConvertFromString("#F51A180E");
                    TxtCategoryBadge.Text = "🏆 HITO & LOGRO IMPERIAL";
                    TxtIcon.Text = "🏆";
                    break;

                case AlertType.Report:
                    mainColor = (Color)ColorConverter.ConvertFromString("#00FF88");
                    bgOverlay = (Color)ColorConverter.ConvertFromString("#F50D2818");
                    TxtCategoryBadge.Text = "📊 INFORME ECONÓMICO IMPERIAL";
                    TxtIcon.Text = "📊";
                    break;

                case AlertType.Advice:
                default:
                    mainColor = (Color)ColorConverter.ConvertFromString("#00F0FF");
                    bgOverlay = (Color)ColorConverter.ConvertFromString("#F50B0E14");
                    TxtCategoryBadge.Text = "💡 CONSEJO TÁCTICO IA";
                    TxtIcon.Text = "💡";
                    break;
            }

            var brush = new SolidColorBrush(mainColor);
            MainBorder.BorderBrush = brush;
            MainBorder.Background = new SolidColorBrush(bgOverlay);
            BadgeBorder.BorderBrush = brush;
            BadgeBorder.Background = new SolidColorBrush(Color.FromArgb(40, mainColor.R, mainColor.G, mainColor.B));
            TxtCategoryBadge.Foreground = brush;
            PrgCountdown.Foreground = brush;

            // Position Overlay Top-Right
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            this.Left = screenWidth - this.Width - 25;
            this.Top = 45;

            this.Show();

            if (playSound)
            {
                try { SystemSounds.Asterisk.Play(); } catch { }
            }

            if (_totalSeconds > 0)
            {
                PrgCountdown.Value = 100;
                _dismissTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(100)
                };
                _dismissTimer.Tick += DismissTimer_Tick;
                _dismissTimer.Start();
            }
            else
            {
                PrgCountdown.Visibility = Visibility.Collapsed;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Ensure Window stays on top
            this.Topmost = true;
        }

        private void DismissTimer_Tick(object? sender, EventArgs e)
        {
            _elapsedSeconds += 0.1;
            double remainingPct = Math.Max(0, (1.0 - (_elapsedSeconds / _totalSeconds)) * 100.0);
            PrgCountdown.Value = remainingPct;

            if (_elapsedSeconds >= _totalSeconds)
            {
                _dismissTimer?.Stop();
                this.Close();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            _dismissTimer?.Stop();
            this.Close();
        }
    }
}
