using System;
using System.IO;
using System.Text;
using Microsoft.Data.Sqlite;
using AuroraDesignSuite.Models;

namespace AuroraDesignSuite.Services
{
    public class BlueprintExportService
    {
        public static bool ExportClassToAuroraDb(string dbPath, ShipDesign design, int raceId, out string message)
        {
            try
            {
                if (!File.Exists(dbPath))
                {
                    message = $"Base de datos no encontrada en: {dbPath}";
                    return false;
                }

                // Create backup before writing
                string backupPath = Path.Combine(Path.GetDirectoryName(dbPath) ?? "", "AuroraDB_Backup_Before_Export.db");
                File.Copy(dbPath, backupPath, true);

                using var conn = new SqliteConnection($"Data Source={dbPath};");
                conn.Open();

                // Get next ShipClassID
                using var idCmd = new SqliteCommand("SELECT COALESCE(MAX(ShipClassID), 0) + 1 FROM FCT_ShipClass", conn);
                int newClassId = Convert.ToInt32(idCmd.ExecuteScalar());

                // Get GameID for race
                using var gameCmd = new SqliteCommand("SELECT GameID FROM FCT_Race WHERE RaceID = @raceId", conn);
                gameCmd.Parameters.AddWithValue("@raceId", raceId);
                int gameId = Convert.ToInt32(gameCmd.ExecuteScalar() ?? 140);

                string insertClassSql = @"
                    INSERT INTO FCT_ShipClass (
                        ShipClassID, ClassName, GameID, RaceID, Size, Cost, Crew, 
                        MilitaryEngines, Commercial, FuelCapacity, ArmourThickness, ArmourWidth, 
                        PlannedDeployment, MaintSupplies, MaxSpeed, Locked
                    ) VALUES (
                        @classId, @className, @gameId, @raceId, @size, @cost, @crew,
                        @militaryEngines, @commercial, @fuelCap, @armorThick, @armorWidth,
                        @deployment, @msp, @maxSpeed, 0
                    )";

                using var cmd = new SqliteCommand(insertClassSql, conn);
                cmd.Parameters.AddWithValue("@classId", newClassId);
                cmd.Parameters.AddWithValue("@className", design.ClassName);
                cmd.Parameters.AddWithValue("@gameId", gameId);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                cmd.Parameters.AddWithValue("@size", design.TotalHS);
                cmd.Parameters.AddWithValue("@cost", design.TotalCostBP);
                cmd.Parameters.AddWithValue("@crew", design.TotalCrewRequired);
                cmd.Parameters.AddWithValue("@militaryEngines", design.IsMilitary ? 1 : 0);
                cmd.Parameters.AddWithValue("@commercial", design.IsMilitary ? 0 : 1);
                cmd.Parameters.AddWithValue("@fuelCap", design.TotalFuelLiters);
                cmd.Parameters.AddWithValue("@armorThick", design.ArmorThickness);
                cmd.Parameters.AddWithValue("@armorWidth", design.ArmorWidth);
                cmd.Parameters.AddWithValue("@deployment", design.PlannedDeploymentMonths);
                cmd.Parameters.AddWithValue("@msp", design.TotalMSP);
                cmd.Parameters.AddWithValue("@maxSpeed", design.MaxSpeedKmS);
                cmd.ExecuteNonQuery();

                // Insert Components into FCT_ClassComponent
                foreach (var item in design.Components)
                {
                    string insertCompSql = @"
                        INSERT INTO FCT_ClassComponent (GameID, ClassID, ComponentID, NumComponent)
                        VALUES (@gameId, @classId, @compId, @numComp)";

                    using var compCmd = new SqliteCommand(insertCompSql, conn);
                    compCmd.Parameters.AddWithValue("@gameId", gameId);
                    compCmd.Parameters.AddWithValue("@classId", newClassId);
                    compCmd.Parameters.AddWithValue("@compId", item.Component.ComponentID);
                    compCmd.Parameters.AddWithValue("@numComp", item.Quantity);
                    compCmd.ExecuteNonQuery();
                }

                message = $"✅ Clase '{design.ClassName}' (ID {newClassId}) inyectada en AuroraDB.db. ¡Ya disponible en tus astilleros en el juego!";
                return true;
            }
            catch (Exception ex)
            {
                message = $"Error exportando clase a AuroraDB: {ex.Message}";
                return false;
            }
        }

