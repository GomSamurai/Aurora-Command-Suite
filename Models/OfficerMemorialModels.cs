using System;
using System.Collections.Generic;

namespace AuroraDesignSuite.Models
{
    public class MemorialOfficerInfo
    {
        public int CommanderID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string RankName { get; set; } = "Oficial";
        public string CommanderTypeDisplay { get; set; } = "Naval";
        public bool IsDeceased { get; set; }
        public int RetireStatus { get; set; }
        public int MilitaryKillsTons { get; set; }
        public int CommercialKillsTons { get; set; }
        public int TotalMedalsCount { get; set; }
        public string MedalsSummary { get; set; } = "Sin condecoraciones registradas";

        public string StatusDisplay => IsDeceased
            ? "🔴 Caído en Acción / Servicio"
            : (RetireStatus > 0 ? "🟡 Retirado con Honor" : "🟢 En Activo (Vencedor Decorado)");

        public string KillsSummaryDisplay => (MilitaryKillsTons + CommercialKillsTons) > 0
            ? $"⚔️ {MilitaryKillsTons:N0} t Militares / {CommercialKillsTons:N0} t Comerciales Hundidas"
            : "🕊️ Sin bajas enemigas registradas";

        public List<OfficerMedalInfo> Medals { get; set; } = new List<OfficerMedalInfo>();
        public List<string> HistoryLogs { get; set; } = new List<string>();

        public override string ToString() => $"{RankName} {Name} - {StatusDisplay}";
    }

    public class OfficerMedalInfo
    {
        public int MedalID { get; set; }
        public string MedalName { get; set; } = string.Empty;
        public string MedalDescription { get; set; } = string.Empty;
        public int NumAwarded { get; set; } = 1;
        public string AwardReason { get; set; } = "Méritos Extraordinarios de Servicio";

        public string DisplayName => NumAwarded > 1 ? $"🏅 {MedalName} (x{NumAwarded})" : $"🏅 {MedalName}";
    }
}
