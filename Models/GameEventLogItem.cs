using System;
using System.Windows.Media;

namespace AuroraDesignSuite.Models
{
    public class GameEventLogItem
    {
        public long IncrementID { get; set; }
        public double GameTimeSeconds { get; set; }
        public string FormattedTime { get; set; } = string.Empty;
        public int EventTypeID { get; set; }
        public string EventTypeDescription { get; set; } = string.Empty;
        public string MessageText { get; set; } = string.Empty;
        
        public string Category { get; set; } = "General";
        public string CategoryIcon { get; set; } = "💬";
        public Brush CategoryColor { get; set; } = Brushes.Gray;
        public Brush BorderColor { get; set; } = Brushes.Transparent;
        
        public bool IsInterrupt { get; set; }
        public bool IsCombat { get; set; }

        public static (string Category, string Icon, string HexColor) Categorize(int eventTypeId, string desc, string text)
        {
            string d = (desc + " " + text).ToLowerInvariant();

            if (d.Contains("attack") || d.Contains("combat") || d.Contains("hit") || d.Contains("damage") || 
                d.Contains("destroyed") || d.Contains("hostile") || d.Contains("alien") || d.Contains("missile") ||
                d.Contains("weapon") || d.Contains("fire") || d.Contains("armour") || d.Contains("penetrat"))
            {
                return ("Combate", "🔴", "#FF4A4A"); // Red
            }
            if (d.Contains("research") || d.Contains("tech") || d.Contains("project") || d.Contains("scientist") || 
                d.Contains("developed") || d.Contains("completed") || d.Contains("industry") || d.Contains("construction"))
            {
                return ("Investigación e Industria", "🔬", "#FFC107"); // Amber/Gold
            }
            if (d.Contains("fleet") || d.Contains("ship") || d.Contains("commander") || d.Contains("retired") || 
                d.Contains("assigned") || d.Contains("fuel") || d.Contains("supply") || d.Contains("promoted") || d.Contains("officer"))
            {
                return ("Flota y Oficiales", "⚓", "#28A745"); // Green
            }
            if (d.Contains("system") || d.Contains("survey") || d.Contains("jump") || d.Contains("body") || 
                d.Contains("planet") || d.Contains("comet") || d.Contains("discovered") || d.Contains("mineral"))
            {
                return ("Exploración", "🪐", "#00BCD4"); // Cyan
            }

            return ("General", "💬", "#9E9E9E"); // Neutral Gray
        }
    }
}
