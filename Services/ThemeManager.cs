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

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Name)) return string.Empty;
            return Name.StartsWith(Icon) ? Name : $"{Icon} {Name}";
        }
    }

    public static class ThemeManager
    {
        public static List<ThemeOption> AvailableThemes { get; } = new List<ThemeOption>
        {
            // === 🌌 MODOS OSCURO / DEEP SPACE ===
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
                Name = "🌌 Cyber Neon Cyan (Default)",
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
                Name = "🛡️ Obsidian Emerald (Tactical Ops)",
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
                Name = "⚡ Royal Nebula (Star Fleet)",
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
                Name = "☀️ Solar Flare Amber (Deep Space)",
                Icon = "☀️",
                BgDark = "#140A07",
                CardBg = "#24120D",
                CardHeader = "#381B13",
                AccentCyan = "#FF9500",
                AccentAmber = "#FF3B30",
                TextPrimary = "#FFEBE6",
                TextSecondary = "#A87C71",
                BorderColor = "#5E2E20"
            },
            new ThemeOption
            {
                Name = "🪐 Saturnian Titanium (Deep Core)",
                Icon = "🪐",
                BgDark = "#1A202C",
                CardBg = "#2D3748",
                CardHeader = "#3A4A63",
                AccentCyan = "#63B3ED",
                AccentAmber = "#F6AD55",
                TextPrimary = "#EDF2F7",
                TextSecondary = "#A0AEC0",
                BorderColor = "#4A5568"
            },
            new ThemeOption
            {
                Name = "🔴 Mars Command Crimson (Red Planet)",
                Icon = "🔴",
                BgDark = "#170C0D",
                CardBg = "#2B1618",
                CardHeader = "#3F1F23",
                AccentCyan = "#FC8181",
                AccentAmber = "#F6AD55",
                TextPrimary = "#FFF5F5",
                TextSecondary = "#FEB2B2",
                BorderColor = "#63171B"
            },
            new ThemeOption
            {
                Name = "🟢 Andromeda Biopunk (Alien Tech)",
                Icon = "🟢",
                BgDark = "#0A120E",
                CardBg = "#14241C",
                CardHeader = "#1E362A",
                AccentCyan = "#68D391",
                AccentAmber = "#4FD1C5",
                TextPrimary = "#F0FFF4",
                TextSecondary = "#9AE6B4",
                BorderColor = "#276749"
            },

            // === ☀️ MODOS CLARO / DIA (Light & High Legibility Modes) ===
            new ThemeOption
            {
                Name = "🏛️ Imperial Marble (Light Classic)",
                Icon = "🏛️",
                BgDark = "#F0F4F8",
                CardBg = "#FFFFFF",
                CardHeader = "#E2E8F0",
                AccentCyan = "#1A365D",
                AccentAmber = "#B7791F",
                TextPrimary = "#0F172A",
                TextSecondary = "#475569",
                BorderColor = "#CBD5E1"
            },
            new ThemeOption
            {
                Name = "❄️ Solar Ice (Polar Light)",
                Icon = "❄️",
                BgDark = "#EBF8FF",
                CardBg = "#FFFFFF",
                CardHeader = "#BEE3F8",
                AccentCyan = "#007791",
                AccentAmber = "#DD6B20",
                TextPrimary = "#1A202C",
                TextSecondary = "#4A5568",
                BorderColor = "#90CDF4"
            },
            new ThemeOption
            {
                Name = "📜 Ancient Codex (Soft Warm Parchment)",
                Icon = "📜",
                BgDark = "#FDF6E3",
                CardBg = "#EEE8D5",
                CardHeader = "#E0D7C3",
                AccentCyan = "#B58900",
                AccentAmber = "#CB4B16",
                TextPrimary = "#073642",
                TextSecondary = "#586E75",
                BorderColor = "#D3C6AA"
            },

            // === ☯️ MODOS NEUTROS Y EQUILIBRADOS (Neutral Slate & Soft Dark) ===
            new ThemeOption
            {
                Name = "⚙️ Slate Protocol (Balanced Industrial)",
                Icon = "⚙️",
                BgDark = "#181A1B",
                CardBg = "#222527",
                CardHeader = "#2D3135",
                AccentCyan = "#38BDF8",
                AccentAmber = "#FBBF24",
                TextPrimary = "#F3F4F6",
                TextSecondary = "#9CA3AF",
                BorderColor = "#374151"
            },
            new ThemeOption
            {
                Name = "🌲 Nordic Pine (Sage & Evergreen)",
                Icon = "🌲",
                BgDark = "#121A17",
                CardBg = "#1C2924",
                CardHeader = "#293B34",
                AccentCyan = "#34D399",
                AccentAmber = "#F59E0B",
                TextPrimary = "#ECFDF5",
                TextSecondary = "#A7F3D0",
                BorderColor = "#059669"
            },
            new ThemeOption
            {
                Name = "☕ Espresso Command (Warm Dark Coffee)",
                Icon = "☕",
                BgDark = "#181412",
                CardBg = "#26201D",
                CardHeader = "#362E2A",
                AccentCyan = "#FB923C",
                AccentAmber = "#FACC15",
                TextPrimary = "#FFF7ED",
                TextSecondary = "#FDBA74",
                BorderColor = "#7C2D12"
            },
            new ThemeOption
            {
                Name = "🌃 Tokyo Night Synth (Cyberpunk Synthwave)",
                Icon = "🌃",
                BgDark = "#1A1B26",
                CardBg = "#24283B",
                CardHeader = "#343B58",
                AccentCyan = "#7AA2F7",
                AccentAmber = "#F7768E",
                TextPrimary = "#C0CAF5",
                TextSecondary = "#A9B1D6",
                BorderColor = "#414868"
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
