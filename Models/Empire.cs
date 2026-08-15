namespace AuroraDesignSuite.Models
{
    public class Empire
    {
        public int RaceID { get; set; }
        public int GameID { get; set; }
        public string RaceName { get; set; } = string.Empty;

        public override string ToString() => RaceName;
    }
}
