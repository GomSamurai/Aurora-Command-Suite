using System;

namespace AuroraDesignSuite.Models
{
    public class ImperialChronicleEvent
    {
        public double GameTimeSeconds { get; set; }
        public int EventTypeID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string MessageText { get; set; } = string.Empty;

        public string FormattedDateDisplay
        {
            get
            {
                if (GameTimeSeconds <= 0) return "Año 2100.01.01";
                // Aurora 4X epoch: 3600*24*365 seconds / year
                DateTime epoch = new DateTime(2100, 1, 1);
                try
                {
                    DateTime date = epoch.AddSeconds(GameTimeSeconds);
                    return date.ToString("yyyy-MM-dd HH:mm:ss");
                }
                catch
                {
                    return $"Año {(2100 + (GameTimeSeconds / (3600 * 24 * 365.25))):N1}";
                }
            }
        }

        public string CategoryIcon => CategoryName.ToLower() switch
        {
            var c when c.Contains("combat") || c.Contains("attack") || c.Contains("enemy") => "⚔️ COMBATE",
            var c when c.Contains("research") || c.Contains("tech") => "🔬 INVESTIGACIÓN",
            var c when c.Contains("survey") || c.Contains("discovery") || c.Contains("system") => "🧭 EXPLORACIÓN",
            var c when c.Contains("build") || c.Contains("ship") || c.Contains("production") => "🏭 INDUSTRIA",
            var c when c.Contains("commander") || c.Contains("officer") => "🎖️ OFICIALES",
            _ => "📜 EVENTO IMPERIAL"
        };
    }
}
