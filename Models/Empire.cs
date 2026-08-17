namespace AuroraDesignSuite.Models
{
    public class Empire
    {
        public int RaceID { get; set; }
        public int GameID { get; set; }
        public string RaceName { get; set; } = string.Empty;

        public override string ToString() => RaceName;
    }

    public class GameTimeInfo
    {
        public double GameTimeSeconds { get; set; }
        public int StartYear { get; set; } = 2026;
        public System.DateTime CurrentGameDate { get; set; } = new System.DateTime(2026, 1, 1);
        public double YearsElapsed => (CurrentGameDate - new System.DateTime(StartYear, 1, 1)).TotalDays / 365.25;

        public string FormattedCurrentDate => CurrentGameDate.ToString("dd'/'MM'/'yyyy HH:mm");
        public string FormattedStartYear => $"Inicio: {StartYear} ({YearsElapsed:F1} a. de servicio)";
    }
}
