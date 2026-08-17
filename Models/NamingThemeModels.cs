namespace AuroraDesignSuite.Models
{
    public class NamingThemeItem
    {
        public int ThemeID { get; set; }
        public string Description { get; set; } = string.Empty;

        public override string ToString() => $"[{ThemeID}] {Description}";
    }

    public class EmpireNamingConfig
    {
        public int RaceID { get; set; }
        public int ClassThemeID { get; set; }
        public int SystemThemeID { get; set; }
        public int DesignThemeID { get; set; }
        public int GroundThemeID { get; set; }
        public int MissileThemeID { get; set; }
        public int NameThemeID { get; set; }
    }
}
