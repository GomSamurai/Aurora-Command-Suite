using System;
using System.Windows;
using System.Windows.Media;
using AuroraDesignSuite.Services;

namespace AuroraDesignSuite.Models
{
    public class ImperialChronicleEvent
    {
        public double GameTimeSeconds { get; set; }
        public int StartYear { get; set; } = 2026;
        public int EventTypeID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string MessageText { get; set; } = string.Empty;

        public string TranslatedMessageText => ImperialChronicleTranslator.TranslateToEpicSpanish(MessageText);

        public string FormattedDateDisplay
        {
            get
            {
                int baseYear = StartYear > 0 ? StartYear : 2026;
                if (GameTimeSeconds <= 0) return $"Año {baseYear}.01.01";
                try
                {
                    DateTime epoch = new DateTime(baseYear, 1, 1);
                    DateTime date = epoch.AddSeconds(GameTimeSeconds);
                    return date.ToString("yyyy-MM-dd HH:mm:ss");
                }
                catch
                {
                    return $"Año {(baseYear + (GameTimeSeconds / (3600 * 24 * 365.25))):N1}";
                }
            }
        }

        public string CategoryIcon => CategoryName.ToLower() switch
        {
            var c when c.Contains("combat") || c.Contains("attack") || c.Contains("enemy") || c.Contains("kill") || c.Contains("destroy") => "⚔️ COMBATE",
            var c when c.Contains("research") || c.Contains("tech") => "🔬 INVESTIGACIÓN",
            var c when c.Contains("survey") || c.Contains("discovery") || c.Contains("system") => "🧭 EXPLORACIÓN",
            var c when c.Contains("build") || c.Contains("ship") || c.Contains("production") || c.Contains("harvester") || c.Contains("unit") => "🏭 INDUSTRIA",
            var c when c.Contains("commander") || c.Contains("officer") || c.Contains("retirement") || c.Contains("promot") || c.Contains("health") || c.Contains("assignment") => "🎖️ OFICIALES",
            var c when c.Contains("fuel") || c.Contains("logistics") || c.Contains("supply") => "⛽ LOGÍSTICA",
            _ => "📜 EVENTO IMPERIAL"
        };

        private bool IsLightTheme()
        {
            try
            {
                if (Application.Current?.Resources["BgDarkBrush"] is SolidColorBrush b)
                {
                    return (b.Color.R * 0.299 + b.Color.G * 0.587 + b.Color.B * 0.114) > 128;
                }
            }
            catch { }
            return false;
        }

        public string BadgeBackground
        {
            get
            {
                if (IsLightTheme())
                {
                    return CategoryIcon switch
                    {
                        "⚔️ COMBATE" => "#FEE2E2",
                        "🔬 INVESTIGACIÓN" => "#E0F2FE",
                        "🧭 EXPLORACIÓN" => "#DCFCE7",
                        "🏭 INDUSTRIA" => "#FEF3C7",
                        "🎖️ OFICIALES" => "#F3E8FF",
                        "⛽ LOGÍSTICA" => "#FFEDD5",
                        _ => "#E2E8F0"
                    };
                }

                return CategoryIcon switch
                {
                    "⚔️ COMBATE" => "#3B0E0E",
                    "🔬 INVESTIGACIÓN" => "#09202C",
                    "🧭 EXPLORACIÓN" => "#072414",
                    "🏭 INDUSTRIA" => "#2A1D07",
                    "🎖️ OFICIALES" => "#2C0F38",
                    "⛽ LOGÍSTICA" => "#3B2609",
                    _ => "#0C192E"
                };
            }
        }

        public string BadgeBorder
        {
            get
            {
                if (IsLightTheme())
                {
                    return CategoryIcon switch
                    {
                        "⚔️ COMBATE" => "#B91C1C",
                        "🔬 INVESTIGACIÓN" => "#0284C7",
                        "🧭 EXPLORACIÓN" => "#15803D",
                        "🏭 INDUSTRIA" => "#B45309",
                        "🎖️ OFICIALES" => "#6B21A8",
                        "⛽ LOGÍSTICA" => "#C2410C",
                        _ => "#475569"
                    };
                }

                return CategoryIcon switch
                {
                    "⚔️ COMBATE" => "#FF4444",
                    "🔬 INVESTIGACIÓN" => "#00F0FF",
                    "🧭 EXPLORACIÓN" => "#00FF88",
                    "🏭 INDUSTRIA" => "#FFD700",
                    "🎖️ OFICIALES" => "#D946EF",
                    "⛽ LOGÍSTICA" => "#F59E0B",
                    _ => "#3B82F6"
                };
            }
        }

        public string BadgeForeground => BadgeBorder;

        public string SeverityGroup => CategoryIcon switch
        {
            "⚔️ COMBATE" => "⚔️ Combates y Bajas",
            "🔬 INVESTIGACIÓN" => "🔬 Hitos Científicos",
            "🧭 EXPLORACIÓN" => "🧭 Descubrimientos",
            "🏭 INDUSTRIA" => "🏭 Producción Industrial",
            "🎖️ OFICIALES" => "🎖️ Decretos de Honor",
            _ => "📜 Eventos Generales"
        };
    }

    public class ImperialChroniclesTelemetry
    {
        public int TotalEvents { get; set; }
        public int ResearchEvents { get; set; }
        public int CombatEvents { get; set; }
        public int ExplorationEvents { get; set; }
        public int OfficerEvents { get; set; }
        public int IndustryEvents { get; set; }
        public int LogisticsEvents { get; set; }

        public double ResearchPercent => TotalEvents > 0 ? (ResearchEvents * 100.0 / TotalEvents) : 0;
        public double CombatPercent => TotalEvents > 0 ? (CombatEvents * 100.0 / TotalEvents) : 0;
        public double ExplorationPercent => TotalEvents > 0 ? (ExplorationEvents * 100.0 / TotalEvents) : 0;
        public double OfficerPercent => TotalEvents > 0 ? (OfficerEvents * 100.0 / TotalEvents) : 0;
        public double IndustryPercent => TotalEvents > 0 ? (IndustryEvents * 100.0 / TotalEvents) : 0;
        public double LogisticsPercent => TotalEvents > 0 ? (LogisticsEvents * 100.0 / TotalEvents) : 0;

        public string TopHeroName { get; set; } = "Sin Registrar";
        public string TopTechName { get; set; } = "Sin Registrar";
        public string TopDiscoveredSystem { get; set; } = "Sol";
    }
}
