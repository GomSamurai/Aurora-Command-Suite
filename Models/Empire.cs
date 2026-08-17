namespace AuroraDesignSuite.Models
{
    public class Empire
    {
        public int RaceID { get; set; }
        public int GameID { get; set; }
        public string RaceName { get; set; } = string.Empty;
        public string RaceTitle { get; set; } = string.Empty;
        public string FlagPic { get; set; } = "flag0000.jpg";
        public string RacePic { get; set; } = "Race001.bmp";
        public string ShipIcon { get; set; } = "Ship001.png";
        public string SpeciesName { get; set; } = "Human";
        public int SpeciesID { get; set; }

        public double ProductionRateModifier { get; set; } = 1.0;
        public double ResearchRateModifier { get; set; } = 1.0;
        public double PopulationGrowthModifier { get; set; } = 1.0;

        public double IdealTemperature { get; set; } = 287.03;
        public double TempDev { get; set; } = 24.0;
        public double IdealGravity { get; set; } = 1.0;
        public double GravDev { get; set; } = 0.9;
        public double IdealOxygen { get; set; } = 0.20;
        public double MaxPressure { get; set; } = 4.0;

        public int Xenophobia { get; set; } = 50;
        public int Diplomacy { get; set; } = 50;
        public int Militancy { get; set; } = 50;

        public string FlagPath { get; set; } = string.Empty;
        public string PortraitPath { get; set; } = string.Empty;
        public string ShipIconPath { get; set; } = string.Empty;

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
