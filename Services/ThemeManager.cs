using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace AuroraDesignSuite.Services
{
    public class ThemeOption
    {
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = "🎨";
        public string BgDark { get; set; } = "#0B0E14";
        public string CardBg { get; set; } = "#131924";
        public string CardHeader { get; set; } = "#1B2333";
        public string AccentCyan { get; set; } = "#00F0FF";
        public string AccentAmber { get; set; } = "#FFB700";
        public string TextPrimary { get; set; } = "#E6EDF3";
        public string TextSecondary { get; set; } = "#8B949E";
        public string BorderColor { get; set; } = "#30363D";

        public override string ToString() => $"{Icon} {Name}";
    }

    public static class ThemeManager
    {
        public static List<ThemeOption> AvailableThemes { get; } = new List<ThemeOption>
        {
            new ThemeOption
            {
                Name = "👑 Imperial Gold (Fran Gómez Edition)",
                Icon = "👑",
                BgDark = "#090C15",
                CardBg = "#121828",
                CardHeader = "#1C243C",
                AccentCyan = "#00F0FF",
                AccentAmber = "#FFD700",
                TextPrimary = "#FFFFFF",
                TextSecondary = "#9DA8C0",
                BorderColor = "#2A385C"
            },
            new ThemeOption
            {
                Name = "Cyber Neon Cyan (Default)",
                Icon = "🌌",
                BgDark = "#0B0E14",
                CardBg = "#131924",
                CardHeader = "#1B2333",
                AccentCyan = "#00F0FF",
                AccentAmber = "#FFB700",
                TextPrimary = "#E6EDF3",
                TextSecondary = "#8B949E",
                BorderColor = "#30363D"
            },
            new ThemeOption
            {
                Name = "Deep Void Gold (Imperial Elite)",
                Icon = "👑",
                BgDark = "#0D0B07",
                CardBg = "#1C160C",
                CardHeader = "#292012",
                AccentCyan = "#FFD700",
                AccentAmber = "#FF8C00",
                TextPrimary = "#F5E6CC",
                TextSecondary = "#A69273",
                BorderColor = "#524022"
            },
            new ThemeOption
            {
                Name = "Obsidian Emerald (Tactical Ops)",
                Icon = "🛡️",
                BgDark = "#070D09",
                CardBg = "#0E1C14",
                CardHeader = "#152E20",
                AccentCyan = "#00FF88",
                AccentAmber = "#00E5FF",
                TextPrimary = "#E0F2E9",
                TextSecondary = "#7EA691",
                BorderColor = "#224F35"
            },
            new ThemeOption
            {
                Name = "Royal Nebula (Star Fleet)",
                Icon = "⚡",
                BgDark = "#0D0914",
                CardBg = "#1A1226",
                CardHeader = "#2A1D3D",
                AccentCyan = "#BF5AF2",
                AccentAmber = "#FF2D55",
                TextPrimary = "#F2E6FF",
                TextSecondary = "#9683B5",
                BorderColor = "#4A3273"
            },
            new ThemeOption
            {
                Name = "Solar Flare Amber (Deep Space)",
                Icon = "☀️",
                BgDark = "#140A07",
                CardBg = "#24120D",
                CardHeader = "#381B13",
                AccentCyan = "#FF9500",
                AccentAmber = "#FF3B30",
                TextPrimary = "#FFEBE6",
                TextSecondary = "#A87C71",
                BorderColor = "#5E2E20"
            }
        };

        public static void ApplyTheme(ThemeOption theme)
        {
            if (theme == null || Application.Current == null) return;

            var res = Application.Current.Resources;

            SetResource(res, "BgDarkColor", "BgDarkBrush", theme.BgDark);
            SetResource(res, "CardBgColor", "CardBgBrush", theme.CardBg);
            SetResource(res, "CardHeaderColor", "CardHeaderBrush", theme.CardHeader);
            SetResource(res, "AccentCyanColor", "AccentCyanBrush", theme.AccentCyan);
            SetResource(res, "AccentAmberColor", "AccentAmberBrush", theme.AccentAmber);
            SetResource(res, "TextPrimaryColor", "TextPrimaryBrush", theme.TextPrimary);
            SetResource(res, "TextSecondaryColor", "TextSecondaryBrush", theme.TextSecondary);
            SetResource(res, "BorderColor", "BorderBrush", theme.BorderColor);

            if (Application.Current.MainWindow is Window mainWin)
            {
                mainWin.Background = (Brush)res["BgDarkBrush"];
            }
        }

        private static void SetResource(ResourceDictionary res, string colorKey, string brushKey, string hexColor)
        {
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(hexColor);
                res[colorKey] = color;
                res[brushKey] = new SolidColorBrush(color);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SetResource Error: {ex.Message}");
            }
        }
    }
}
