using System;

namespace AuroraDesignSuite.Models
{
    public class ImperialChronicleEvent
    {
        public double GameTimeSeconds { get; set; }
        public int StartYear { get; set; } = 2026;
        public int EventTypeID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string MessageText { get; set; } = string.Empty;

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
            _ => "📜 EVENTO IMPERIAL"
        };

        public string BadgeBackground => CategoryIcon switch
        {
            "⚔️ COMBATE" => "#3B0E0E",
            "🔬 INVESTIGACIÓN" => "#09202C",
            "🧭 EXPLORACIÓN" => "#072414",
            "🏭 INDUSTRIA" => "#2A1D07",
            "🎖️ OFICIALES" => "#2C0F38",
            _ => "#0C192E"
        };

        public string BadgeBorder => CategoryIcon switch
        {
            "⚔️ COMBATE" => "#FF4444",
            "🔬 INVESTIGACIÓN" => "#00F0FF",
            "🧭 EXPLORACIÓN" => "#00FF88",
            "🏭 INDUSTRIA" => "#FFD700",
            "🎖️ OFICIALES" => "#D946EF",
            _ => "#3B82F6"
        };

        public string BadgeForeground => CategoryIcon switch
        {
            "⚔️ COMBATE" => "#FF6B6B",
            "🔬 INVESTIGACIÓN" => "#64F4FF",
            "🧭 EXPLORACIÓN" => "#66FFAA",
            "🏭 INDUSTRIA" => "#FFE066",
            "🎖️ OFICIALES" => "#F472B6",
            _ => "#93C5FD"
        };
    }
}
