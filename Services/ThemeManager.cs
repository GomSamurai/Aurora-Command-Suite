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
        public string Category { get; set; } = "🌌 Oscuro";
        public bool IsHeader { get; set; } = false;
        public bool IsCustom { get; set; } = false;
        public bool IsEditorAction { get; set; } = false;

        public string BgDark { get; set; } = "#0B0E14";
        public string CardBg { get; set; } = "#131924";
        public string CardHeader { get; set; } = "#1B2333";
        
        public string TextPrimary { get; set; } = "#E6EDF3";
        public string TextSecondary { get; set; } = "#8B949E";
        
        public string AccentCyan { get; set; } = "#00F0FF";
        public string AccentAmber { get; set; } = "#FFB700";
        public string AccentGold { get; set; } = "#FFD700";
        public string AccentGreen { get; set; } = "#00FF88";
        public string AccentRed { get; set; } = "#FF5555";
        public string AccentPurple { get; set; } = "#BF5AF2";
        public string BorderColor { get; set; } = "#30363D";

        public override string ToString()
        {
            if (IsHeader) return Name;
            if (string.IsNullOrEmpty(Name)) return string.Empty;
            return Name.StartsWith(Icon) ? Name : $"{Icon} {Name}";
        }
    }

    public static class ThemeManager
    {
        public static List<ThemeOption> AvailableThemes { get; } = new List<ThemeOption>
        {
            // ==========================================
            // 👑 EDICIÓN INSIGNIA
            // ==========================================
            new ThemeOption { Name = "─── 👑 EDICIÓN INSIGNIA ───", IsHeader = true },
            new ThemeOption
            {
                Category = "👑 Insignia",
                Name = "👑 Imperial Gold (Fran Gómez Edition)",
                Icon = "👑",
                BgDark = "#090C15",
                CardBg = "#121828",
                CardHeader = "#1C243C",
                TextPrimary = "#FFFFFF",
                TextSecondary = "#9DA8C0",
                AccentCyan = "#00F0FF",
                AccentAmber = "#FFD700",
                AccentGold = "#FFD700",
                AccentGreen = "#00FF88",
                AccentRed = "#FF4444",
                AccentPurple = "#BF5AF2",
                BorderColor = "#2A385C"
            },

            // ==========================================
            // 🌌 CATEGORÍA: MODOS OSCURO / DEEP SPACE
            // ==========================================
            new ThemeOption { Name = "─── 🌌 MODOS OSCURO (DEEP SPACE) ───", IsHeader = true },
            new ThemeOption
            {
                Category = "🌌 Modos Oscuro",
                Name = "🌌 Cyber Neon Cyan (Default)",
                Icon = "🌌",
                BgDark = "#0B0E14",
                CardBg = "#131924",
                CardHeader = "#1B2333",
                TextPrimary = "#E6EDF3",
                TextSecondary = "#8B949E",
                AccentCyan = "#00F0FF",
                AccentAmber = "#FFB700",
                AccentGold = "#FFD700",
                AccentGreen = "#00FF88",
                AccentRed = "#FF5555",
                AccentPurple = "#AF52DE",
                BorderColor = "#30363D"
            },
            new ThemeOption
            {
                Category = "🌌 Modos Oscuro",
                Name = "🛡️ Obsidian Emerald (Tactical Ops)",
                Icon = "🛡️",
                BgDark = "#070D09",
                CardBg = "#0E1C14",
                CardHeader = "#152E20",
                TextPrimary = "#E0F2E9",
                TextSecondary = "#7EA691",
                AccentCyan = "#00FF88",
                AccentAmber = "#00E5FF",
                AccentGold = "#FFD700",
                AccentGreen = "#55FF55",
                AccentRed = "#FF3B30",
                AccentPurple = "#AF52DE",
                BorderColor = "#224F35"
            },
            new ThemeOption
            {
                Category = "🌌 Modos Oscuro",
                Name = "⚡ Royal Nebula (Star Fleet)",
                Icon = "⚡",
                BgDark = "#0D0914",
                CardBg = "#1A1226",
                CardHeader = "#2A1D3D",
                TextPrimary = "#F2E6FF",
                TextSecondary = "#9683B5",
                AccentCyan = "#BF5AF2",
                AccentAmber = "#FF2D55",
                AccentGold = "#FFD700",
                AccentGreen = "#00FF88",
                AccentRed = "#FF3B30",
                AccentPurple = "#D946EF",
                BorderColor = "#4A3273"
            },
            new ThemeOption
            {
                Category = "🌌 Modos Oscuro",
                Name = "☀️ Solar Flare Amber (Deep Space)",
                Icon = "☀️",
                BgDark = "#140A07",
                CardBg = "#24120D",
                CardHeader = "#381B13",
                TextPrimary = "#FFEBE6",
                TextSecondary = "#A87C71",
                AccentCyan = "#FF9500",
                AccentAmber = "#FF3B30",
                AccentGold = "#FFD700",
                AccentGreen = "#34D399",
                AccentRed = "#EF4444",
                AccentPurple = "#C084FC",
                BorderColor = "#5E2E20"
            },
            new ThemeOption
            {
                Category = "🌌 Modos Oscuro",
                Name = "🪐 Saturnian Titanium (Deep Core)",
                Icon = "🪐",
                BgDark = "#1A202C",
                CardBg = "#2D3748",
                CardHeader = "#3A4A63",
                TextPrimary = "#EDF2F7",
                TextSecondary = "#A0AEC0",
                AccentCyan = "#63B3ED",
                AccentAmber = "#F6AD55",
                AccentGold = "#FFD700",
                AccentGreen = "#48BB78",
                AccentRed = "#F56565",
                AccentPurple = "#9F7AEA",
                BorderColor = "#4A5568"
            },
            new ThemeOption
            {
                Category = "🌌 Modos Oscuro",
                Name = "🔴 Mars Command Crimson (Red Planet)",
                Icon = "🔴",
                BgDark = "#170C0D",
                CardBg = "#2B1618",
                CardHeader = "#3F1F23",
                TextPrimary = "#FFF5F5",
                TextSecondary = "#FEB2B2",
                AccentCyan = "#FC8181",
                AccentAmber = "#F6AD55",
                AccentGold = "#FFD700",
                AccentGreen = "#68D391",
                AccentRed = "#E53E3E",
                AccentPurple = "#B794F4",
                BorderColor = "#63171B"
            },
            new ThemeOption
            {
                Category = "🌌 Modos Oscuro",
                Name = "🟢 Andromeda Biopunk (Alien Tech)",
                Icon = "🟢",
                BgDark = "#0A120E",
                CardBg = "#14241C",
                CardHeader = "#1E362A",
                TextPrimary = "#F0FFF4",
                TextSecondary = "#9AE6B4",
                AccentCyan = "#68D391",
                AccentAmber = "#4FD1C5",
                AccentGold = "#FFD700",
                AccentGreen = "#38A169",
                AccentRed = "#E53E3E",
                AccentPurple = "#B794F4",
                BorderColor = "#276749"
            },

            // ==========================================
            // ☀️ CATEGORÍA: MODOS CLARO / DÍA (Alta Legibilidad)
            // ==========================================
            new ThemeOption { Name = "─── ☀️ MODOS CLARO / DÍA (ALTA LEGIBILIDAD) ───", IsHeader = true },
            new ThemeOption
            {
                Category = "☀️ Modos Claro",
                Name = "🏛️ Imperial Marble (Light Sapphire & Gold)",
                Icon = "🏛️",
                BgDark = "#F1F5F9",
                CardBg = "#FFFFFF",
                CardHeader = "#E2E8F0",
                TextPrimary = "#0F172A",
                TextSecondary = "#475569",
                AccentCyan = "#1E3A8A",
                AccentAmber = "#B45309",
                AccentGold = "#B45309",
                AccentGreen = "#15803D",
                AccentRed = "#B91C1C",
                AccentPurple = "#6B21A8",
                BorderColor = "#CBD5E1"
            },
            new ThemeOption
            {
                Category = "☀️ Modos Claro",
                Name = "❄️ Polar Frost (Light Ocean Blue)",
                Icon = "❄️",
                BgDark = "#EBF8FF",
                CardBg = "#FFFFFF",
                CardHeader = "#BEE3F8",
                TextPrimary = "#1A202C",
                TextSecondary = "#4A5568",
                AccentCyan = "#0284C7",
                AccentAmber = "#C2410C",
                AccentGold = "#C2410C",
                AccentGreen = "#166534",
                AccentRed = "#C53030",
                AccentPurple = "#7E22CE",
                BorderColor = "#90CDF4"
            },
            new ThemeOption
            {
                Category = "☀️ Modos Claro",
                Name = "📜 Ancient Parchment (Warm Soft Eye-Care)",
                Icon = "📜",
                BgDark = "#FDF6E3",
                CardBg = "#FAF4E1",
                CardHeader = "#EEE8D5",
                TextPrimary = "#073642",
                TextSecondary = "#586E75",
                AccentCyan = "#B58900",
                AccentAmber = "#CB4B16",
                AccentGold = "#B58900",
                AccentGreen = "#2AA198",
                AccentRed = "#DC322F",
                AccentPurple = "#6C71C4",
                BorderColor = "#D3C6AA"
            },
            new ThemeOption
            {
                Category = "☀️ Modos Claro",
                Name = "📄 Clean White Paper (Minimalist Office)",
                Icon = "📄",
                BgDark = "#F8FAFC",
                CardBg = "#FFFFFF",
                CardHeader = "#F1F5F9",
                TextPrimary = "#020617",
                TextSecondary = "#334155",
                AccentCyan = "#0369A1",
                AccentAmber = "#D97706",
                AccentGold = "#D97706",
                AccentGreen = "#047857",
                AccentRed = "#BE123C",
                AccentPurple = "#581C87",
                BorderColor = "#E2E8F0"
            },
            new ThemeOption
            {
                Category = "☀️ Modos Claro",
                Name = "🌅 Solar Daylight (Warm Golden Light)",
                Icon = "🌅",
                BgDark = "#FFFBEB",
                CardBg = "#FFFFFF",
                CardHeader = "#FEF3C7",
                TextPrimary = "#451A03",
                TextSecondary = "#78350F",
                AccentCyan = "#0369A1",
                AccentAmber = "#B45309",
                AccentGold = "#B45309",
                AccentGreen = "#15803D",
                AccentRed = "#B91C1C",
                AccentPurple = "#6B21A8",
                BorderColor = "#FDE68A"
            },

            // ==========================================
            // ☯️ CATEGORÍA: MODOS NEUTROS Y SLATE
            // ==========================================
            new ThemeOption { Name = "─── ☯️ MODOS NEUTROS Y SLATE ───", IsHeader = true },
            new ThemeOption
            {
                Category = "☯️ Modos Neutros",
                Name = "⚙️ Slate Industrial (Cool Gray)",
                Icon = "⚙️",
                BgDark = "#181A1B",
                CardBg = "#222527",
                CardHeader = "#2D3135",
                TextPrimary = "#F3F4F6",
                TextSecondary = "#9CA3AF",
                AccentCyan = "#38BDF8",
                AccentAmber = "#FBBF24",
                AccentGold = "#FFD700",
                AccentGreen = "#34D399",
                AccentRed = "#F87171",
                AccentPurple = "#C084FC",
                BorderColor = "#374151"
            },
            new ThemeOption
            {
                Category = "☯️ Modos Neutros",
                Name = "🌲 Nordic Forest (Sage Pine)",
                Icon = "🌲",
                BgDark = "#121A17",
                CardBg = "#1C2924",
                CardHeader = "#293B34",
                TextPrimary = "#ECFDF5",
                TextSecondary = "#A7F3D0",
                AccentCyan = "#34D399",
                AccentAmber = "#F59E0B",
                AccentGold = "#FFD700",
                AccentGreen = "#10B981",
                AccentRed = "#EF4444",
                AccentPurple = "#A78BFA",
                BorderColor = "#059669"
            },
            new ThemeOption
            {
                Category = "☯️ Modos Neutros",
                Name = "☕ Espresso Roast (Warm Dark Coffee)",
                Icon = "☕",
                BgDark = "#181412",
                CardBg = "#26201D",
                CardHeader = "#362E2A",
                TextPrimary = "#FFF7ED",
                TextSecondary = "#FDBA74",
                AccentCyan = "#FB923C",
                AccentAmber = "#FACC15",
                AccentGold = "#FFD700",
                AccentGreen = "#4ADE80",
                AccentRed = "#F87171",
                AccentPurple = "#E879F9",
                BorderColor = "#7C2D12"
            },
            new ThemeOption
            {
                Category = "☯️ Modos Neutros",
                Name = "🌃 Tokyo Night Synth (Cyberpunk)",
                Icon = "🌃",
                BgDark = "#1A1B26",
                CardBg = "#24283B",
                CardHeader = "#343B58",
                TextPrimary = "#C0CAF5",
                TextSecondary = "#A9B1D6",
                AccentCyan = "#7AA2F7",
                AccentAmber = "#F7768E",
                AccentGold = "#FFD700",
                AccentGreen = "#73DACA",
                AccentRed = "#F7768E",
                AccentPurple = "#BB9AF7",
                BorderColor = "#414868"
            },

            // ==========================================
            // 💎 CATEGORÍA: EDICIONES REFINADAS
            // ==========================================
            new ThemeOption { Name = "─── 💎 EDICIONES REFINADAS ───", IsHeader = true },
            new ThemeOption
            {
                Category = "💎 Ediciones Refinadas",
                Name = "🔮 Amethyst Crystal (Deep Violet)",
                Icon = "🔮",
                BgDark = "#13091E",
                CardBg = "#211132",
                CardHeader = "#311B49",
                TextPrimary = "#F5F3FF",
                TextSecondary = "#DDD6FE",
                AccentCyan = "#A78BFA",
                AccentAmber = "#F472B6",
                AccentGold = "#FFD700",
                AccentGreen = "#34D399",
                AccentRed = "#FB7185",
                AccentPurple = "#C084FC",
                BorderColor = "#5B21B6"
            },
            new ThemeOption
            {
                Category = "💎 Ediciones Refinadas",
                Name = "🌊 Oceanic Abyss (Deep Aquamarine)",
                Icon = "🌊",
                BgDark = "#06181C",
                CardBg = "#0D2930",
                CardHeader = "#153D47",
                TextPrimary = "#ECFEFF",
                TextSecondary = "#A5F3FC",
                AccentCyan = "#22D3EE",
                AccentAmber = "#FACC15",
                AccentGold = "#FFD700",
                AccentGreen = "#34D399",
                AccentRed = "#F87171",
                AccentPurple = "#C084FC",
                BorderColor = "#165B6E"
            },
            new ThemeOption
            {
                Category = "💎 Ediciones Refinadas",
                Name = "🖤 Pure Monochrome (Dark Minimalist)",
                Icon = "🖤",
                BgDark = "#09090B",
                CardBg = "#18181B",
                CardHeader = "#27272A",
                TextPrimary = "#FAFAFA",
                TextSecondary = "#A1A1AA",
                AccentCyan = "#E4E4E7",
                AccentAmber = "#D4D4D8",
                AccentGold = "#FFD700",
                AccentGreen = "#10B981",
                AccentRed = "#EF4444",
                AccentPurple = "#A855F7",
                BorderColor = "#3F3F46"
            }
        };

        static ThemeManager()
        {
            // Add Editor action item initially
            AvailableThemes.Add(new ThemeOption { Name = "─── ⚙️ EDITOR Y ESTUDIO ───", IsHeader = true, Category = "⚙️ Editor" });
            AvailableThemes.Add(new ThemeOption { Name = "🎨 ⚙️ CREAR / EDITAR TEMA PERSONALIZADO...", Icon = "⚙️", Category = "⚙️ Editor", IsEditorAction = true });
        }

        public static void RegisterCustomThemes(List<ThemeOption> customThemes)
        {
            AvailableThemes.RemoveAll(t => t.IsCustom || t.Category == "💾 Mis Temas Personalizados" || t.Name.Contains("MIS TEMAS PERSONALIZADOS") || t.IsEditorAction || t.Name.Contains("EDITOR Y ESTUDIO"));

            if (customThemes != null && customThemes.Count > 0)
            {
                AvailableThemes.Add(new ThemeOption { Name = "─── 💾 MIS TEMAS PERSONALIZADOS ───", IsHeader = true, Category = "💾 Mis Temas Personalizados" });
                foreach (var theme in customThemes)
                {
                    theme.IsCustom = true;
                    theme.Category = "💾 Mis Temas Personalizados";
                    AvailableThemes.Add(theme);
                }
            }

            AvailableThemes.Add(new ThemeOption { Name = "─── ⚙️ EDITOR Y ESTUDIO ───", IsHeader = true, Category = "⚙️ Editor" });
            AvailableThemes.Add(new ThemeOption { Name = "🎨 ⚙️ CREAR / EDITAR TEMA PERSONALIZADO...", Icon = "⚙️", Category = "⚙️ Editor", IsEditorAction = true });
        }

        public static void ApplyTheme(ThemeOption theme)
        {
            if (theme == null || theme.IsHeader || theme.IsEditorAction || Application.Current == null) return;

            var res = Application.Current.Resources;

            bool isLight = false;
            try
            {
                var c = (Color)ColorConverter.ConvertFromString(theme.BgDark);
                isLight = (c.R * 0.299 + c.G * 0.587 + c.B * 0.114) > 128;
            }
            catch { }

            string goldColor = theme.AccentGold;
            if (string.IsNullOrEmpty(goldColor) || (isLight && goldColor == "#FFD700"))
            {
                goldColor = isLight ? "#B45309" : "#FFD700";
            }

            SetResource(res, "BgDarkColor", "BgDarkBrush", theme.BgDark);
            SetResource(res, "CardBgColor", "CardBgBrush", theme.CardBg);
            SetResource(res, "CardHeaderColor", "CardHeaderBrush", theme.CardHeader);
            
            SetResource(res, "TextPrimaryColor", "TextPrimaryBrush", theme.TextPrimary);
            SetResource(res, "TextSecondaryColor", "TextSecondaryBrush", theme.TextSecondary);
            
            SetResource(res, "AccentCyanColor", "AccentCyanBrush", theme.AccentCyan);
            SetResource(res, "AccentAmberColor", "AccentAmberBrush", theme.AccentAmber);
            SetResource(res, "AccentGoldColor", "AccentGoldBrush", goldColor);
            SetResource(res, "AccentGreenColor", "AccentGreenBrush", theme.AccentGreen);
            SetResource(res, "AccentRedColor", "AccentRedBrush", theme.AccentRed);
            SetResource(res, "AccentPurpleColor", "AccentPurpleBrush", theme.AccentPurple);
            
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
