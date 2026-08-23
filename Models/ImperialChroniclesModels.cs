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
            var c when c.Contains("combat") || c.Contains("attack") || c.Contains("enemy") || c.Contains("kill") => "⚔️ COMBATE",
            var c when c.Contains("research") || c.Contains("tech") => "🔬 INVESTIGACIÓN",
            var c when c.Contains("survey") || c.Contains("discovery") || c.Contains("system") => "🧭 EXPLORACIÓN",
            var c when c.Contains("build") || c.Contains("ship") || c.Contains("production") || c.Contains("harvester") => "🏭 INDUSTRIA",
            var c when c.Contains("commander") || c.Contains("officer") || c.Contains("retirement") || c.Contains("promot") => "🎖️ OFICIALES",
            _ => "📜 EVENTO IMPERIAL"
        };
    }
}
