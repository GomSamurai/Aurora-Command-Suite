using System;

namespace AuroraDesignSuite.Models
{
    public class IndustrialProjectInfo
    {
        public int ProjectID { get; set; }
        public int GameID { get; set; }
        public int RaceID { get; set; }
        public int PopulationID { get; set; }
        public string Description { get; set; } = string.Empty;
        public double Amount { get; set; }
        public double Percentage { get; set; }
        public double PartialCompletion { get; set; }
        public double ProdPerUnit { get; set; }
        public bool Pause { get; set; }

        public string StatusDisplay => Pause ? "⏸️ En Pausa" : $"{Percentage:F1}% En Curso";
        public string AmountDisplay => $"{Amount:N2} Unidades";
        public string ProgressDisplay => $"{PartialCompletion:N2} BP Completados ({Percentage:F1}%)";

        public DateTime EstimatedCompletionDate { get; set; } = DateTime.Now.AddDays(180);
        public string CompletionDateDisplay
        {
            get
            {
                if (Pause) return "⏸️ Pausado";
                double remainingBP = Math.Max(0.0, (Amount * (ProdPerUnit > 0 ? ProdPerUnit : 120.0)) - PartialCompletion);
                double daysLeft = Math.Round(remainingBP / 10.0, 0); // Baseline factory output
                if (daysLeft <= 0) return "✅ Finalización Inminente";

                int years = (int)(daysLeft / 365.0);
                int months = (int)((daysLeft % 365.0) / 30.4);
                string timeStr = years > 0 ? $"{years} a. y {months} m." : $"{months} m.";
                return $"📅 Quedan ~{timeStr} (Calculado)";
            }
        }
    }

    public class PopulationInstallationInfo
    {
        public int PopID { get; set; }
        public int InstallationID { get; set; }
        public string InstallationName { get; set; } = string.Empty;
        public double Amount { get; set; }
        public double RecommendedAmount { get; set; } = 0;
        public string StatusBadge { get; set; } = "🟢 Óptimo";
        public string RecommendationHint { get; set; } = "Nivel adecuado para la población actual.";

        public string AmountDisplay => $"{Amount:N0} Instalaciones";
        public string RecommendedDisplay => RecommendedAmount > 0 ? $"{RecommendedAmount:N0} Rec." : "Variable";
    }
}
