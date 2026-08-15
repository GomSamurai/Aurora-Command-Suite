namespace AuroraDesignSuite.Models
{
    public class MineralRequirement
    {
        public double Duranium { get; set; }
        public double Sorium { get; set; }
        public double Neutronium { get; set; }
        public double Corundium { get; set; }
        public double Uridium { get; set; }
        public double Corbomite { get; set; }
        public double Tritium { get; set; }
        public double Boronide { get; set; }
        public double Mercassium { get; set; }
        public double Vendarite { get; set; }
        public double Gallicite { get; set; }

        public double TotalCost => Duranium + Sorium + Neutronium + Corundium + Uridium + 
                                  Corbomite + Tritium + Boronide + Mercassium + Vendarite + Gallicite;

        public void Add(string mineral, double amount)
        {
            switch (mineral.Trim().ToLower())
            {
                case "duranium": Duranium += amount; break;
                case "sorium": Sorium += amount; break;
                case "neutronium": Neutronium += amount; break;
                case "corundium": Corundium += amount; break;
                case "uridium": Uridium += amount; break;
                case "corbomite": Corbomite += amount; break;
                case "tritium": Tritium += amount; break;
                case "boronide": Boronide += amount; break;
                case "mercassium": Mercassium += amount; break;
                case "vendarite": Vendarite += amount; break;
                case "gallicite": Gallicite += amount; break;
            }
        }
    }
}