        public static bool ExportClassAsResearchProject(string dbPath, ShipDesign design, int raceId, out string message)
        {
            try
            {
                if (!File.Exists(dbPath))
                {
                    message = $"Base de datos no encontrada en: {dbPath}";
                    return false;
                }

                string backupPath = Path.Combine(Path.GetDirectoryName(dbPath) ?? "", "AuroraDB_Backup_Before_Export.db");
                File.Copy(dbPath, backupPath, true);

                using var conn = new SqliteConnection($"Data Source={dbPath};");
                conn.Open();

                // Get GameID for race
                using var gameCmd = new SqliteCommand("SELECT GameID FROM FCT_Race WHERE RaceID = @raceId", conn);
                gameCmd.Parameters.AddWithValue("@raceId", raceId);
                int gameId = Convert.ToInt32(gameCmd.ExecuteScalar() ?? 140);

                // Get PopID for Earth or capital
                using var popCmd = new SqliteCommand("SELECT PopulationID FROM FCT_Population WHERE RaceID = @raceId LIMIT 1", conn);
                popCmd.Parameters.AddWithValue("@raceId", raceId);
                int popId = Convert.ToInt32(popCmd.ExecuteScalar() ?? 4642);

                // Get Next TechID
                using var techIdCmd = new SqliteCommand("SELECT COALESCE(MAX(TechSystemID), 0) + 1 FROM FCT_TechSystem", conn);
                int nextTechId = Convert.ToInt32(techIdCmd.ExecuteScalar());

                // Prototype research RP needed
                int rpNeeded = Math.Max(1000, (int)(design.TotalCostBP * 10));

                // Insert into FCT_TechSystem
                string insertTechSql = @"
                    INSERT INTO FCT_TechSystem (
                        TechSystemID, GameID, RaceID, Name, Cost, CategoryID, AdditionalSystemID
                    ) VALUES (
                        @techId, @gameId, @raceId, @name, @cost, 1, 0
                    )";

                using var insertTechCmd = new SqliteCommand(insertTechSql, conn);
                insertTechCmd.Parameters.AddWithValue("@techId", nextTechId);
                insertTechCmd.Parameters.AddWithValue("@gameId", gameId);
                insertTechCmd.Parameters.AddWithValue("@raceId", raceId);
                insertTechCmd.Parameters.AddWithValue("@name", $"Prototipo de Clase: {design.ClassName}");
                insertTechCmd.Parameters.AddWithValue("@cost", rpNeeded);
                insertTechCmd.ExecuteNonQuery();

                // Get Next ProjectID
                using var projIdCmd = new SqliteCommand("SELECT COALESCE(MAX(ProjectID), 0) + 1 FROM FCT_ResearchProject", conn);
                int nextProjId = Convert.ToInt32(projIdCmd.ExecuteScalar());

                // Insert into FCT_ResearchProject
                string insertProjSql = @"
                    INSERT INTO FCT_ResearchProject (
                        ProjectID, GameID, TechID, RaceID, PopulationID, Facilities, ResSpecID, ResearchPointsRequired, Pause, AssignNew
                    ) VALUES (
                        @projId, @gameId, @techId, @raceId, @popId, 0, 0, @rp, 0, 0
                    )";

                using var insertProjCmd = new SqliteCommand(insertProjSql, conn);
                insertProjCmd.Parameters.AddWithValue("@projId", nextProjId);
                insertProjCmd.Parameters.AddWithValue("@gameId", gameId);
                insertProjCmd.Parameters.AddWithValue("@techId", nextTechId);
                insertProjCmd.Parameters.AddWithValue("@raceId", raceId);
                insertProjCmd.Parameters.AddWithValue("@popId", popId);
                insertProjCmd.Parameters.AddWithValue("@rp", rpNeeded);
                insertProjCmd.ExecuteNonQuery();

                message = $"🔬 Prototipo '{design.ClassName}' enviado a la cola de I+D ({rpNeeded:N0} RP requeridos). ¡Tus científicos y laboratorios deberán investigarlo en la partida antes de poder construirlo!";
                return true;
            }
            catch (Exception ex)
            {
                message = $"Error creando proyecto de I+D en AuroraDB: {ex.Message}";
                return false;
            }
        }

        public static string GenerateAuroraTextReport(ShipDesign design)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Clase {design.ClassName}  {design.TotalTonnage:N0} toneladas  {design.TotalCrewRequired} Tripulantes  {design.TotalCostBP:N1} BP  TCS {design.TotalHS:F0}  TH {design.ThermalSignature:N0}  EM {design.EMSignature:N0}");
            sb.AppendLine($"{design.MaxSpeedKmS:N0} km/s  Armadura {design.ArmorThickness}-{design.ArmorWidth}  Escudos {design.ShieldStrength:N0}  Sensores 0/0/0/0  DCR 0-0  PPV 0");
            sb.AppendLine($"Vida Mantenimiento {design.MaintenanceLifeYears:F2} Años  MSP {design.TotalMSP:N0}  AFR {design.AnnualFailureRate * 100:F0}%  IFR {design.AnnualFailureRate * 10:F1}%  1YR {design.TotalMSP * 0.2:F0}  5YR {design.TotalMSP * 0.8:F0}  Max Repair {design.TotalMSP * 0.5:F0} MSP");
            sb.AppendLine($"Tiempo Despliegue Intencionado: {design.PlannedDeploymentMonths} meses  Moral Tripulación: 100%");
            sb.AppendLine();
            sb.AppendLine("Componentes Equipados:");
            foreach (var item in design.Components)
            {
                sb.AppendLine($"  {item.Quantity}x {item.Component.ComponentName} (HS {item.TotalHS:F1}, {item.TotalCost:N1} BP)");
            }
            sb.AppendLine();
            sb.AppendLine("Desglose Mineral Necesario:");
            sb.AppendLine($"  Duranium: {design.Minerals.Duranium:N1} t | Sorium: {design.Minerals.Sorium:N1} t | Gallicite: {design.Minerals.Gallicite:N1} t | Neutronium: {design.Minerals.Neutronium:N1} t");
            sb.AppendLine($"  Corundium: {design.Minerals.Corundium:N1} t | Uridium: {design.Minerals.Uridium:N1} t | Tritium: {design.Minerals.Tritium:N1} t | Boronide: {design.Minerals.Boronide:N1} t");
            sb.AppendLine();
            sb.AppendLine("Diseñado con Aurora Design & Empire Command Center Suite");
            return sb.ToString();
        }
    }
}
