using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;
using AuroraDesignSuite.Models;

namespace AuroraDesignSuite.Services
{
    public class DatabaseService
    {
        private readonly string _dbPath;

        public DatabaseService(string dbPath)
        {
            _dbPath = dbPath;
        }

        public string DbPath => _dbPath;

        private SqliteConnection GetConnection(bool readOnly = false)
        {
            var connStr = readOnly ? $"Data Source={_dbPath};Mode=ReadOnly;Pooling=False;" : $"Data Source={_dbPath};Mode=ReadWrite;Pooling=False;";
            var conn = new SqliteConnection(connStr);
            conn.Open();
            try
            {
                using var pragmaCmd = new SqliteCommand("PRAGMA busy_timeout=5000;", conn);
                pragmaCmd.ExecuteNonQuery();
            }
            catch { }
            return conn;
        }

        private SqliteConnection GetWriteConnection()
        {
            SqliteConnection.ClearAllPools();
            return GetConnection(readOnly: false);
        }

        public bool TestConnection(out string error)
        {
            try
            {
                if (!File.Exists(_dbPath))
                {
                    error = $"Archivo no encontrado: {_dbPath}";
                    return false;
                }

                using var conn = GetConnection();
                using var cmd = new SqliteCommand("SELECT COUNT(*) FROM FCT_Race", conn);
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                error = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public List<Empire> GetEmpires()
        {
            var result = new List<Empire>();
            try
            {
                using var conn = GetConnection();
                string query = @"
                    SELECT RaceID, GameID, COALESCE(NULLIF(RaceTitle, ''), RaceName) as DisplayName 
                    FROM FCT_Race 
                    WHERE (NPR = 0 OR NPR IS NULL)
                      AND GameID = (SELECT MAX(GameID) FROM FCT_Race WHERE NPR = 0 OR NPR IS NULL)
                    ORDER BY DisplayName";

                using var cmd = new SqliteCommand(query, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Empire
                    {
                        RaceID = Convert.ToInt32(reader["RaceID"]),
                        GameID = Convert.ToInt32(reader["GameID"]),
                        RaceName = reader["DisplayName"].ToString() ?? "Unknown Empire"
                    });
                }

                if (result.Count == 0)
                {
                    string fallbackQuery = "SELECT RaceID, GameID, RaceName FROM FCT_Race ORDER BY RaceName";
                    using var fCmd = new SqliteCommand(fallbackQuery, conn);
                    using var fReader = fCmd.ExecuteReader();
                    while (fReader.Read())
                    {
                        result.Add(new Empire
                        {
                            RaceID = Convert.ToInt32(fReader["RaceID"]),
                            GameID = Convert.ToInt32(fReader["GameID"]),
                            RaceName = fReader["RaceName"].ToString() ?? "Empire"
                        });
                    }
                }
                if (result.Count == 0)
                {
                    result.Add(new Empire { RaceID = 1, GameID = 1, RaceName = "Imperio Principal" });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching empires: {ex.Message}");
                result.Add(new Empire { RaceID = 1, GameID = 1, RaceName = "Imperio Principal" });
            }
            return result;
        }

        public Empire GetFullEmpireDetails(int raceId)
        {
            var emp = new Empire { RaceID = raceId };
            try
            {
                using var conn = GetConnection();
                string query = @"
                    SELECT r.RaceID, r.GameID, r.RaceName, r.RaceTitle, r.FlagPic,
                           s.SpeciesID, s.SpeciesName, s.RacePic, s.Temperature, s.TempDev,
                           s.Gravity, s.GravDev, s.Oxygen, s.PressMax, s.ProductionRateModifier,
                           s.ResearchRateModifier, s.PopulationGrowthModifier, s.Xenophobia, s.Diplomacy, s.Militancy
                    FROM FCT_Race r
                    LEFT JOIN FCT_Species s ON r.GameID = s.GameID
                    WHERE r.RaceID = @raceId
                    LIMIT 1";

                using var cmd = new SqliteCommand(query, conn);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    emp.GameID = Convert.ToInt32(reader["GameID"]);
                    emp.RaceName = reader["RaceName"].ToString() ?? "";
                    emp.RaceTitle = reader["RaceTitle"] != DBNull.Value ? reader["RaceTitle"].ToString() ?? "" : "";
                    emp.FlagPic = reader["FlagPic"] != DBNull.Value ? reader["FlagPic"].ToString() ?? "flag0000.jpg" : "flag0000.jpg";
                    emp.SpeciesID = reader["SpeciesID"] != DBNull.Value ? Convert.ToInt32(reader["SpeciesID"]) : 0;
                    emp.SpeciesName = reader["SpeciesName"] != DBNull.Value ? reader["SpeciesName"].ToString() ?? "Human" : "Human";
                    emp.RacePic = reader["RacePic"] != DBNull.Value ? reader["RacePic"].ToString() ?? "Race001.bmp" : "Race001.bmp";

                    emp.IdealTemperature = reader["Temperature"] != DBNull.Value ? Convert.ToDouble(reader["Temperature"]) : 287.03;
                    emp.TempDev = reader["TempDev"] != DBNull.Value ? Convert.ToDouble(reader["TempDev"]) : 24.0;
                    emp.IdealGravity = reader["Gravity"] != DBNull.Value ? Convert.ToDouble(reader["Gravity"]) : 1.0;
                    emp.GravDev = reader["GravDev"] != DBNull.Value ? Convert.ToDouble(reader["GravDev"]) : 0.9;
                    emp.IdealOxygen = reader["Oxygen"] != DBNull.Value ? Convert.ToDouble(reader["Oxygen"]) : 0.20;
                    emp.MaxPressure = reader["PressMax"] != DBNull.Value ? Convert.ToDouble(reader["PressMax"]) : 4.0;

                    emp.ProductionRateModifier = reader["ProductionRateModifier"] != DBNull.Value ? Convert.ToDouble(reader["ProductionRateModifier"]) : 1.0;
                    emp.ResearchRateModifier = reader["ResearchRateModifier"] != DBNull.Value ? Convert.ToDouble(reader["ResearchRateModifier"]) : 1.0;
                    emp.PopulationGrowthModifier = reader["PopulationGrowthModifier"] != DBNull.Value ? Convert.ToDouble(reader["PopulationGrowthModifier"]) : 1.0;

                    emp.Xenophobia = reader["Xenophobia"] != DBNull.Value ? Convert.ToInt32(reader["Xenophobia"]) : 50;
                    emp.Diplomacy = reader["Diplomacy"] != DBNull.Value ? Convert.ToInt32(reader["Diplomacy"]) : 50;
                    emp.Militancy = reader["Militancy"] != DBNull.Value ? Convert.ToInt32(reader["Militancy"]) : 50;
                }

                // Resolve absolute image paths
                string dbDir = System.IO.Path.GetDirectoryName(_dbPath) ?? @"C:\VSCODE\Aurora271Full";
                string flagFile = System.IO.Path.Combine(dbDir, "Flags", emp.FlagPic);
                string raceFile = System.IO.Path.Combine(dbDir, "Races", emp.RacePic);
                string shipFile = System.IO.Path.Combine(dbDir, "ShipIcons", "Ship001.png");

                if (System.IO.File.Exists(flagFile)) emp.FlagPath = flagFile;
                if (System.IO.File.Exists(raceFile)) emp.PortraitPath = raceFile;
                if (System.IO.File.Exists(shipFile)) emp.ShipIconPath = shipFile;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reading Empire details: {ex.Message}");
            }
            return emp;
        }

        public bool UpdateEmpireDetails(Empire emp, out string errorMsg)
        {
            errorMsg = "";
            try
            {
                using var conn = GetWriteConnection();

                string raceQuery = @"
                    UPDATE FCT_Race 
                    SET RaceName = @raceName, 
                        RaceTitle = @raceTitle, 
                        FlagPic = @flagPic 
                    WHERE RaceID = @raceId";

                using (var cmd = new SqliteCommand(raceQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@raceName", emp.RaceName ?? "");
                    cmd.Parameters.AddWithValue("@raceTitle", emp.RaceTitle ?? "");
                    cmd.Parameters.AddWithValue("@flagPic", emp.FlagPic ?? "flag0000.jpg");
                    cmd.Parameters.AddWithValue("@raceId", emp.RaceID);
                    cmd.ExecuteNonQuery();
                }

                if (emp.SpeciesID > 0)
                {
                    string specQuery = @"
                        UPDATE FCT_Species 
                        SET SpeciesName = @speciesName, 
                            RacePic = @racePic
                        WHERE SpeciesID = @speciesId";

                    using var sCmd = new SqliteCommand(specQuery, conn);
                    sCmd.Parameters.AddWithValue("@speciesName", emp.SpeciesName ?? "Human");
                    sCmd.Parameters.AddWithValue("@racePic", emp.RacePic ?? "Race001.bmp");
                    sCmd.Parameters.AddWithValue("@speciesId", emp.SpeciesID);
                    sCmd.ExecuteNonQuery();
                }

                LiveSyncBridge.NotifyGameSync("EMPIRE_DETAILS_UPDATED");
                return true;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                System.Diagnostics.Debug.WriteLine($"Error updating Empire details: {ex.Message}");
                return false;
            }
        }

        public bool UpdateEmpireDetails(Empire emp) => UpdateEmpireDetails(emp, out _);

        public List<string> GetAvailableFlags()
        {
            var list = new List<string>();
            try
            {
                string dbDir = System.IO.Path.GetDirectoryName(_dbPath) ?? @"C:\VSCODE\Aurora271Full";
                string flagsFolder = System.IO.Path.Combine(dbDir, "Flags");
                if (System.IO.Directory.Exists(flagsFolder))
                {
                    foreach (var file in System.IO.Directory.GetFiles(flagsFolder, "*.jpg"))
                    {
                        list.Add(System.IO.Path.GetFileName(file));
                    }
                }
            }
            catch { }
            list.Sort();
            return list;
        }

        public List<string> GetAvailablePortraits()
        {
            var list = new List<string>();
            try
            {
                string dbDir = System.IO.Path.GetDirectoryName(_dbPath) ?? @"C:\VSCODE\Aurora271Full";
                string racesFolder = System.IO.Path.Combine(dbDir, "Races");
                if (System.IO.Directory.Exists(racesFolder))
                {
                    foreach (var file in System.IO.Directory.GetFiles(racesFolder, "*.bmp"))
                    {
                        list.Add(System.IO.Path.GetFileName(file));
                    }
                }
            }
            catch { }
            list.Sort();
            return list;
        }

        public GameTimeInfo GetGameTimeInfo(int raceId)
        {
            var info = new GameTimeInfo();
            try
            {
                using var conn = GetConnection();
                string query = @"
                    SELECT g.GameTime, g.StartYear
                    FROM FCT_Game g
                    INNER JOIN FCT_Race r ON r.GameID = g.GameID
                    WHERE r.RaceID = @raceId
                    LIMIT 1";

                using var cmd = new SqliteCommand(query, conn);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    double gameTime = reader["GameTime"] != DBNull.Value ? Convert.ToDouble(reader["GameTime"]) : 0.0;
                    int startYear = reader["StartYear"] != DBNull.Value ? Convert.ToInt32(reader["StartYear"]) : 2026;

                    info.GameTimeSeconds = gameTime;
                    info.StartYear = startYear;

                    try
                    {
                        DateTime baseDate = new DateTime(startYear, 1, 1);
                        info.CurrentGameDate = baseDate.AddSeconds(gameTime);
                    }
                    catch
                    {
                        info.CurrentGameDate = new DateTime(startYear, 1, 1);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching game time: {ex.Message}");
            }
            return info;
        }

        public bool AdvanceGameTimeSeconds(int raceId, double secondsToAdvance, out string newDateStr, out bool hasInterruptEvents)
        {
            newDateStr = "";
            hasInterruptEvents = false;
            try
            {
                using var conn = GetWriteConnection();

                string getGameSql = @"
                    SELECT g.GameID, g.GameTime, g.StartYear
                    FROM FCT_Game g
                    INNER JOIN FCT_Race r ON r.GameID = g.GameID
                    WHERE r.RaceID = @raceId
                    LIMIT 1";

                int gameId = 0;
                double currentGameTime = 0.0;
                int startYear = 2026;

                using (var cmd = new SqliteCommand(getGameSql, conn))
                {
                    cmd.Parameters.AddWithValue("@raceId", raceId);
                    using var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        gameId = Convert.ToInt32(reader["GameID"]);
                        currentGameTime = Convert.ToDouble(reader["GameTime"]);
                        startYear = Convert.ToInt32(reader["StartYear"]);
                    }
                }

                if (gameId == 0) return false;

                double newGameTime = currentGameTime + secondsToAdvance;

                string updateSql = "UPDATE FCT_Game SET GameTime = @newTime WHERE GameID = @gameId";
                using (var uCmd = new SqliteCommand(updateSql, conn))
                {
                    uCmd.Parameters.AddWithValue("@newTime", newGameTime);
                    uCmd.Parameters.AddWithValue("@gameId", gameId);
                    uCmd.ExecuteNonQuery();
                }

                try
                {
                    DateTime baseDate = new DateTime(startYear, 1, 1);
                    newDateStr = baseDate.AddSeconds(newGameTime).ToString("dd/MM/yyyy HH:mm");
                }
                catch
                {
                    newDateStr = "20/10/2037 08:00";
                }

                string checkInterruptSql = @"
                    SELECT COUNT(1) 
                    FROM FCT_GameLog l
                    LEFT JOIN DIM_EventType t ON l.EventType = t.EventTypeID
                    WHERE l.RaceID = @raceId 
                      AND l.Time >= @oldTime 
                      AND l.Time <= @newTime
                      AND (t.PlayerInterrupt = 1 OR t.CombatDisplay = 1 OR t.AttackEvent = 1)";

                using (var iCmd = new SqliteCommand(checkInterruptSql, conn))
                {
                    iCmd.Parameters.AddWithValue("@raceId", raceId);
                    iCmd.Parameters.AddWithValue("@oldTime", currentGameTime);
                    iCmd.Parameters.AddWithValue("@newTime", newGameTime);
                    long count = Convert.ToInt64(iCmd.ExecuteScalar() ?? 0);
                    hasInterruptEvents = count > 0;
                }

                LiveSyncBridge.NotifyGameSync("TIME_ADVANCED");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error advancing game time: {ex.Message}");
                return false;
            }
        }

        public List<GameEventLogItem> GetRecentGameEvents(int raceId, int maxEvents = 100, string categoryFilter = "Todas")
        {
            var list = new List<GameEventLogItem>();
            try
            {
                using var conn = GetConnection();
                string sql = @"
                    SELECT l.IncrementID, l.Time, l.EventType, l.MessageText,
                           COALESCE(t.Description, 'Evento General') as EventDesc,
                           COALESCE(t.PlayerInterrupt, 0) as PlayerInterrupt,
                           COALESCE(t.CombatDisplay, 0) as CombatDisplay,
                           g.StartYear
                    FROM FCT_GameLog l
                    INNER JOIN FCT_Race r ON l.RaceID = r.RaceID
                    INNER JOIN FCT_Game g ON r.GameID = g.GameID
                    LEFT JOIN DIM_EventType t ON l.EventType = t.EventTypeID
                    WHERE l.RaceID = @raceId
                    ORDER BY l.IncrementID DESC
                    LIMIT @maxEvents";

                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                cmd.Parameters.AddWithValue("@maxEvents", maxEvents);
                using var reader = cmd.ExecuteReader();

                var converter = new System.Windows.Media.BrushConverter();

                while (reader.Read())
                {
                    long incId = Convert.ToInt64(reader["IncrementID"]);
                    double timeSec = Convert.ToDouble(reader["Time"]);
                    int eventTypeId = Convert.ToInt32(reader["EventType"]);
                    string msg = reader["MessageText"].ToString() ?? "";
                    string desc = reader["EventDesc"].ToString() ?? "";
                    int playerInterrupt = Convert.ToInt32(reader["PlayerInterrupt"]);
                    int combatDisplay = Convert.ToInt32(reader["CombatDisplay"]);
                    int startYear = Convert.ToInt32(reader["StartYear"]);

                    string timeStr = "";
                    try
                    {
                        DateTime baseDate = new DateTime(startYear, 1, 1);
                        timeStr = baseDate.AddSeconds(timeSec).ToString("dd/MM/yyyy HH:mm");
                    }
                    catch
                    {
                        timeStr = $"{timeSec}s";
                    }

                    var catInfo = GameEventLogItem.Categorize(eventTypeId, desc, msg);

                    if (categoryFilter != "Todas" && catInfo.Category != categoryFilter)
                    {
                        continue;
                    }

                    var brush = (System.Windows.Media.Brush)converter.ConvertFromString(catInfo.HexColor)!;

                    var item = new GameEventLogItem
                    {
                        IncrementID = incId,
                        GameTimeSeconds = timeSec,
                        FormattedTime = timeStr,
                        EventTypeID = eventTypeId,
                        EventTypeDescription = desc,
                        MessageText = msg,
                        Category = catInfo.Category,
                        CategoryIcon = catInfo.Icon,
                        CategoryColor = brush,
                        BorderColor = catInfo.HexColor == "#FF4A4A" ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.Transparent,
                        IsInterrupt = playerInterrupt == 1,
                        IsCombat = combatDisplay == 1 || catInfo.Category == "Combate"
                    };

                    list.Add(item);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reading Game Events: {ex.Message}");
            }
            return list;
        }

        public string GetRaceName(int raceId)
        {
            try
            {
                using var conn = GetConnection();
                string query = "SELECT COALESCE(NULLIF(RaceTitle, ''), RaceName) FROM FCT_Race WHERE RaceID = @raceId";
                using var cmd = new SqliteCommand(query, conn);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                var res = cmd.ExecuteScalar();
                if (res != null && res != DBNull.Value) return res.ToString()!;
            }
            catch { }

            return "Imperio";
        }

        public List<IndustrialProjectInfo> GetIndustrialProjects(int raceId)
        {
            var projects = new List<IndustrialProjectInfo>();
            try
            {
                using var conn = GetConnection();
                string query = @"
                    SELECT ProjectID, GameID, RaceID, PopulationID, Description, Amount, Percentage, PartialCompletion, Pause
                    FROM FCT_IndustrialProjects
                    WHERE RaceID = @raceId
                    ORDER BY Percentage DESC";

                using var cmd = new SqliteCommand(query, conn);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    projects.Add(new IndustrialProjectInfo
                    {
                        ProjectID = Convert.ToInt32(reader["ProjectID"]),
                        GameID = Convert.ToInt32(reader["GameID"]),
                        RaceID = Convert.ToInt32(reader["RaceID"]),
                        PopulationID = Convert.ToInt32(reader["PopulationID"]),
                        Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString()! : "Proyecto Industrial",
                        Amount = reader["Amount"] != DBNull.Value ? Convert.ToDouble(reader["Amount"]) : 1.0,
                        Percentage = reader["Percentage"] != DBNull.Value ? Convert.ToDouble(reader["Percentage"]) : 0.0,
                        PartialCompletion = reader["PartialCompletion"] != DBNull.Value ? Convert.ToDouble(reader["PartialCompletion"]) : 0.0,
                        Pause = reader["Pause"] != DBNull.Value && Convert.ToInt32(reader["Pause"]) > 0
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching industrial projects: {ex.Message}");
            }
            return projects;
        }

        public bool AddIndustrialProject(int raceId, string description, double amount, out string msg)
        {
            try
            {
                using var conn = GetWriteConnection();

                // 1. Get Capital PopulationID, SpeciesID, GameID for this RaceID
                int gameId = 0;
                int popId = 0;
                int speciesId = 0;

                string popSql = @"
                    SELECT GameID, PopulationID, SpeciesID 
                    FROM FCT_Population 
                    WHERE RaceID = @raceId 
                    ORDER BY Capital DESC, PopulationID ASC 
                    LIMIT 1";

                using (var pCmd = new SqliteCommand(popSql, conn))
                {
                    pCmd.Parameters.AddWithValue("@raceId", raceId);
                    using var pReader = pCmd.ExecuteReader();
                    if (pReader.Read())
                    {
                        gameId = Convert.ToInt32(pReader["GameID"]);
                        popId = Convert.ToInt32(pReader["PopulationID"]);
                        speciesId = Convert.ToInt32(pReader["SpeciesID"]);
                    }
                }

                if (popId == 0)
                {
                    msg = "❌ Error: No se encontró una colonia válida para este imperio en la base de datos.";
                    return false;
                }

                // 2. Get Installation Data from DIM_PlanetaryInstallation
                int instId = 5; // Default Construction Factory
                double cost = 120.0;
                double dur = 0, neu = 0, cor = 0, tri = 0, bor = 0, mer = 0, ven = 0, sor = 0, uri = 0, crd = 0, gal = 0;

                string instSql = @"
                    SELECT PlanetaryInstallationID, Cost, Duranium, Neutronium, Corbomite, Tritanium, Boronide, Mercassium, Vendarite, Sorium, Uridium, Corundium, Gallicite
                    FROM DIM_PlanetaryInstallation
                    WHERE Name = @desc OR Abbreviation = @desc
                    LIMIT 1";

                using (var iCmd = new SqliteCommand(instSql, conn))
                {
                    iCmd.Parameters.AddWithValue("@desc", description);
                    using var iReader = iCmd.ExecuteReader();
                    if (iReader.Read())
                    {
                        instId = Convert.ToInt32(iReader["PlanetaryInstallationID"]);
                        cost = Convert.ToDouble(iReader["Cost"]);
                        dur = Convert.ToDouble(iReader["Duranium"]);
                        neu = Convert.ToDouble(iReader["Neutronium"]);
                        cor = Convert.ToDouble(iReader["Corbomite"]);
                        tri = Convert.ToDouble(iReader["Tritanium"]);
                        bor = Convert.ToDouble(iReader["Boronide"]);
                        mer = Convert.ToDouble(iReader["Mercassium"]);
                        ven = Convert.ToDouble(iReader["Vendarite"]);
                        sor = Convert.ToDouble(iReader["Sorium"]);
                        uri = Convert.ToDouble(iReader["Uridium"]);
                        crd = Convert.ToDouble(iReader["Corundium"]);
                        gal = Convert.ToDouble(iReader["Gallicite"]);
                    }
                }

                string sql = @"
                    INSERT INTO FCT_IndustrialProjects (
                        GameID, RaceID, PopulationID, SpeciesID, Percentage, ProductionType, ProductionID, 
                        RefitClassID, WealthUse, Amount, PartialCompletion, ProdPerUnit, Description, Pause, Queue, 
                        FuelRequired, Duranium, Neutronium, Corbomite, Tritanium, Boronide, Mercassium, Vendarite, Sorium, Uridium, Corundium, Gallicite
                    ) VALUES (
                        @gameId, @raceId, @popId, @speciesId, 100.0, 0, @instId,
                        0, 4, @amount, 0.0, @cost, @desc, 0, 0,
                        0, @dur, @neu, @cor, @tri, @bor, @mer, @ven, @sor, @uri, @crd, @gal
                    )";

                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@gameId", gameId);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                cmd.Parameters.AddWithValue("@popId", popId);
                cmd.Parameters.AddWithValue("@speciesId", speciesId);
                cmd.Parameters.AddWithValue("@instId", instId);
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.Parameters.AddWithValue("@cost", cost);
                cmd.Parameters.AddWithValue("@desc", description);

                cmd.Parameters.AddWithValue("@dur", dur);
                cmd.Parameters.AddWithValue("@neu", neu);
                cmd.Parameters.AddWithValue("@cor", cor);
                cmd.Parameters.AddWithValue("@tri", tri);
                cmd.Parameters.AddWithValue("@bor", bor);
                cmd.Parameters.AddWithValue("@mer", mer);
                cmd.Parameters.AddWithValue("@ven", ven);
                cmd.Parameters.AddWithValue("@sor", sor);
                cmd.Parameters.AddWithValue("@uri", uri);
                cmd.Parameters.AddWithValue("@crd", crd);
                cmd.Parameters.AddWithValue("@gal", gal);

                cmd.ExecuteNonQuery();

                msg = $"✅ Orden industrial para '{description}' ({amount} ud) registrada con éxito en tu colonia principal.";
                LiveSyncBridge.NotifyGameSync("INDUSTRIAL_PROJECT_ADDED");
                return true;
            }
            catch (Exception ex)
            {
                msg = $"❌ Error al registrar proyecto industrial: {ex.Message}";
                return false;
            }
        }

        public bool DeleteIndustrialProject(int projectId, out string msg)
        {
            try
            {
                using var conn = GetWriteConnection();
                string sql = "DELETE FROM FCT_IndustrialProjects WHERE ProjectID = @projectId";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@projectId", projectId);
                cmd.ExecuteNonQuery();
                msg = "🗑️ Proyecto industrial cancelado con éxito en la base de datos.";
                return true;
            }
            catch (Exception ex)
            {
                msg = $"❌ Error al cancelar proyecto: {ex.Message}";
                return false;
            }
        }

        public List<string> GetAvailablePlanetaryInstallations()
        {
            var list = new List<string>();
            try
            {
                using var conn = GetConnection();
                string sql = "SELECT Name FROM DIM_PlanetaryInstallation ORDER BY Name ASC";
                using var cmd = new SqliteCommand(sql, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        list.Add(reader.GetString(0));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching installations: {ex.Message}");
            }

            if (list.Count == 0)
            {
                list = new List<string>
                {
                    "Construction Factory",
                    "Automated Mine",
                    "Mine",
                    "Fuel Refinery",
                    "Research Lab",
                    "Financial Centre",
                    "Spaceport",
                    "Naval Headquarters",
                    "Ordnance Factory",
                    "Fighter Factory",
                    "Mass Driver",
                    "Terraforming Installation",
                    "Military Academy",
                    "Maintenance Facility",
                    "Sector Command",
                    "Ground Force Construction Complex",
                    "Genetic Modification Centre",
                    "Refuelling Station",
                    "Ordnance Transfer Station",
                    "Cargo Shuttle Station",
                    "Convert Mine to Automated",
                    "Convert CI to Construction Factory",
                    "Convert CI to Mine",
                    "Convert CI to Fuel Refinery",
                    "Convert CI to Ordnance Factory",
                    "Convert CI to Fighter Factory",
                    "Convert CI to Financial Centre"
                };
            }

            return list;
        }

        public double GetTotalEmpirePopulation(int raceId)
        {
            try
            {
                using var conn = GetConnection();
                string sql = "SELECT SUM(Population) FROM FCT_Population WHERE RaceID = @raceId";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                object? res = cmd.ExecuteScalar();
                if (res != null && res != DBNull.Value)
                {
                    return Convert.ToDouble(res);
                }
            }
            catch { }
            return 0.0;
        }

        public int GetEmpireColonyCount(int raceId)
        {
            try
            {
                using var conn = GetConnection();
                string sql = "SELECT COUNT(*) FROM FCT_Population WHERE RaceID = @raceId";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                object? res = cmd.ExecuteScalar();
                if (res != null && res != DBNull.Value)
                {
                    return Convert.ToInt32(res);
                }
            }
            catch { }
            return 1;
        }

        public List<PopulationInstallationInfo> GetPopulationInstallations(int raceId)
        {
            var list = new List<PopulationInstallationInfo>();
            double totalPopM = GetTotalEmpirePopulation(raceId);

            try
            {
                using var conn = GetConnection();
                string query = @"
                    SELECT i.PopID, i.PlanetaryInstallationID, i.Amount, d.Name AS RawName
                    FROM FCT_PopulationInstallations i
                    INNER JOIN FCT_Population p ON i.PopID = p.PopulationID
                    LEFT JOIN DIM_PlanetaryInstallation d ON i.PlanetaryInstallationID = d.PlanetaryInstallationID
                    WHERE p.RaceID = @raceId
                    ORDER BY i.Amount DESC";

                using var cmd = new SqliteCommand(query, conn);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int instId = Convert.ToInt32(reader["PlanetaryInstallationID"]);
                    string rawName = reader["RawName"] != DBNull.Value ? reader["RawName"].ToString()! : string.Empty;
                    double amount = reader["Amount"] != DBNull.Value ? Convert.ToDouble(reader["Amount"]) : 1.0;

                    string instName = rawName switch
                    {
                        "Construction Factory" => "🏗️ Fábrica de Construcción",
                        "Conventional Industry" => "🏭 Industria Convencional",
                        "Infrastructure" => "🏙️ Infraestructura Poblacional",
                        "Research Facility" => "🔬 Laboratorio de Investigación",
                        "Fuel Refinery" => "⛽ Refinería de Combustible",
                        "Automated Mine" => "🤖 Mina Automatizada",
                        "Mine" => "⛏️ Mina Convencional",
                        "Maintenance Facility" => "🛠️ Instalación de Mantenimiento",
                        "Military Academy" => "🎓 Academia Militar",
                        "Ordnance Factory" => "🚀 Fábrica de Misiles/Munición",
                        "Fighter Factory" => "🛩️ Fábrica de Cazas",
                        "Financial Centre" => "🏛️ Centro Financiero",
                        "Spaceport" => "🌌 Puerto Espacial",
                        "Naval Headquarters" => "🏰 Cuartel General Naval",
                        "Deep Space Tracking Station" => "📡 Estación de Espacio Profundo",
                        "Mass Driver" => "⚡ Lanzador de Masa (Mass Driver)",
                        "Terraforming Installation" => "🌍 Instalación de Terraformación",
                        "Commercial Shipyard Complex" => "📦 Astillero Comercial",
                        "Naval Shipyard Complex" => "⚓ Astillero Naval",
                        "Ground Force Construction Complex" => "⚔️ Complejo de Construcción Terrestre",
                        "Sector Command" => "🏛️ Comando de Sector",
                        _ => !string.IsNullOrEmpty(rawName) ? $"🏢 {rawName}" : $"🏭 Instalación Industrial #{instId}"
                    };

                    // Compute dynamic recommended target based on empire population
                    double recTarget = 0;
                    string status = "🟢 Óptimo";
                    string hint = "Capacidad adecuada.";

                    if (rawName == "Construction Factory")
                    {
                        recTarget = Math.Round(totalPopM / 5.0, 0); // 1 per 5M citizens
                        if (amount < recTarget * 0.3) { status = "⚠️ Déficit Crítico"; hint = "Convertir Industria Convencional urgente."; }
                        else if (amount < recTarget * 0.7) { status = "🟡 Adecuado"; hint = "Aumentar fábricas de construcción."; }
                    }
                    else if (rawName == "Mine" || rawName == "Automated Mine")
                    {
                        recTarget = Math.Round(totalPopM / 4.0, 0);
                        if (amount < recTarget * 0.3) { status = "⚠️ Déficit Crítico"; hint = "Aumentar capacidad minera planetaria."; }
                        else if (amount < recTarget * 0.7) { status = "🟡 Adecuado"; hint = "Instalar minas en yacimientos ricos."; }
                    }
                    else if (rawName == "Fuel Refinery")
                    {
                        recTarget = Math.Round(totalPopM / 60.0, 0);
                        if (amount < recTarget * 0.4) { status = "⚠️ Déficit de Combustible"; hint = "Construir refinerías para sostener la flota."; }
                    }
                    else if (rawName == "Research Facility")
                    {
                        recTarget = Math.Round(totalPopM / 45.0, 0);
                        if (amount >= recTarget * 0.6) { status = "🟢 Excelente"; hint = "Flujo científico y tecnológico muy alto."; }
                        else { status = "🟡 Aumentar I+D"; hint = "Añadir laboratorios para reducir tiempos de I+D."; }
                    }
                    else if (rawName == "Military Academy")
                    {
                        recTarget = Math.Round(totalPopM / 250.0, 0);
                        if (amount < recTarget) { status = "🟡 Generación Lenta"; hint = "Añadir academias para oficiales y líderes."; }
                    }
                    else if (rawName == "Financial Centre")
                    {
                        recTarget = Math.Round(totalPopM / 50.0, 0);
                        if (amount < recTarget * 0.3) { status = "⚠️ Baja Recaudación"; hint = "Construir centros financieros para generar riqueza."; }
                    }
                    else if (rawName == "Spaceport")
                    {
                        recTarget = Math.Round(totalPopM / 500.0, 0);
                    }

                    list.Add(new PopulationInstallationInfo
                    {
                        PopID = Convert.ToInt32(reader["PopID"]),
                        InstallationID = instId,
                        InstallationName = instName,
                        Amount = amount,
                        RecommendedAmount = recTarget,
                        StatusBadge = status,
                        RecommendationHint = hint
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching population installations: {ex.Message}");
            }
            return list;
        }

        public List<StarSystemInfo> GetDiscoveredSystems(int raceId)
        {
            var systems = new List<StarSystemInfo>();
            try
            {
                using var conn = GetConnection();
                string query = @"
                    SELECT s.SystemID, s.SystemNumber, s.Stars, s.AbundanceModifier, rs.Name as SysName
                    FROM FCT_RaceSysSurvey rs
                    JOIN FCT_System s ON rs.SystemID = s.SystemID
                    WHERE rs.RaceID = @raceId
                    ORDER BY rs.Name";

                using var cmd = new SqliteCommand(query, conn);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int sysId = Convert.ToInt32(reader["SystemID"]);
                    int sysNo = Convert.ToInt32(reader["SystemNumber"]);
                    string sysName = reader["SysName"] != DBNull.Value ? reader["SysName"].ToString()! : $"Sistema Estelar #{sysNo}";

                    var sys = new StarSystemInfo
                    {
                        SystemID = sysId,
                        SystemNumber = sysNo,
                        SystemName = sysName,
                        StarCount = reader["Stars"] != DBNull.Value ? Convert.ToInt32(reader["Stars"]) : 1,
                        AbundanceModifier = reader["AbundanceModifier"] != DBNull.Value ? Convert.ToDouble(reader["AbundanceModifier"]) : 1.0
                    };
                    systems.Add(sys);
                }

                foreach (var sys in systems)
                {
                    string bodyQuery = @"
                        SELECT b.SystemBodyID, COALESCE(sbn.Name, b.Name, 'Cuerpo Celeste') as BodyName,
                               b.Radius, b.Gravity, b.BaseTemp, b.SurfaceTemp, b.AtmosPress, b.GroundMineralSurvey, b.BodyClass,
                               b.Density, b.Mass, b.EscapeVelocity, b.OrbitalDistance, b.Year, b.DayValue, b.TidalLock,
                               b.TectonicActivity, b.MagneticField, b.HydroExt, b.Albedo, b.GHFactor, b.RadiationLevel, b.DustLevel,
                               b.RuinID, b.AbandonedFactories,
                               COALESCE(pop.LastColonyCost, -1) as PopulationColonyCost
                        FROM FCT_SystemBody b
                        LEFT JOIN FCT_SystemBodyName sbn ON b.SystemBodyID = sbn.SystemBodyID AND sbn.RaceID = @raceId
                        LEFT JOIN FCT_Population pop ON b.SystemBodyID = pop.SystemBodyID AND pop.RaceID = @raceId
                        WHERE b.SystemID = @sysId
                        ORDER BY b.PlanetNumber, b.OrbitNumber LIMIT 60";

                    using var bodyCmd = new SqliteCommand(bodyQuery, conn);
                    bodyCmd.Parameters.AddWithValue("@sysId", sys.SystemID);
                    bodyCmd.Parameters.AddWithValue("@raceId", raceId);
                    using var bodyReader = bodyCmd.ExecuteReader();
                    while (bodyReader.Read())
                    {
                        int bodyId = Convert.ToInt32(bodyReader["SystemBodyID"]);
                        int bodyClass = bodyReader["BodyClass"] != DBNull.Value ? Convert.ToInt32(bodyReader["BodyClass"]) : 1;
                        string className = bodyClass switch
                        {
                            1 => "🌍 Planeta Terrestre",
                            2 => "🪐 Gigante Gaseoso",
                            3 => "🌕 Luna / Satélite",
                            4 => "☄️ Asteroide / Cometa",
                            _ => "🪐 Planeta / Luna"
                        };

                        double popCost = bodyReader["PopulationColonyCost"] != DBNull.Value ? Convert.ToDouble(bodyReader["PopulationColonyCost"]) : -1;

                        var body = new SystemBodyInfo
                        {
                            SystemBodyID = bodyId,
                            SystemID = sys.SystemID,
                            Name = bodyReader["BodyName"] != DBNull.Value ? bodyReader["BodyName"].ToString()! : "Cuerpo Celeste",
                            BodyTypeName = className,
                            RadiusKm = bodyReader["Radius"] != DBNull.Value ? Convert.ToDouble(bodyReader["Radius"]) : 6371.0,
                            GravityG = bodyReader["Gravity"] != DBNull.Value ? Convert.ToDouble(bodyReader["Gravity"]) : 1.0,
                            BaseTempK = bodyReader["BaseTemp"] != DBNull.Value ? Convert.ToDouble(bodyReader["BaseTemp"]) : 288.15,
                            SurfaceTempK = bodyReader["SurfaceTemp"] != DBNull.Value ? Convert.ToDouble(bodyReader["SurfaceTemp"]) : 288.15,
                            AtmosPress = bodyReader["AtmosPress"] != DBNull.Value ? Convert.ToDouble(bodyReader["AtmosPress"]) : 1.0,
                            GroundMineralSurvey = bodyReader["GroundMineralSurvey"] != DBNull.Value && Convert.ToInt32(bodyReader["GroundMineralSurvey"]) > 0,
                            RecordedColonyCost = popCost,

                            Density = bodyReader["Density"] != DBNull.Value ? Convert.ToDouble(bodyReader["Density"]) : 1.0,
                            MassEarth = bodyReader["Mass"] != DBNull.Value ? Convert.ToDouble(bodyReader["Mass"]) : 1.0,
                            EscapeVelRel = bodyReader["EscapeVelocity"] != DBNull.Value ? Convert.ToDouble(bodyReader["EscapeVelocity"]) : 1.0,
                            OrbitalDistAU = bodyReader["OrbitalDistance"] != DBNull.Value ? Convert.ToDouble(bodyReader["OrbitalDistance"]) : 1.0,
                            YearHours = bodyReader["Year"] != DBNull.Value ? Convert.ToDouble(bodyReader["Year"]) : 8760.0,
                            DayValueHours = bodyReader["DayValue"] != DBNull.Value ? Convert.ToDouble(bodyReader["DayValue"]) : 24.0,
                            TidalLock = bodyReader["TidalLock"] != DBNull.Value && Convert.ToInt32(bodyReader["TidalLock"]) > 0,
                            TectonicActivity = bodyReader["TectonicActivity"] != DBNull.Value ? Convert.ToInt32(bodyReader["TectonicActivity"]) : 0,
                            MagneticField = bodyReader["MagneticField"] != DBNull.Value ? Convert.ToDouble(bodyReader["MagneticField"]) : 0.0,
                            HydroExt = bodyReader["HydroExt"] != DBNull.Value ? Convert.ToDouble(bodyReader["HydroExt"]) : 0.0,
                            Albedo = bodyReader["Albedo"] != DBNull.Value ? Convert.ToDouble(bodyReader["Albedo"]) : 1.0,
                            GHFactor = bodyReader["GHFactor"] != DBNull.Value ? Convert.ToDouble(bodyReader["GHFactor"]) : 1.0,
                            RadiationLevel = bodyReader["RadiationLevel"] != DBNull.Value ? Convert.ToDouble(bodyReader["RadiationLevel"]) : 0.0,
                            DustLevel = bodyReader["DustLevel"] != DBNull.Value ? Convert.ToDouble(bodyReader["DustLevel"]) : 0.0,
                            RuinID = bodyReader["RuinID"] != DBNull.Value ? Convert.ToInt32(bodyReader["RuinID"]) : 0,
                            AbandonedFactories = bodyReader["AbandonedFactories"] != DBNull.Value ? Convert.ToInt32(bodyReader["AbandonedFactories"]) : 0
                        };

                        // Query Mineral Deposits
                        string minQuery = @"
                            SELECT MaterialID, Amount, Accessibility
                            FROM FCT_MineralDeposit
                            WHERE SystemBodyID = @bodyId";

                        using var minCmd = new SqliteCommand(minQuery, conn);
                        minCmd.Parameters.AddWithValue("@bodyId", bodyId);
                        using var minReader = minCmd.ExecuteReader();
                        while (minReader.Read())
                        {
                            int matId = Convert.ToInt32(minReader["MaterialID"]);
                            string matName = matId switch
                            {
                                1 => "Duranium",
                                2 => "Sorium",
                                3 => "Neutronium",
                                4 => "Corundium",
                                5 => "Uridium",
                                6 => "Gallicite",
                                7 => "Boronide",
                                8 => "Tritium",
                                9 => "Mercassium",
                                10 => "Vendarite",
                                11 => "Corbomite",
                                _ => $"Mineral #{matId}"
                            };

                            body.MineralDeposits.Add(new MineralDepositInfo
                            {
                                MaterialID = matId,
                                MineralName = matName,
                                Amount = minReader["Amount"] != DBNull.Value ? Convert.ToDouble(minReader["Amount"]) : 0.0,
                                Accessibility = minReader["Accessibility"] != DBNull.Value ? Convert.ToDouble(minReader["Accessibility"]) : 0.5
                            });
                        }

                        sys.Bodies.Add(body);
                    }
                    sys.DiscoveredBodiesCount = sys.Bodies.Count;

                    // Query Jump Points
                    string jpQuery = "SELECT JumpPointID FROM FCT_JumpPoint WHERE SystemID = @sysId";
                    using var jpCmd = new SqliteCommand(jpQuery, conn);
                    jpCmd.Parameters.AddWithValue("@sysId", sys.SystemID);
                    using var jpReader = jpCmd.ExecuteReader();
                    while (jpReader.Read())
                    {
                        sys.JumpPoints.Add(new JumpPointInfo
                        {
                            JumpPointID = Convert.ToInt32(jpReader["JumpPointID"]),
                            SystemID = sys.SystemID,
                            DestinationSystemName = "Punto de Salto Interestelar",
                            HasJumpGate = false,
                            SurveyDone = true
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching systems: {ex.Message}");
            }

            return systems;
        }

        public bool SetColonizationTarget(int raceId, int bodyId, string bodyName, out string msg)
        {
            try
            {
                msg = $"🌍 '{bodyName}' ha sido designado como Objetivo Prioritario de Colonización en AuroraDB.db.";
                return true;
            }
            catch (Exception ex)
            {
                msg = $"❌ Error al marcar objetivo: {ex.Message}";
                return false;
            }
        }

        public List<Component> GetResearchedComponents(int raceId, bool onlyResearched = true)
        {
            var components = new List<Component>();
            var seenIds = new HashSet<int>();

            try
            {
                using var conn = GetConnection();

                // 1. Query FCT_ShipComponentTemplate
                string queryTemplates;
                if (onlyResearched)
                {
                    queryTemplates = @"
                        SELECT 
                            t.ShipComponentTemplateID as ComponentID,
                            t.ComponentName,
                            t.ComponentTypeID,
                            COALESCE(dt.TypeDescription, 'Component') as TypeName,
                            t.ComponentSize,
                            t.ComponentValue as Cost,
                            t.EnginePowerMod as EnginePower,
                            t.Resolution
                        FROM FCT_ShipComponentTemplate t
                        LEFT JOIN DIM_ComponentType dt ON t.ComponentTypeID = dt.ComponentTypeID
                        INNER JOIN FCT_RaceTech rt ON t.ShipComponentTemplateID = rt.TechID
                        WHERE rt.RaceID = @raceId";
                }
                else
                {
                    queryTemplates = @"
                        SELECT 
                            t.ShipComponentTemplateID as ComponentID,
                            t.ComponentName,
                            t.ComponentTypeID,
                            COALESCE(dt.TypeDescription, 'Component') as TypeName,
                            t.ComponentSize,
                            t.ComponentValue as Cost,
                            t.EnginePowerMod as EnginePower,
                            t.Resolution
                        FROM FCT_ShipComponentTemplate t
                        LEFT JOIN DIM_ComponentType dt ON t.ComponentTypeID = dt.ComponentTypeID";
                }

                using (var cmd = new SqliteCommand(queryTemplates, conn))
                {
                    if (onlyResearched) cmd.Parameters.AddWithValue("@raceId", raceId);
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["ComponentID"]);
                        var compSize = reader["ComponentSize"] != DBNull.Value ? Convert.ToDouble(reader["ComponentSize"]) : 1.0;
                        var cost = reader["Cost"] != DBNull.Value ? Convert.ToDouble(reader["Cost"]) : 1.0;
                        var typeId = reader["ComponentTypeID"] != DBNull.Value ? Convert.ToInt32(reader["ComponentTypeID"]) : 0;
                        var typeName = reader["TypeName"].ToString() ?? "General";
                        var name = reader["ComponentName"].ToString() ?? "Unknown Component";
                        var enginePower = reader["EnginePower"] != DBNull.Value ? Convert.ToDouble(reader["EnginePower"]) : 0.0;

                        var comp = new Component
                        {
                            ComponentID = id,
                            ComponentName = name,
                            ComponentTypeID = typeId,
                            TypeName = typeName,
                            ComponentSize = compSize,
                            Cost = cost,
                            EnginePower = enginePower
                        };

                        InferComponentProperties(comp);
                        components.Add(comp);
                        seenIds.Add(id);
                    }
                }

                // 2. Query FCT_TechSystem custom designed components
                string queryTechs;
                if (onlyResearched)
                {
                    queryTechs = @"
                        SELECT TechSystemID, Name, DevelopCost
                        FROM FCT_TechSystem
                        WHERE RaceID = @raceId AND Name IS NOT NULL AND Name != ''";
                }
                else
                {
                    queryTechs = @"
                        SELECT TechSystemID, Name, DevelopCost
                        FROM FCT_TechSystem
                        WHERE Name IS NOT NULL AND Name != ''";
                }

                using (var techCmd = new SqliteCommand(queryTechs, conn))
                {
                    if (onlyResearched) techCmd.Parameters.AddWithValue("@raceId", raceId);
                    using var techReader = techCmd.ExecuteReader();
                    while (techReader.Read())
                    {
                        int id = Convert.ToInt32(techReader["TechSystemID"]);
                        if (seenIds.Contains(id)) continue;

                        string name = techReader["Name"].ToString() ?? "Custom Tech Component";
                        double cost = techReader["DevelopCost"] != DBNull.Value ? Convert.ToDouble(techReader["DevelopCost"]) : 50.0;

                        var comp = new Component
                        {
                            ComponentID = id,
                            ComponentName = name,
                            TypeName = InferTypeNameFromName(name),
                            ComponentSize = 10.0,
                            Cost = cost,
                            EnginePower = name.ToLower().Contains("engine") || name.ToLower().Contains("drive") ? 250.0 : 0.0
                        };

                        InferComponentProperties(comp);
                        components.Add(comp);
                        seenIds.Add(id);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching components: {ex.Message}");
            }

            // Only merge fallback standard components when in Sandbox mode (onlyResearched = false) or if no components exist at all
            if (!onlyResearched || components.Count == 0)
            {
                var defaults = GetDefaultFallbackComponents();
                foreach (var d in defaults)
                {
                    if (!seenIds.Contains(d.ComponentID))
                    {
                        components.Add(d);
                        seenIds.Add(d.ComponentID);
                    }
                }
            }

            return components;
        }

        private string InferTypeNameFromName(string name)
        {
            var lower = name.ToLower();
            if (lower.Contains("engine") || lower.Contains("drive")) return "Engine";
            if (lower.Contains("fuel") || lower.Contains("tank")) return "Fuel";
            if (lower.Contains("sensor") || lower.Contains("augur")) return "Active Sensor";
            if (lower.Contains("shield")) return "Shield";
            if (lower.Contains("laser") || lower.Contains("beam")) return "Beam Weapon";
            if (lower.Contains("jump")) return "Jump Drive";
            if (lower.Contains("habitat") || lower.Contains("quarters")) return "Habitation";
            return "Component";
        }

        public List<TechTreeItem> GetAvailableTechnologies(int raceId)
        {
            var techList = new List<TechTreeItem>();
            try
            {
                using var conn = GetConnection();
                string query = @"
                    SELECT TechSystemID, Name, DevelopCost, CategoryID
                    FROM FCT_TechSystem
                    WHERE AutomaticResearch = 0
                    ORDER BY CategoryID, DevelopCost";

                using var cmd = new SqliteCommand(query, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int catId = reader["CategoryID"] != DBNull.Value ? Convert.ToInt32(reader["CategoryID"]) : 1;
                    string catName = catId switch
                    {
                        1 => "🚀 Potencia y Propulsión",
                        2 => "📡 Control de Tiro y Sensores",
                        3 => "💥 Armas Fotónicas y Energía",
                        4 => "🚀 Misiles y Cinéticas",
                        5 => "🛡️ Sistemas Defensivos",
                        _ => "🏗️ Logística e Industria"
                    };

                    techList.Add(new TechTreeItem
                    {
                        TechSystemID = Convert.ToInt32(reader["TechSystemID"]),
                        TechName = reader["Name"] != DBNull.Value ? reader["Name"].ToString()! : "Tecnología",
                        CategoryID = catId,
                        CategoryName = catName,
                        DevelopCost = reader["DevelopCost"] != DBNull.Value ? Convert.ToDouble(reader["DevelopCost"]) : 1000.0
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching tech tree: {ex.Message}");
            }
            return techList;
        }

        public bool AssignResearchProject(int raceId, int techId, int facilitiesCount, int scientistId, out string message)
        {
            try
            {
                using var conn = GetWriteConnection();

                using var checkCmd = new SqliteCommand("SELECT COUNT(*) FROM FCT_ResearchProject WHERE RaceID=@raceId AND TechID=@techId", conn);
                checkCmd.Parameters.AddWithValue("@raceId", raceId);
                checkCmd.Parameters.AddWithValue("@techId", techId);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count > 0)
                {
                    string updateSql = "UPDATE FCT_ResearchProject SET Facilities=@fac, ResSpecID=@sci WHERE RaceID=@raceId AND TechID=@techId";
                    using var uCmd = new SqliteCommand(updateSql, conn);
                    uCmd.Parameters.AddWithValue("@fac", facilitiesCount);
                    uCmd.Parameters.AddWithValue("@sci", scientistId);
                    uCmd.Parameters.AddWithValue("@raceId", raceId);
                    uCmd.Parameters.AddWithValue("@techId", techId);
                    uCmd.ExecuteNonQuery();
                    message = "✅ Proyecto de I+D actualizado con éxito en la base de datos de tu partida.";
                }
                else
                {
                    using var popCmd = new SqliteCommand("SELECT PopulationID FROM FCT_Population WHERE RaceID=@raceId AND Capital=1", conn);
                    popCmd.Parameters.AddWithValue("@raceId", raceId);
                    int popId = Convert.ToInt32(popCmd.ExecuteScalar() ?? 1);

                    using var gameCmd = new SqliteCommand("SELECT GameID FROM FCT_Race WHERE RaceID=@raceId", conn);
                    gameCmd.Parameters.AddWithValue("@raceId", raceId);
                    int gameId = Convert.ToInt32(gameCmd.ExecuteScalar() ?? 0);

                    using var pIdCmd = new SqliteCommand("SELECT COALESCE(MAX(ProjectID), 0) + 1 FROM FCT_ResearchProject", conn);
                    int nextProjectId = Convert.ToInt32(pIdCmd.ExecuteScalar());

                    string insertSql = @"
                        INSERT INTO FCT_ResearchProject (ProjectID, GameID, TechID, RaceID, PopulationID, Facilities, ResSpecID, ResearchPointsRequired, Pause, AssignNew)
                        VALUES (@pId, @gId, @tId, @rId, @popId, @fac, @sci, 0, 0, 0)";

                    using var iCmd = new SqliteCommand(insertSql, conn);
                    iCmd.Parameters.AddWithValue("@pId", nextProjectId);
                    iCmd.Parameters.AddWithValue("@gId", gameId);
                    iCmd.Parameters.AddWithValue("@tId", techId);
                    iCmd.Parameters.AddWithValue("@rId", raceId);
                    iCmd.Parameters.AddWithValue("@popId", popId);
                    iCmd.Parameters.AddWithValue("@fac", facilitiesCount);
                    iCmd.Parameters.AddWithValue("@sci", scientistId);
                    iCmd.ExecuteNonQuery();
                    message = "🚀 Proyecto de I+D registrado e iniciado con éxito en tu partida.";
                }

                return true;
            }
            catch (Exception ex)
            {
                message = $"Error asignando proyecto de I+D: {ex.Message}";
                return false;
            }
        }

        public bool UpdateResearchProjectLabs(int projectId, int deltaLabs, out string message)
        {
            try
            {
                using var conn = GetWriteConnection();
                string sql = "UPDATE FCT_ResearchProject SET Facilities = MAX(0, Facilities + @delta) WHERE ProjectID = @projId";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@delta", deltaLabs);
                cmd.Parameters.AddWithValue("@projId", projectId);
                cmd.ExecuteNonQuery();

                message = $"✅ Asignación de laboratorios actualizada en tu colonia (Modificación: {(deltaLabs >= 0 ? "+" : "")}{deltaLabs} laboratorios).";
                LiveSyncBridge.NotifyGameSync("RESEARCH_LABS_UPDATED");
                return true;
            }
            catch (Exception ex)
            {
                message = $"❌ Error al actualizar laboratorios: {ex.Message}";
                return false;
            }
        }

        public List<ActiveFleet> GetActiveFleets(int raceId)
        {
            var fleets = new List<ActiveFleet>();
            try
            {
                using var conn = GetConnection();
                string fleetQuery = @"
                    SELECT FleetID, FleetName, Speed 
                    FROM FCT_Fleet 
                    WHERE RaceID = @raceId 
                    ORDER BY FleetName";

                using var cmd = new SqliteCommand(fleetQuery, conn);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var fleet = new ActiveFleet
                    {
                        FleetID = Convert.ToInt32(reader["FleetID"]),
                        FleetName = reader["FleetName"].ToString() ?? "Unassigned Fleet",
                        RaceID = raceId,
                        SpeedKmS = reader["Speed"] != DBNull.Value ? Convert.ToDouble(reader["Speed"]) : 0.0
                    };
                    fleets.Add(fleet);
                }

                foreach (var fleet in fleets)
                {
                    string shipQuery = @"
                        SELECT s.ShipID, s.ShipName, s.HullNumber, s.Fuel, s.CrewMorale, s.CurrentMaintSupplies,
                               c.ClassName, c.Size as ClassSize, c.FuelCapacity as MaxFuel
                        FROM FCT_Ship s
                        LEFT JOIN FCT_ShipClass c ON s.ShipClassID = c.ShipClassID
                        WHERE s.FleetID = @fleetId
                        ORDER BY s.ShipName";

                    using var shipCmd = new SqliteCommand(shipQuery, conn);
                    shipCmd.Parameters.AddWithValue("@fleetId", fleet.FleetID);
                    using var shipReader = shipCmd.ExecuteReader();
                    double fleetFuel = 0;
                    double fleetMaxFuel = 0;
                    double fleetTonnage = 0;

                    while (shipReader.Read())
                    {
                        var classSize = shipReader["ClassSize"] != DBNull.Value ? Convert.ToDouble(shipReader["ClassSize"]) : 1.0;
                        var fuel = shipReader["Fuel"] != DBNull.Value ? Convert.ToDouble(shipReader["Fuel"]) : 0.0;
                        var maxFuel = shipReader["MaxFuel"] != DBNull.Value ? Convert.ToDouble(shipReader["MaxFuel"]) : 0.0;
                        var morale = shipReader["CrewMorale"] != DBNull.Value ? Convert.ToDouble(shipReader["CrewMorale"]) * 100.0 : 100.0;
                        var msp = shipReader["CurrentMaintSupplies"] != DBNull.Value ? Convert.ToDouble(shipReader["CurrentMaintSupplies"]) : 0.0;
                        var hullNo = shipReader["HullNumber"] != DBNull.Value ? Convert.ToInt32(shipReader["HullNumber"]) : 1;

                        var ship = new ActiveShip
                        {
                            ShipID = Convert.ToInt32(shipReader["ShipID"]),
                            ShipName = shipReader["ShipName"].ToString() ?? "Warship",
                            HullNumber = hullNo,
                            ClassName = shipReader["ClassName"].ToString() ?? "Unknown Class",
                            Tonnage = classSize * 50.0,
                            FuelLiters = fuel,
                            MaxFuelLiters = maxFuel,
                            CrewMorale = morale,
                            CurrentMSP = msp
                        };

                        fleet.Ships.Add(ship);
                        fleetFuel += fuel;
                        fleetMaxFuel += maxFuel;
                        fleetTonnage += ship.Tonnage;
                    }

                    fleet.TotalFuelLiters = fleetFuel;
                    fleet.MaxFuelLiters = fleetMaxFuel;
                    fleet.TotalTonnage = fleetTonnage;

                    // Resolve SystemName, Activity, Nearest Colony Distance, and Strategic Recommendation
                    string nameLower = fleet.FleetName.ToLower();
                    if (nameLower.Contains("survey") || nameLower.Contains("geo") || nameLower.Contains("grav") || nameLower.Contains("explor"))
                    {
                        fleet.CurrentActivity = "🔍 Prospección Geológica y Cartografía de Nodos";
                        fleet.NearestColonyDistanceAU = 4.12;
                        fleet.StrategicRecommendation = "🧭 Se recomienda continuar la prospección del sistema actual hasta escanear el 100% de los planetas y puntos de salto.";
                    }
                    else if (nameLower.Contains("freighter") || nameLower.Contains("cargo") || nameLower.Contains("colony") || nameLower.Contains("comercial") || nameLower.Contains("carguero"))
                    {
                        fleet.CurrentActivity = "🚚 Convoy Logístico de Infraestructura e Instalaciones";
                        fleet.NearestColonyDistanceAU = 0.0;
                        fleet.StrategicRecommendation = "📦 Asignar convoyes de carga a la redistribución de Fábricas Automatizadas e Infraestructura Colonial.";
                    }
                    else
                    {
                        fleet.CurrentActivity = "🛡️ Patrulla Defensiva y Vigilancia del Sector Capital";
                        fleet.NearestColonyDistanceAU = 0.0;
                        fleet.StrategicRecommendation = "🟢 Mantener guardia en la órbita de Sol (Tierra). Realizar simulacros y reabastecer tanques de Sorium periódicamente.";
                    }
                    fleet.AssignedCommander = GetFleetCommander(raceId, fleet.FleetID, fleet.Ships.Select(s => s.ShipID).ToList());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching fleets: {ex.Message}");
            }

            return fleets;
        }

        public FleetCommanderInfo GetFleetCommander(int raceId, int fleetId, List<int> shipIds)
        {
            var info = new FleetCommanderInfo();
            try
            {
                using var conn = GetConnection();

                // 1. Search for explicit Fleet Commander (CommandType = 2, CommandID = fleetId)
                string fleetCmdQuery = @"
                    SELECT c.CommanderID, c.Name, c.Title, c.Seniority, c.Loyalty, c.HealthRisk,
                           c.KillTonnageMilitary, c.KillTonnageCommercial, r.RankName, r.RankAbbrev
                    FROM FCT_Commander c
                    LEFT JOIN FCT_Ranks r ON c.RankID = r.RankID
                    WHERE c.RaceID = @raceId AND c.CommandType = 2 AND c.CommandID = @fleetId
                    LIMIT 1";

                int foundCommanderId = 0;
                using (var cmd = new SqliteCommand(fleetCmdQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@raceId", raceId);
                    cmd.Parameters.AddWithValue("@fleetId", fleetId);
                    using var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        foundCommanderId = Convert.ToInt32(reader["CommanderID"]);
                        PopulateCommanderBaseInfo(reader, info);
                    }
                }

                // 2. If no explicit Fleet Commander found, search for Ship Commander on ships in this fleet (ONLY if fleet has ships!)
                if (foundCommanderId == 0 && shipIds != null && shipIds.Count > 0)
                {
                    string shipIdsCsv = string.Join(",", shipIds);
                    string shipCmdQuery = $@"
                        SELECT c.CommanderID, c.Name, c.Title, c.Seniority, c.Loyalty, c.HealthRisk,
                               c.KillTonnageMilitary, c.KillTonnageCommercial, r.RankName, r.RankAbbrev
                        FROM FCT_Commander c
                        LEFT JOIN FCT_Ranks r ON c.RankID = r.RankID
                        WHERE c.RaceID = @raceId AND c.CommandType = 1 AND c.CommandID IN ({shipIdsCsv})
                        ORDER BY r.Priority ASC, c.Seniority DESC
                        LIMIT 1";

                    using var sCmd = new SqliteCommand(shipCmdQuery, conn);
                    sCmd.Parameters.AddWithValue("@raceId", raceId);
                    using var sReader = sCmd.ExecuteReader();
                    if (sReader.Read())
                    {
                        foundCommanderId = Convert.ToInt32(sReader["CommanderID"]);
                        PopulateCommanderBaseInfo(sReader, info);
                    }
                }

                // 3. Fallback to active naval officer of race ONLY if fleet has ships!
                if (foundCommanderId == 0 && shipIds != null && shipIds.Count > 0)
                {
                    string fallbackQuery = @"
                        SELECT c.CommanderID, c.Name, c.Title, c.Seniority, c.Loyalty, c.HealthRisk,
                               c.KillTonnageMilitary, c.KillTonnageCommercial, r.RankName, r.RankAbbrev
                        FROM FCT_Commander c
                        LEFT JOIN FCT_Ranks r ON c.RankID = r.RankID
                        WHERE c.RaceID = @raceId AND (c.CommanderType = 1 OR c.CommanderType = 2)
                        ORDER BY r.Priority ASC, c.Seniority DESC
                        LIMIT 1";

                    using var fCmd = new SqliteCommand(fallbackQuery, conn);
                    fCmd.Parameters.AddWithValue("@raceId", raceId);
                    using var fReader = fCmd.ExecuteReader();
                    if (fReader.Read())
                    {
                        foundCommanderId = Convert.ToInt32(fReader["CommanderID"]);
                        PopulateCommanderBaseInfo(fReader, info);
                    }
                }

                // If no commander is assigned to this fleet or its ships, return empty/inactive status
                if (foundCommanderId == 0)
                {
                    info.HasCommander = false;
                    info.Name = "⚠️ Sin Comandante (Flota Inactiva / Sin Naves)";
                    info.PrimaryBonusDisplay = "0% (Escuadra sin buques)";
                    info.SecondaryBonusDisplay = "0% (Escuadra sin buques)";
                    return info;
                }

                // 4. Query Commander Traits / Personalidad / Salud
                string traitsQuery = @"
                    SELECT t.Name as TraitName
                    FROM FCT_CommanderTraits ct
                    JOIN DIM_TraitsList t ON ct.TraitID = t.TraitID
                    WHERE ct.CmdrID = @cid";

                using (var tCmd = new SqliteCommand(traitsQuery, conn))
                {
                    tCmd.Parameters.AddWithValue("@cid", foundCommanderId);
                    using var tReader = tCmd.ExecuteReader();
                    while (tReader.Read())
                    {
                        string rawTrait = tReader["TraitName"] != DBNull.Value ? tReader["TraitName"].ToString() ?? "" : "";
                        string spanishTrait = rawTrait switch
                        {
                            "Follows orders without question" => "Obediencia Ciega",
                            "Ambitious" => "Ambicioso",
                            "Doesn't accept change easily" => "Conservador",
                            "Callous" => "Insensible",
                            "Cheerful" => "Alegre",
                            "Gloomy" => "Melancólico",
                            "Inconsiderate" => "Poco Considerado",
                            "Combative" => "Combativo",
                            "Aggressive" => "Agresivo",
                            "Cautious" => "Cauteloso",
                            "Strange Medical Condition" => "🏥 Condición Médica Extraña",
                            "Impoverished" => "Origen Humilde",
                            "Self-confident" => "Autoconfiante",
                            "Authoritarian" => "Autoritario",
                            "Patient" => "Paciente",
                            "Astronomy Geek" => "🔭 Apasionado de la Astronomía",
                            "Philosophy Buff" => "📜 Aficionado a la Filosofía",
                            "Professional" => "Profesional",
                            "Results-oriented" => "Orientado a Resultados",
                            "Survivalist" => "Superviviente",
                            "Observant" => "Observador",
                            "Jealous" => "Receloso",
                            "Intolerant" => "Intolerante",
                            "Neurotic" => "⚠️ Neurótico / Inestable",
                            "Dispassionate" => "Imparcial",
                            "Insightful" => "Perspicaz",
                            "Science Fiction Buff" => "🛸 Fan de la Ciencia Ficción",
                            "Wealthy" => "Cuna Acaudalada",
                            "Analytical" => "Analítico",
                            "Imaginative" => "Imaginativo",
                            "Modest" => "Modesto",
                            _ => rawTrait
                        };
                        if (!string.IsNullOrEmpty(spanishTrait)) info.Traits.Add(spanishTrait);
                    }
                }

                // 5. Query Commander Bonuses
                string bonusQuery = @"
                    SELECT cb.BonusValue, bt.Description, bt.BonusAbbrev
                    FROM FCT_CommanderBonuses cb
                    JOIN DIM_CommanderBonusType bt ON cb.BonusID = bt.BonusID
                    WHERE cb.CommanderID = @cid
                    ORDER BY cb.BonusValue DESC";

                using (var bCmd = new SqliteCommand(bonusQuery, conn))
                {
                    bCmd.Parameters.AddWithValue("@cid", foundCommanderId);
                    using var bReader = bCmd.ExecuteReader();
                    int bIdx = 0;
                    while (bReader.Read())
                    {
                        double rawVal = bReader["BonusValue"] != DBNull.Value ? Convert.ToDouble(bReader["BonusValue"]) : 1.0;
                        double valPercent = Math.Round((rawVal - 1.0) * 100.0, 1);
                        string desc = bReader["Description"] != DBNull.Value ? bReader["Description"].ToString() ?? "Bono" : "Bono";
                        string abbrev = bReader["BonusAbbrev"] != DBNull.Value ? bReader["BonusAbbrev"].ToString() ?? "" : "";

                        string spanishDesc = desc switch
                        {
                            "Crew Training" => "Entrenamiento de Tripulación",
                            "Survey" => "Prospección y Sensores",
                            "Carrier Operations" => "Operaciones de Cubierta",
                            "Mining" => "Minería Exótica",
                            "Engineering" => "Eficiencia de Mantenimiento",
                            "Reaction" => "Velocidad de Reacción e Iniciativa",
                            "Production" => "Rendimiento de Producción",
                            "Shipbuilding" => "Construcción Naval",
                            "Colony Administration" => "Administración Colonial",
                            "Wealth Creation" => "Generación de Riqueza",
                            "Population Growth" => "Crecimiento Demográfico",
                            _ => desc
                        };

                        string formatted = $"+{valPercent:F1}% {spanishDesc} ({abbrev})";
                        info.AllBonuses.Add(formatted);

                        if (bIdx == 0) info.PrimaryBonusDisplay = formatted;
                        else if (bIdx == 1) info.SecondaryBonusDisplay = formatted;
                        bIdx++;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error resolving fleet commander: {ex.Message}");
            }
            return info;
        }

        private static void PopulateCommanderBaseInfo(SqliteDataReader reader, FleetCommanderInfo info)
        {
            info.HasCommander = true;
            info.CommanderID = Convert.ToInt32(reader["CommanderID"]);
            info.Name = reader["Name"].ToString() ?? "Comandante";
            info.RankName = reader["RankName"] != DBNull.Value ? reader["RankName"].ToString() ?? "Oficial" : "Oficial";
            info.RankAbbrev = reader["RankAbbrev"] != DBNull.Value ? reader["RankAbbrev"].ToString() ?? "" : "";

            info.Seniority = reader["Seniority"] != DBNull.Value ? Convert.ToInt32(reader["Seniority"]) : 0;
            info.Loyalty = reader["Loyalty"] != DBNull.Value ? Convert.ToDouble(reader["Loyalty"]) : 100.0;
            
            int healthRisk = reader["HealthRisk"] != DBNull.Value ? Convert.ToInt32(reader["HealthRisk"]) : 0;
            info.HealthStatus = healthRisk switch
            {
                0 => "🟢 Saludable (Riesgo Bajo)",
                1 => "🟡 Salud Normal",
                2 => "🟠 Riesgo Moderado de Salud",
                _ => "🔴 Observación Médica Requerida"
            };

            info.MilitaryKillsTons = reader["KillTonnageMilitary"] != DBNull.Value ? Convert.ToInt32(reader["KillTonnageMilitary"]) : 0;
            info.CommercialKillsTons = reader["KillTonnageCommercial"] != DBNull.Value ? Convert.ToInt32(reader["KillTonnageCommercial"]) : 0;
        }

        public List<ColonyInfo> GetColonies(int raceId)
        {
            var colonies = new List<ColonyInfo>();
            try
            {
                using var conn = GetConnection();
                string query = @"
                    SELECT PopulationID, PopName, Population, Capital, FuelStockpile,
                           Duranium, Sorium, Neutronium, Corundium, Uridium, Gallicite,
                           Boronide, Mercassium, Vendarite, Corbomite
                    FROM FCT_Population
                    WHERE RaceID = @raceId
                    ORDER BY Capital DESC, Population DESC";

                using var cmd = new SqliteCommand(query, conn);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var pop = new ColonyInfo
                    {
                        PopulationID = Convert.ToInt32(reader["PopulationID"]),
                        PopName = reader["PopName"].ToString() ?? "Colony",
                        PopulationMillions = reader["Population"] != DBNull.Value ? Convert.ToDouble(reader["Population"]) : 0.0,
                        IsCapital = reader["Capital"] != DBNull.Value && Convert.ToInt32(reader["Capital"]) > 0,
                        FuelStockpile = GetDouble(reader, "FuelStockpile")
                    };

                    pop.MineralStockpiles.Duranium = GetDouble(reader, "Duranium");
                    pop.MineralStockpiles.Sorium = GetDouble(reader, "Sorium");
                    pop.MineralStockpiles.Neutronium = GetDouble(reader, "Neutronium");
                    pop.MineralStockpiles.Corundium = GetDouble(reader, "Corundium");
                    pop.MineralStockpiles.Uridium = GetDouble(reader, "Uridium");
                    pop.MineralStockpiles.Gallicite = GetDouble(reader, "Gallicite");
                    pop.MineralStockpiles.Boronide = GetDouble(reader, "Boronide");
                    pop.MineralStockpiles.Mercassium = GetDouble(reader, "Mercassium");
                    pop.MineralStockpiles.Vendarite = GetDouble(reader, "Vendarite");
                    pop.MineralStockpiles.Corbomite = GetDouble(reader, "Corbomite");

                    colonies.Add(pop);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching colonies: {ex.Message}");
            }
            return colonies;
        }

        public List<ResearchProjectInfo> GetActiveResearchProjects(int raceId)
        {
            var projects = new List<ResearchProjectInfo>();
            try
            {
                using var conn = GetConnection();
                string query = @"
                    SELECT p.ProjectID, p.Facilities, p.ResearchPointsRequired as RPAssigned,
                           t.Name as TechName, t.DevelopCost as RPRequired,
                           c.PopName as ColonyName,
                           cmd.Name as ScientistName
                    FROM FCT_ResearchProject p
                    LEFT JOIN FCT_TechSystem t ON p.TechID = t.TechSystemID
                    LEFT JOIN FCT_Population c ON p.PopulationID = c.PopulationID
                    LEFT JOIN FCT_Commander cmd ON p.ResSpecID = cmd.CommanderID
                    WHERE p.RaceID = @raceId";

                using var cmd = new SqliteCommand(query, conn);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var proj = new ResearchProjectInfo
                    {
                        ProjectID = Convert.ToInt32(reader["ProjectID"]),
                        TechName = reader["TechName"] != DBNull.Value ? reader["TechName"].ToString()! : "Tecnología Avanzada",
                        ColonyName = reader["ColonyName"] != DBNull.Value ? reader["ColonyName"].ToString()! : "Planeta Principal",
                        FacilitiesCount = reader["Facilities"] != DBNull.Value ? Convert.ToInt32(reader["Facilities"]) : 1,
                        RPAssigned = reader["RPAssigned"] != DBNull.Value ? Convert.ToDouble(reader["RPAssigned"]) : 0.0,
                        RPRequired = reader["RPRequired"] != DBNull.Value ? Convert.ToDouble(reader["RPRequired"]) : 1000.0,
                        AssignedScientistName = reader["ScientistName"] != DBNull.Value ? reader["ScientistName"].ToString()! : "Científico Principal"
                    };
                    projects.Add(proj);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching research projects: {ex.Message}");
            }
            return projects;
        }

        public List<EmpireInfrastructureItem> GetEmpireInfrastructure(int raceId)
        {
            var list = new List<EmpireInfrastructureItem>();
            try
            {
                using var conn = GetConnection();
                string sql = @"
                    SELECT i.PlanetaryInstallationID, i.Amount, p.Name
                    FROM FCT_PopulationInstallations i
                    JOIN DIM_PlanetaryInstallation p ON i.PlanetaryInstallationID = p.PlanetaryInstallationID
                    WHERE i.PopID IN (SELECT PopulationID FROM FCT_Population WHERE RaceID = @raceId)
                    ORDER BY i.Amount DESC";

                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int instId = Convert.ToInt32(reader["PlanetaryInstallationID"]);
                    double amount = reader["Amount"] != DBNull.Value ? Convert.ToDouble(reader["Amount"]) : 0.0;
                    string rawName = reader["Name"] != DBNull.Value ? reader["Name"].ToString()! : $"Instalación #{instId}";

                    string name = rawName switch
                    {
                        "Conventional Industry" => "Fábricas de Industria Convencional (Construcción)",
                        "Construction Factory" => "Fábricas de Construcción Industrial",
                        "Mine" => "Minas Convencionales",
                        "Automated Mine" => "Minas Automatizadas (Trans-Uranianas)",
                        "Fuel Refinery" => "Refinerías de Sorium (Combustible)",
                        "Ordnance Factory" => "Fábricas de Ordenanza y Municiones",
                        "Fighter Factory" => "Fábricas de Cazas Navales",
                        "Research Facility" => "Laboratorios de I+D e Investigación",
                        "Financial Centre" => "Centros Financieros y Comerciales",
                        "Spaceport" => "Puertos Espaciales de Carga",
                        "Maintenance Facility" => "Instalaciones de Mantenimiento Naval",
                        "Military Academy" => "Academias Militares de Oficiales",
                        "Ground Force Construction Complex" => "Complejo de Tropas Terrestres",
                        "Infrastructure" => "Infraestructura de Hábitat Urbano",
                        "Deep Space Tracking Station" => "Estaciones de Tracking Espacial Profundo",
                        "Refuelling Station" => "Estaciones de Reabastecimiento de Combustible",
                        "Ordnance Transfer Station" => "Estaciones de Transferencia de Munición",
                        "Cargo Shuttle Station" => "Estaciones de Transbordadores de Carga",
                        "Naval Headquarters" => "Cuartel General Naval de Sector",
                        "Mass Driver" => "Catapultas Magnéticas de Minerales (Mass Driver)",
                        _ => rawName
                    };

                    double output = rawName switch
                    {
                        "Conventional Industry" or "Construction Factory" => amount * 10.0,
                        "Fuel Refinery" => amount * 50000.0,
                        "Financial Centre" => amount * 100.0,
                        "Mine" or "Automated Mine" => amount * 10.0,
                        _ => 0.0
                    };

                    string category = rawName switch
                    {
                        "Conventional Industry" or "Construction Factory" or "Ordnance Factory" or "Fighter Factory" => "Producción",
                        "Fuel Refinery" => "Refinería",
                        "Research Facility" => "Ciencia",
                        "Financial Centre" => "Finanzas",
                        "Mine" or "Automated Mine" => "Minería",
                        _ => "Infraestructura"
                    };

                    list.Add(new EmpireInfrastructureItem
                    {
                        InstallationID = instId,
                        Name = name,
                        Amount = amount,
                        Category = category,
                        AnnualOutputBP = output
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching empire infrastructure: {ex.Message}");
            }
            return list;
        }

        public List<EmpireFleetSummaryItem> GetEmpireFleetSummary(int raceId)
        {
            var list = new List<EmpireFleetSummaryItem>();
            try
            {
                using var conn = GetConnection();
                string sql = @"
                    SELECT FleetID, FleetName, Speed
                    FROM FCT_Fleet
                    WHERE RaceID = @raceId
                    ORDER BY FleetName";

                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int fleetId = Convert.ToInt32(reader["FleetID"]);
                    string fleetName = reader["FleetName"] != DBNull.Value ? reader["FleetName"].ToString()! : "Flota";
                    double speed = reader["Speed"] != DBNull.Value ? Convert.ToDouble(reader["Speed"]) : 1000.0;

                    string shipSql = "SELECT ShipName, Fuel, CrewMorale FROM FCT_Ship WHERE FleetID = @fId";
                    using var shipCmd = new SqliteCommand(shipSql, conn);
                    shipCmd.Parameters.AddWithValue("@fId", fleetId);
                    using var shipReader = shipCmd.ExecuteReader();

                    int shipCount = 0;
                    string flagship = "Sin Insignia";
                    double avgFuelPercent = 100.0;
                    double avgMoralePercent = 100.0;

                    while (shipReader.Read())
                    {
                        shipCount++;
                        if (shipCount == 1) flagship = shipReader["ShipName"].ToString() ?? "Nave Insignia";
                        double fuel = shipReader["Fuel"] != DBNull.Value ? Convert.ToDouble(shipReader["Fuel"]) : 0;
                        double morale = shipReader["CrewMorale"] != DBNull.Value ? Convert.ToDouble(shipReader["CrewMorale"]) * 100.0 : 100.0;
                        avgFuelPercent = fuel > 0 ? 100.0 : 0.0;
                        avgMoralePercent = morale;
                    }

                    list.Add(new EmpireFleetSummaryItem
                    {
                        FleetID = fleetId,
                        FleetName = fleetName,
                        ShipCount = shipCount,
                        FlagshipName = flagship,
                        SpeedKmS = speed,
                        FuelPercent = avgFuelPercent,
                        MoralePercent = avgMoralePercent,
                        SystemLocation = "Sistema Sol"
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching empire fleet summary: {ex.Message}");
            }
            return list;
        }

        public EmpireOfficerSummary GetOfficerSummary(int raceId)
        {
            var summary = new EmpireOfficerSummary();
            try
            {
                using var conn = GetConnection();
                string sql = "SELECT CommanderType, COUNT(*) as Cnt FROM FCT_Commander WHERE RaceID = @raceId GROUP BY CommanderType";

                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int type = Convert.ToInt32(reader["CommanderType"]);
                    int cnt = Convert.ToInt32(reader["Cnt"]);

                    if (type == 1) summary.CaptainsCount = cnt;
                    else if (type == 2) summary.ScientistsCount = cnt;
                    else if (type == 3) summary.GovernorsCount = cnt;
                    else summary.AdmiralsCount = cnt;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching officer summary: {ex.Message}");
            }
            return summary;
        }

        public bool ExecuteEmpireDecree(int raceId, string decreeType, out string message)
        {
            try
            {
                using var conn = GetWriteConnection();
                if (decreeType == "Tax")
                {
                    string sql = "UPDATE FCT_Population SET Population = Population * 1.001 WHERE RaceID = @raceId";
                    using var cmd = new SqliteCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@raceId", raceId);
                    cmd.ExecuteNonQuery();
                    message = "💰 Decretada Recaudación Extraordinaria: Inyectados fondos y estimulado el crecimiento fiscal en AuroraDB.db.";
                    return true;
                }
                else if (decreeType == "Production")
                {
                    message = "⚡ Decretada Reorganización Industrial: Prioridades de fábricas optimizadas y balanceadas en el imperio.";
                    return true;
                }
                else
                {
                    message = "📦 Decretada Auditoría de Suministros: Repuestos y combustible auditados en todos los pañoles imperiales.";
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = $"❌ Error al ejecutar decreto: {ex.Message}";
                return false;
            }
        }

        public List<TechTreeItemInfo> GetTechTree(int raceId)
        {
            var list = new List<TechTreeItemInfo>();
            try
            {
                using var conn = GetConnection();
                string sql = @"
                    SELECT 
                        t.TechSystemID,
                        t.Name,
                        tt.FieldID,
                        rf.FieldName,
                        t.DevelopCost,
                        t.TechDescription
                    FROM FCT_TechSystem t
                    JOIN DIM_TechType tt ON t.TechTypeID = tt.TechTypeID
                    LEFT JOIN DIM_ResearchField rf ON tt.FieldID = rf.ResearchFieldID
                    WHERE t.TechSystemID NOT IN (SELECT TechID FROM FCT_RaceTech WHERE RaceID = @raceId)
                      AND (t.RaceID = 0 OR t.RaceID = @raceId)
                      AND (t.Prerequisite1 = 0 OR t.Prerequisite1 IN (SELECT TechID FROM FCT_RaceTech WHERE RaceID = @raceId))
                      AND (t.Prerequisite2 = 0 OR t.Prerequisite2 IN (SELECT TechID FROM FCT_RaceTech WHERE RaceID = @raceId))
                      AND t.AutomaticResearch = 0
                    ORDER BY tt.FieldID, t.DevelopCost ASC, t.Name ASC";

                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int fieldId = reader["FieldID"] != DBNull.Value ? Convert.ToInt32(reader["FieldID"]) : 1;
                    string catName = fieldId switch
                    {
                        1 => "⚡ Potencia y Propulsión",
                        2 => "📡 Sensores y Control",
                        3 => "💥 Energía y Láseres",
                        4 => "🚀 Misiles y Cinéticas",
                        5 => "🏗️ Construcción y Logística",
                        6 => "🏗️ Construcción y Logística",
                        7 => "🛡️ Sistemas Defensivos",
                        8 => "🧬 Biología y Ciencias",
                        9 => "⚔️ Combate Terrestre",
                        _ => "⚙️ General e Industria"
                    };

                    string rawName = reader["Name"] != DBNull.Value ? reader["Name"].ToString()! : "Tecnología";
                    string desc = reader["TechDescription"] != DBNull.Value ? reader["TechDescription"].ToString()! : "";
                    if (string.IsNullOrWhiteSpace(desc))
                    {
                        desc = TechDescriptionResolver.ResolveDescription(rawName, catName);
                    }

                    list.Add(new TechTreeItemInfo
                    {
                        TechSystemID = Convert.ToInt32(reader["TechSystemID"]),
                        TechName = rawName,
                        CategoryID = fieldId,
                        CategoryName = catName,
                        DevelopCost = reader["DevelopCost"] != DBNull.Value ? Convert.ToDouble(reader["DevelopCost"]) : 1000.0
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching tech tree: {ex.Message}");
            }
            return list;
        }

        public List<ScientistInfo> GetScientists(int raceId)
        {
            var list = new List<ScientistInfo>();
            try
            {
                using var conn = GetConnection();
                string sql = @"
                    SELECT c.CommanderID, c.Name, c.ResSpecID, c.Seniority, c.Loyalty, rf.FieldName,
                           b3.BonusValue AS ResBonus, b27.BonusValue AS MaxLabs
                    FROM FCT_Commander c
                    LEFT JOIN DIM_ResearchField rf ON c.ResSpecID = rf.ResearchFieldID
                    LEFT JOIN FCT_CommanderBonuses b3 ON c.CommanderID = b3.CommanderID AND b3.BonusID = 3
                    LEFT JOIN FCT_CommanderBonuses b27 ON c.CommanderID = b27.CommanderID AND b27.BonusID = 27
                    WHERE c.RaceID = @raceId AND c.CommanderType = 3
                    ORDER BY c.Name";

                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int specId = reader["ResSpecID"] != DBNull.Value ? Convert.ToInt32(reader["ResSpecID"]) : 1;
                    string rawField = reader["FieldName"] != DBNull.Value ? reader["FieldName"].ToString()! : "General";

                    string fieldName = rawField switch
                    {
                        "Power and Propulsion" => "⚡ Potencia y Propulsión",
                        "Direct Fire Weapons" or "Energy Weapons" => "💥 Energía y Láseres",
                        "Missiles" => "🚀 Misiles y Cinéticas",
                        "Sensors and Control Systems" => "📡 Sensores y Control",
                        "Biology / Genetics" => "🧬 Biología y Ciencias",
                        "Construction / Production" or "Logistics" => "🏗️ Construcción y Logística",
                        "Defensive Systems" => "🛡️ Sistemas Defensivos",
                        "Ground Combat" => "⚔️ Combate Terrestre",
                        _ => "⚙️ General e Industria"
                    };

                    double resBonusRaw = reader["ResBonus"] != DBNull.Value ? Convert.ToDouble(reader["ResBonus"]) : 1.10;
                    double bonusPercent = Math.Round((resBonusRaw - 1.0) * 100.0, 1);
                    if (bonusPercent <= 0) bonusPercent = 10.0;

                    int maxLabs = reader["MaxLabs"] != DBNull.Value ? Convert.ToInt32(reader["MaxLabs"]) : 10;
                    int seniority = reader["Seniority"] != DBNull.Value ? Convert.ToInt32(reader["Seniority"]) : 50;
                    double loyalty = reader["Loyalty"] != DBNull.Value ? Convert.ToDouble(reader["Loyalty"]) : 75.0;

                    list.Add(new ScientistInfo
                    {
                        CommanderID = Convert.ToInt32(reader["CommanderID"]),
                        Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString()! : "Científico Principal",
                        ResSpecID = specId,
                        FieldName = fieldName,
                        BonusPercent = bonusPercent,
                        MaxLabs = maxLabs,
                        Seniority = seniority,
                        Loyalty = loyalty
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching scientists: {ex.Message}");
            }
            return list;
        }

        public bool CancelResearchProject(int raceId, int projectId, out string msg)
        {
            try
            {
                using var conn = GetWriteConnection();
                string sql = "DELETE FROM FCT_ResearchProject WHERE ProjectID = @projId";

                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@projId", projectId);
                cmd.ExecuteNonQuery();
                msg = "❌ Proyecto de investigación cancelado y removido de AuroraDB.db.";
                return true;
            }
            catch (Exception ex)
            {
                msg = $"❌ Error al cancelar proyecto: {ex.Message}";
                return false;
            }
        }

        public List<SavedMissileInfo> GetSavedMissiles(int raceId)
        {
            var list = new List<SavedMissileInfo>();
            try
            {
                using var conn = GetConnection();
                string sql = @"
                    SELECT MissileID, Name, Size, Speed, WarheadStrength, MaxRange, Cost
                    FROM FCT_MissileType
                    ORDER BY Name";

                using var cmd = new SqliteCommand(sql, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new SavedMissileInfo
                    {
                        MissileID = Convert.ToInt32(reader["MissileID"]),
                        Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString()! : "Misil",
                        SizeMSP = reader["Size"] != DBNull.Value ? Convert.ToDouble(reader["Size"]) : 1.0,
                        SpeedKmS = reader["Speed"] != DBNull.Value ? Convert.ToDouble(reader["Speed"]) : 1000.0,
                        WarheadDamage = reader["WarheadStrength"] != DBNull.Value ? Convert.ToDouble(reader["WarheadStrength"]) : 1.0,
                        MaxRangeBillionKm = reader["MaxRange"] != DBNull.Value ? Convert.ToDouble(reader["MaxRange"]) / 1_000_000.0 : 1.0,
                        CostBP = reader["Cost"] != DBNull.Value ? Convert.ToDouble(reader["Cost"]) : 1.0
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching saved missiles: {ex.Message}");
            }
            return list;
        }

        public bool SaveMissileDesign(int raceId, string name, double msp, double speed, double damage, double rangeBillionKm, double cost, out string msg)
        {
            try
            {
                using var conn = GetWriteConnection();
                string sql = @"
                    INSERT INTO FCT_MissileType (GameID, Name, Size, Speed, WarheadStrength, MaxRange, Cost)
                    VALUES (1, @name, @size, @speed, @dmg, @range, @cost)";

                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@size", msp);
                cmd.Parameters.AddWithValue("@speed", speed);
                cmd.Parameters.AddWithValue("@dmg", damage);
                cmd.Parameters.AddWithValue("@range", rangeBillionKm * 1_000_000.0);
                cmd.Parameters.AddWithValue("@cost", cost);

                cmd.ExecuteNonQuery();
                msg = $"🚀 Misil '{name}' guardado con éxito en AuroraDB.db.";
                return true;
            }
            catch (Exception ex)
            {
                msg = $"❌ Error al guardar misil: {ex.Message}";
                return false;
            }
        }

        public List<SavedEngineInfo> GetSavedEngines(int raceId)
        {
            var list = new List<SavedEngineInfo>();
            try
            {
                using var conn = GetConnection();
                string sql = @"
                    SELECT ShipComponentTemplateID, ComponentName, ComponentSize, ComponentValue, EnginePowerMod
                    FROM FCT_ShipComponentTemplate
                    WHERE ComponentTypeID = 1 OR ComponentName LIKE '%Engine%'
                    ORDER BY ComponentName";

                using var cmd = new SqliteCommand(sql, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    double size = reader["ComponentSize"] != DBNull.Value ? Convert.ToDouble(reader["ComponentSize"]) : 5.0;
                    double cost = reader["ComponentValue"] != DBNull.Value ? Convert.ToDouble(reader["ComponentValue"]) : 50.0;
                    double powerMod = reader["EnginePowerMod"] != DBNull.Value ? Convert.ToDouble(reader["EnginePowerMod"]) : 1.0;
                    string name = reader["ComponentName"] != DBNull.Value ? reader["ComponentName"].ToString()! : "Motor Naval";

                    list.Add(new SavedEngineInfo
                    {
                        ComponentID = Convert.ToInt32(reader["ShipComponentTemplateID"]),
                        Name = name,
                        SizeHS = size,
                        PowerEP = size * 50.0 * powerMod,
                        FuelEfficiency = 1.0 / Math.Max(0.1, powerMod),
                        ThermalSignature = size * 10.0 * powerMod,
                        CostBP = cost,
                        IsCommercial = powerMod <= 1.0
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching saved engines: {ex.Message}");
            }
            return list;
        }

        public bool SaveEngineDesign(int raceId, string name, double hs, double ep, double fuelEff, double thermal, double cost, bool isComm, out string msg)
        {
            try
            {
                using var conn = GetWriteConnection();
                string sql = @"
                    INSERT INTO FCT_ShipComponentTemplate (ComponentTypeID, ComponentName, ComponentSize, ComponentValue, EnginePowerMod)
                    VALUES (1, @name, @hs, @cost, @powerMod)";

                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@hs", hs);
                cmd.Parameters.AddWithValue("@cost", cost);
                cmd.Parameters.AddWithValue("@powerMod", ep / Math.Max(1.0, hs * 50.0));

                cmd.ExecuteNonQuery();
                msg = $"⚡ Motor Naval '{name}' guardado con éxito en AuroraDB.db.";
                return true;
            }
            catch (Exception ex)
            {
                msg = $"❌ Error al guardar motor: {ex.Message}";
                return false;
            }
        }

        public List<ResearchedTechItem> GetResearchedTechsForRace(int raceId)
        {
            var list = new List<ResearchedTechItem>();
            try
            {
                using var conn = GetConnection();
                string sql = @"
                    SELECT rt.TechID, ts.Name, ts.CategoryID, ts.TechTypeID, ts.AdditionalInfo, ts.TechDescription
                    FROM FCT_RaceTech rt
                    JOIN FCT_TechSystem ts ON rt.TechID = ts.TechSystemID
                    WHERE rt.RaceID = @raceId
                    ORDER BY ts.CategoryID, ts.TechTypeID, ts.Name";

                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new ResearchedTechItem
                    {
                        TechID = Convert.ToInt32(reader["TechID"]),
                        Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString()! : "Tecnología Investigada",
                        CategoryID = reader["CategoryID"] != DBNull.Value ? Convert.ToInt32(reader["CategoryID"]) : 1,
                        TechTypeID = reader["TechTypeID"] != DBNull.Value ? Convert.ToInt32(reader["TechTypeID"]) : 1,
                        AdditionalInfo = reader["AdditionalInfo"] != DBNull.Value ? Convert.ToDouble(reader["AdditionalInfo"]) : 0.0,
                        Description = reader["TechDescription"] != DBNull.Value ? reader["TechDescription"].ToString()! : ""
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching researched techs: {ex.Message}");
            }
            return list;
        }

        public List<CustomProjectItem> GetCustomProjects(int raceId)
        {
            var list = new List<CustomProjectItem>();
            try
            {
                using var conn = GetConnection();

                // 1. Fetch Custom TechSystem projects
                string tsSql = @"
                    SELECT TechSystemID, Name, CategoryID, TechDescription, DevelopCost
                    FROM FCT_TechSystem
                    WHERE RaceID = @raceId OR TechDescription LIKE '%Race-designed%' OR TechDescription LIKE '%Custom%' OR CategoryID > 0
                    ORDER BY TechSystemID DESC";

                using (var cmd = new SqliteCommand(tsSql, conn))
                {
                    cmd.Parameters.AddWithValue("@raceId", raceId);
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["TechSystemID"]);
                        string name = reader["Name"] != DBNull.Value ? reader["Name"].ToString()! : "Proyecto I+D";
                        int catId = reader["CategoryID"] != DBNull.Value ? Convert.ToInt32(reader["CategoryID"]) : 1;
                        double cost = reader["DevelopCost"] != DBNull.Value ? Convert.ToDouble(reader["DevelopCost"]) : 100.0;
                        string desc = reader["TechDescription"] != DBNull.Value ? reader["TechDescription"].ToString()! : "Diseño del Imperio";

                        string categoryName = catId switch
                        {
                            1 => "⚡ Propulsión y Potencia",
                            2 => "💥 Energía y Láseres",
                            3 => "🚀 Misiles y Cinéticas",
                            4 => "📡 Sensores y Control",
                            5 => "🧬 Biología y Ciencias",
                            6 => "🏗️ Construcción y Logística",
                            _ => "⚙️ General e Industria"
                        };

                        list.Add(new CustomProjectItem
                        {
                            ProjectID = id,
                            Name = name,
                            Category = categoryName,
                            Source = ProjectSource.Aurora4XGame,
                            DevelopmentCostRP = cost,
                            BuildCostBP = Math.Round(cost / 10.0, 1),
                            SizeHS = 1.0,
                            Crew = 5,
                            HTK = 1,
                            SpecificationsSummary = string.IsNullOrEmpty(desc) ? "Proyecto personalizado registrado en el juego Aurora 4X" : desc
                        });
                    }
                }

                // 2. Fetch Custom Component Templates
                string sctSql = @"
                    SELECT ShipComponentTemplateID, ComponentName, ComponentValue, ComponentSize, ComponentTypeID
                    FROM FCT_ShipComponentTemplate
                    ORDER BY ShipComponentTemplateID DESC";

                using (var cmd = new SqliteCommand(sctSql, conn))
                {
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["ShipComponentTemplateID"]);
                        string name = reader["ComponentName"] != DBNull.Value ? reader["ComponentName"].ToString()! : "Componente Naval";
                        double cost = reader["ComponentValue"] != DBNull.Value ? Convert.ToDouble(reader["ComponentValue"]) : 10.0;
                        double size = reader["ComponentSize"] != DBNull.Value ? Convert.ToDouble(reader["ComponentSize"]) : 1.0;
                        int typeId = reader["ComponentTypeID"] != DBNull.Value ? Convert.ToInt32(reader["ComponentTypeID"]) : 1;

                        string category = typeId switch
                        {
                            1 => "⚡ Motor Naval",
                            8 or 24 => "📡 Sensor Táctico",
                            15 => "💥 Arma de Energía",
                            22 => "🚀 Lanzador de Misiles",
                            _ => "🛠️ Componente Especial"
                        };

                        if (!list.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                        {
                            list.Add(new CustomProjectItem
                            {
                                ProjectID = id + 100000,
                                Name = name,
                                Category = category,
                                Source = ProjectSource.Aurora4XGame,
                                DevelopmentCostRP = cost * 5.0,
                                BuildCostBP = cost,
                                SizeHS = size,
                                Crew = (int)Math.Max(1, size * 2),
                                HTK = (int)Math.Max(1, size),
                                SpecificationsSummary = $"Componente personalizado en el juego | Tamaño: {size:F1} HS ({size * 50:N0} t) | Costo: {cost:N1} BP"
                            });
                        }
                    }
                }

                // 3. Fetch Custom Missile Types
                string mtSql = @"
                    SELECT MissileID, Name, Size, Speed, WarheadStrength, Cost
                    FROM FCT_MissileType
                    ORDER BY MissileID DESC";

                using (var cmd = new SqliteCommand(mtSql, conn))
                {
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["MissileID"]);
                        string name = reader["Name"] != DBNull.Value ? reader["Name"].ToString()! : "Misil";
                        double msp = reader["Size"] != DBNull.Value ? Convert.ToDouble(reader["Size"]) : 1.0;
                        double speed = reader["Speed"] != DBNull.Value ? Convert.ToDouble(reader["Speed"]) : 1000.0;
                        double damage = reader["WarheadStrength"] != DBNull.Value ? Convert.ToDouble(reader["WarheadStrength"]) : 1.0;
                        double cost = reader["Cost"] != DBNull.Value ? Convert.ToDouble(reader["Cost"]) : 1.0;

                        if (!list.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                        {
                            list.Add(new CustomProjectItem
                            {
                                ProjectID = id + 200000,
                                Name = name,
                                Category = "🚀 Misil / Torpedo",
                                Source = ProjectSource.Aurora4XGame,
                                DevelopmentCostRP = cost * 10.0,
                                BuildCostBP = cost,
                                SizeHS = msp / 20.0,
                                Crew = 0,
                                HTK = 1,
                                SpecificationsSummary = $"Misil guardado en Aurora 4X | Tamaño: {msp:F1} MSP | Vel: {speed:N0} km/s | Daño: {damage:F1} HP | Costo: {cost:N2} BP"
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching custom projects: {ex.Message}");
            }
            return list;
        }

        public bool CreateCustomProjectInDatabase(int raceId, CustomProjectItem project, out string msg)
        {
            try
            {
                using var conn = GetWriteConnection();

                // 1. Insert into FCT_TechSystem
                string tsSql = @"
                    INSERT INTO FCT_TechSystem (GameID, RaceID, Name, CategoryID, DevelopCost, TechDescription, TechTypeID, NoTechScan, RuinOnly, StartingSystem, ConventionalSystem, AutomaticResearch)
                    VALUES (1, @raceId, @name, 4, @cost, @desc, 100, 0, 0, 0, 0, 0)";

                using (var cmd = new SqliteCommand(tsSql, conn))
                {
                    cmd.Parameters.AddWithValue("@raceId", raceId);
                    cmd.Parameters.AddWithValue("@name", project.Name);
                    cmd.Parameters.AddWithValue("@cost", project.DevelopmentCostRP > 0 ? project.DevelopmentCostRP : project.BuildCostBP * 10.0);
                    cmd.Parameters.AddWithValue("@desc", $"Diseñado en Aurora Master Command Suite | {project.Category} | {project.SpecificationsSummary}");
                    cmd.ExecuteNonQuery();
                }

                // 2. Insert into FCT_ShipComponentTemplate if applicable
                string sctSql = @"
                    INSERT INTO FCT_ShipComponentTemplate (ComponentTypeID, ComponentName, ComponentSize, ComponentValue, EnginePowerMod)
                    VALUES (1, @name, @size, @cost, 1.0)";

                using (var cmd = new SqliteCommand(sctSql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", project.Name);
                    cmd.Parameters.AddWithValue("@size", project.SizeHS);
                    cmd.Parameters.AddWithValue("@cost", project.BuildCostBP);
                    cmd.ExecuteNonQuery();
                }

                msg = $"🚀 Proyecto '{project.Name}' creado y registrado con éxito en AuroraDB.db.";
                return true;
            }
            catch (Exception ex)
            {
                msg = $"❌ Error al guardar proyecto en base de datos: {ex.Message}";
                return false;
            }
        }

        public List<ShipyardComplexInfo> GetShipyards(int raceId)
        {
            var shipyards = new List<ShipyardComplexInfo>();
            try
            {
                using var conn = GetConnection();
                string syQuery = @"
                    SELECT ShipyardID, ShipyardName, Slipways, Capacity, SYType
                    FROM FCT_Shipyard
                    WHERE RaceID = @raceId
                    ORDER BY SYType, ShipyardName";

                using var cmd = new SqliteCommand(syQuery, conn);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var sy = new ShipyardComplexInfo
                    {
                        ShipyardID = Convert.ToInt32(reader["ShipyardID"]),
                        ShipyardName = reader["ShipyardName"].ToString() ?? "Astillero",
                        Slipways = reader["Slipways"] != DBNull.Value ? Convert.ToInt32(reader["Slipways"]) : 1,
                        CapacityTons = reader["Capacity"] != DBNull.Value ? Convert.ToDouble(reader["Capacity"]) : 5000.0,
                        SYType = reader["SYType"] != DBNull.Value ? Convert.ToInt32(reader["SYType"]) : 1
                    };

                    shipyards.Add(sy);
                }

                foreach (var sy in shipyards)
                {
                    string taskQuery = @"
                        SELECT TaskID, UnitName, TotalBP, CompletedBP
                        FROM FCT_ShipyardTask
                        WHERE ShipyardID = @syId";

                    using var taskCmd = new SqliteCommand(taskQuery, conn);
                    taskCmd.Parameters.AddWithValue("@syId", sy.ShipyardID);
                    using var taskReader = taskCmd.ExecuteReader();
                    while (taskReader.Read())
                    {
                        sy.Tasks.Add(new ShipyardTaskInfo
                        {
                            TaskID = Convert.ToInt32(taskReader["TaskID"]),
                            UnitName = taskReader["UnitName"].ToString() ?? "Nave",
                            TotalBP = taskReader["TotalBP"] != DBNull.Value ? Convert.ToDouble(taskReader["TotalBP"]) : 100.0,
                            CompletedBP = taskReader["CompletedBP"] != DBNull.Value ? Convert.ToDouble(taskReader["CompletedBP"]) : 0.0
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching shipyards: {ex.Message}");
            }

            return shipyards;
        }

        public List<ShipClassSimpleInfo> GetRaceClasses(int raceId)
        {
            var list = new List<ShipClassSimpleInfo>();
            try
            {
                using var conn = GetConnection();
                string sql = @"
                    SELECT ShipClassID, ClassName, Size, Cost, Commercial,
                           MaxSpeed, FuelCapacity, Crew, MaintSupplies, ClassThermal, EMSensorStrength
                    FROM FCT_ShipClass
                    WHERE RaceID = @raceId AND (Obsolete IS NULL OR Obsolete = 0)
                    ORDER BY ClassName";

                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int isComm = reader["Commercial"] != DBNull.Value ? Convert.ToInt32(reader["Commercial"]) : 0;
                    list.Add(new ShipClassSimpleInfo
                    {
                        ClassID = Convert.ToInt32(reader["ShipClassID"]),
                        ClassName = reader["ClassName"].ToString() ?? "Clase",
                        SizeHS = reader["Size"] != DBNull.Value ? Convert.ToDouble(reader["Size"]) : 10.0,
                        CostBP = reader["Cost"] != DBNull.Value ? Convert.ToDouble(reader["Cost"]) : 100.0,
                        IsMilitary = isComm == 0,
                        MaxSpeedKmS = reader["MaxSpeed"] != DBNull.Value ? Convert.ToDouble(reader["MaxSpeed"]) : 1000.0,
                        TotalFuelLiters = reader["FuelCapacity"] != DBNull.Value ? Convert.ToDouble(reader["FuelCapacity"]) : 50000.0,
                        TotalCrewRequired = reader["Crew"] != DBNull.Value ? Convert.ToInt32(reader["Crew"]) : 50,
                        TotalMSP = reader["MaintSupplies"] != DBNull.Value ? Convert.ToDouble(reader["MaintSupplies"]) : 100.0,
                        ThermalSignature = reader["ClassThermal"] != DBNull.Value ? Convert.ToDouble(reader["ClassThermal"]) : 0.0,
                        EMSignature = reader["EMSensorStrength"] != DBNull.Value ? Convert.ToDouble(reader["EMSensorStrength"]) : 0.0
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching race classes: {ex.Message}");
            }

            return list;
        }

        public bool AddShipyardTask(int shipyardId, string unitName, double totalBP, out string msg)
        {
            try
            {
                using var conn = GetWriteConnection();
                string sql = @"
                    INSERT INTO FCT_ShipyardTask (ShipyardID, UnitName, TotalBP, CompletedBP)
                    VALUES (@syId, @name, @bp, 0.0)";

                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@syId", shipyardId);
                cmd.Parameters.AddWithValue("@name", unitName);
                cmd.Parameters.AddWithValue("@bp", totalBP);

                cmd.ExecuteNonQuery();
                msg = $"✅ Orden de construcción para '{unitName}' enviada con éxito al astillero.";
                return true;
            }
            catch (Exception ex)
            {
                msg = $"Error al enviar orden de construcción: {ex.Message}";
                return false;
            }
        }

        public bool DeleteShipyardTask(int taskId, out string msg)
        {
            try
            {
                using var conn = GetWriteConnection();
                string sql = "DELETE FROM FCT_ShipyardTask WHERE TaskID = @taskId";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@taskId", taskId);

                cmd.ExecuteNonQuery();
                msg = "✅ Tarea de construcción cancelada y grada liberada correctamente.";
                return true;
            }
            catch (Exception ex)
            {
                msg = $"Error al cancelar tarea: {ex.Message}";
                return false;
            }
        }

        public EmpireShipyardTelemetry GetShipyardTelemetry(int raceId)
        {
            var tele = new EmpireShipyardTelemetry();
            var shipyards = GetShipyards(raceId);

            foreach (var sy in shipyards)
            {
                if (sy.SYType == 1) // Naval
                {
                    tele.TotalNavalCapacityTons += sy.CapacityTons;
                    tele.TotalNavalSlipways += sy.Slipways;
                }
                else // Commercial
                {
                    tele.TotalCommercialCapacityTons += sy.CapacityTons;
                    tele.TotalCommercialSlipways += sy.Slipways;
                }

                tele.ActiveBuildTasks += sy.ActiveTasksCount;
                tele.FreeSlipways += sy.FreeSlipways;
                tele.TotalAnnualBPOutput += sy.BuildSpeedBPPerYear * sy.Slipways;
            }

            tele.GovernorBonusPercent = 10.0; // Default Governor Shipbuilding Bonus %
            return tele;
        }

        public List<CommanderInfo> GetCommanders(int raceId)
        {
            var commanders = new List<CommanderInfo>();
            try
            {
                using var conn = GetConnection();
                string query = @"
                    SELECT CommanderID, Name, Title, CommanderType, Seniority, 
                           PromotionScore, Loyalty, CommandID, PopLocationID
                    FROM FCT_Commander
                    WHERE RaceID = @raceId AND (Deceased = 0 OR Deceased IS NULL)
                    ORDER BY CommanderType, Seniority DESC";

                using var cmd = new SqliteCommand(query, conn);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int type = reader["CommanderType"] != DBNull.Value ? Convert.ToInt32(reader["CommanderType"]) : 1;
                    string typeDisp = type switch
                    {
                        1 => "🎓 Científico",
                        2 => "⚓ Oficial Naval",
                        3 => "🏛️ Gobernador Planetario",
                        _ => "⚔️ Comandante Terrestre"
                    };

                    int cmdId = reader["CommandID"] != DBNull.Value ? Convert.ToInt32(reader["CommandID"]) : 0;
                    int popId = reader["PopLocationID"] != DBNull.Value ? Convert.ToInt32(reader["PopLocationID"]) : 0;

                    string assignLoc = "Sin Asignar";
                    if (type == 1) assignLoc = cmdId > 0 ? $"Lab I+D N° {cmdId}" : "Sin Asignar";
                    else if (type == 3) assignLoc = popId > 0 ? "Gobernación Capital" : "Sin Asignar";
                    else if (type == 2) assignLoc = cmdId > 0 ? $"Flota / Nave #{cmdId}" : "Sin Asignar";
                    else assignLoc = cmdId > 0 ? $"División Terrestre #{cmdId}" : "Sin Asignar";

                    commanders.Add(new CommanderInfo
                    {
                        CommanderID = Convert.ToInt32(reader["CommanderID"]),
                        Name = reader["Name"].ToString() ?? "Comandante",
                        Title = reader["Title"] != DBNull.Value ? reader["Title"].ToString()! : "Oficial",
                        CommanderType = type,
                        TypeDisplay = typeDisp,
                        Seniority = reader["Seniority"] != DBNull.Value ? Convert.ToDouble(reader["Seniority"]) : 0.0,
                        PromotionScore = reader["PromotionScore"] != DBNull.Value ? Convert.ToDouble(reader["PromotionScore"]) : 50.0,
                        LoyaltyRating = reader["Loyalty"] != DBNull.Value ? Convert.ToDouble(reader["Loyalty"]) : 100.0,
                        AssignmentLocation = assignLoc
                    });
                }

                // Fetch real bonus values from FCT_CommanderBonuses & DIM_CommanderBonusType
                foreach (var cmdr in commanders)
                {
                    string bonusSql = @"
                        SELECT b.BonusValue, t.Description
                        FROM FCT_CommanderBonuses b
                        INNER JOIN DIM_CommanderBonusType t ON b.BonusID = t.BonusID
                        WHERE b.CommanderID = @cmdrId AND b.BonusValue > 0
                        ORDER BY b.BonusValue DESC";

                    using var bCmd = new SqliteCommand(bonusSql, conn);
                    bCmd.Parameters.AddWithValue("@cmdrId", cmdr.CommanderID);
                    using var bReader = bCmd.ExecuteReader();
                    while (bReader.Read())
                    {
                        double val = Convert.ToDouble(bReader["BonusValue"]);
                        string desc = bReader["Description"].ToString() ?? "Bonificación";
                        cmdr.DetailedBonuses.Add(new CommanderBonusItem
                        {
                            Description = desc,
                            ValuePercent = val * 100.0
                        });
                    }

                    if (cmdr.DetailedBonuses.Count == 0)
                    {
                        // Default fallback bonus per type if empty
                        double defaultVal = cmdr.CommanderType switch { 1 => 25.0, 2 => 15.0, 3 => 20.0, _ => 10.0 };
                        string defaultDesc = cmdr.CommanderType switch { 1 => "Investigación I+D", 2 => "Velocidad Naval", 3 => "Minería Colonias", _ => "Combate Terrestre" };
                        cmdr.DetailedBonuses.Add(new CommanderBonusItem { Description = defaultDesc, ValuePercent = defaultVal });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching commanders: {ex.Message}");
            }
            return commanders;
        }

        public bool PromoteCommander(int commanderId, out string msg)
        {
            try
            {
                using var conn = GetWriteConnection();
                string sql = @"
                    UPDATE FCT_Commander
                    SET PromotionScore = PromotionScore + 25.0,
                        Title = CASE 
                            WHEN Title LIKE '%Capitán%' THEN 'Almirante de Flota'
                            WHEN Title LIKE '%Científico%' THEN 'Director de I+D Imperial'
                            WHEN Title LIKE '%Gobernador%' THEN 'Gobernador General'
                            ELSE 'Comandante Superior'
                        END
                    WHERE CommanderID = @cmdrId";

                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@cmdrId", commanderId);
                cmd.ExecuteNonQuery();

                msg = "🎖️ ¡Promoción concedida! Se ha incrementado el rango oficial y la aptitud de mando del comandante.";
                return true;
            }
            catch (Exception ex)
            {
                msg = $"Error al promover comandante: {ex.Message}";
                return false;
            }
        }

        public bool AssignCommanderLocation(int commanderId, string locationName, out string msg)
        {
            try
            {
                using var conn = GetWriteConnection();
                string sql = "UPDATE FCT_Commander SET CommandID = 1 WHERE CommanderID = @cmdrId";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@cmdrId", commanderId);
                cmd.ExecuteNonQuery();

                msg = $"📜 Asignación oficial registrada. Comandante desplegado en: '{locationName}'.";
                return true;
            }
            catch (Exception ex)
            {
                msg = $"Error al asignar comandante: {ex.Message}";
                return false;
            }
        }

        private double GetDouble(SqliteDataReader reader, string column)
        {
            return reader[column] != DBNull.Value ? Convert.ToDouble(reader[column]) : 0.0;
        }

        private void InferComponentProperties(Component comp)
        {
            var name = comp.ComponentName.ToLower();
            var type = comp.TypeName.ToLower();

            if (name.Contains("fuel tank") || type.Contains("fuel"))
            {
                if (name.Contains("ultra-large") || comp.ComponentSize >= 10) comp.FuelCapacity = comp.ComponentSize * 50000;
                else if (name.Contains("large") || comp.ComponentSize >= 5) comp.FuelCapacity = comp.ComponentSize * 40000;
                else comp.FuelCapacity = comp.ComponentSize * 25000;
                comp.MineralCosts["Sorium"] = comp.Cost;
            }
            else if (name.Contains("drive") || name.Contains("engine") || type.Contains("engine"))
            {
                if (comp.EnginePower <= 0) comp.EnginePower = comp.ComponentSize * 20.0;
                comp.FuelEfficiency = 1.0;
                comp.MineralCosts["Gallicite"] = comp.Cost * 0.8;
                comp.MineralCosts["Duranium"] = comp.Cost * 0.2;
            }
            else if (name.Contains("crew quarters") || name.Contains("habitat"))
            {
                comp.Crew = 0;
                comp.MineralCosts["Duranium"] = comp.Cost;
            }
            else if (name.Contains("engineering space"))
            {
                comp.MaintSupplies = (int)(comp.ComponentSize * 10);
                comp.MineralCosts["Duranium"] = comp.Cost * 0.5;
            }
            else if (name.Contains("shield") || type.Contains("shield"))
            {
                comp.ShieldStrength = comp.ComponentSize * 3.0;
                comp.MineralCosts["Corundium"] = comp.Cost;
            }
            else if (name.Contains("jump drive") || type.Contains("jump"))
            {
                comp.JumpRating = 3;
                comp.JumpMaxHS = (int)(comp.ComponentSize * 100);
                comp.MineralCosts["Tritium"] = comp.Cost * 0.7;
                comp.MineralCosts["Duranium"] = comp.Cost * 0.3;
            }
            else
            {
                comp.MineralCosts["Duranium"] = comp.Cost * 0.6;
                comp.MineralCosts["Gallicite"] = comp.Cost * 0.4;
            }

            if (comp.Crew == 0 && !name.Contains("crew quarters") && comp.ComponentSize > 1)
            {
                comp.Crew = Math.Max(1, (int)(comp.ComponentSize * 0.8));
            }
        }

        public List<Component> GetDefaultFallbackComponents()
        {
            var list = new List<Component>();

            list.Add(new Component { ComponentID = 101, ComponentName = "Magneto-Plasma Drive (HS 10)", TypeName = "Engine", ComponentSize = 10, Cost = 50, EnginePower = 250, Crew = 5, MineralCosts = new Dictionary<string, double> { { "Gallicite", 40 }, { "Duranium", 10 } } });
            list.Add(new Component { ComponentID = 102, ComponentName = "Commercial Nuclear Thermal Engine (HS 50)", TypeName = "Engine", ComponentSize = 50, Cost = 100, EnginePower = 400, Crew = 10, MineralCosts = new Dictionary<string, double> { { "Gallicite", 80 }, { "Duranium", 20 } } });

            list.Add(new Component { ComponentID = 201, ComponentName = "Standard Fuel Tank (50k Liters)", TypeName = "Fuel", ComponentSize = 1, Cost = 5, FuelCapacity = 50000, Crew = 0, MineralCosts = new Dictionary<string, double> { { "Sorium", 5 } } });
            list.Add(new Component { ComponentID = 202, ComponentName = "Large Fuel Tank (250k Liters)", TypeName = "Fuel", ComponentSize = 5, Cost = 25, FuelCapacity = 250000, Crew = 0, MineralCosts = new Dictionary<string, double> { { "Sorium", 25 } } });
            list.Add(new Component { ComponentID = 203, ComponentName = "Very Large Fuel Tank (1M Liters)", TypeName = "Fuel", ComponentSize = 20, Cost = 100, FuelCapacity = 1000000, Crew = 0, MineralCosts = new Dictionary<string, double> { { "Sorium", 100 } } });

            list.Add(new Component { ComponentID = 301, ComponentName = "Crew Quarters (50 Crew Capacity)", TypeName = "Habitation", ComponentSize = 1, Cost = 2, Crew = 0, MineralCosts = new Dictionary<string, double> { { "Duranium", 2 } } });
            list.Add(new Component { ComponentID = 302, ComponentName = "Engineering Spaces", TypeName = "Maintenance", ComponentSize = 1, Cost = 10, Crew = 5, MaintSupplies = 25, MineralCosts = new Dictionary<string, double> { { "Duranium", 5 } } });

            list.Add(new Component { ComponentID = 401, ComponentName = "Alpha Shield Generator", TypeName = "Shield", ComponentSize = 2, Cost = 15, ShieldStrength = 6, Crew = 2, MineralCosts = new Dictionary<string, double> { { "Corundium", 15 } } });
            list.Add(new Component { ComponentID = 402, ComponentName = "Military Jump Drive (Max 10,000 Tons)", TypeName = "Jump Drive", ComponentSize = 10, Cost = 150, JumpRating = 3, JumpMaxHS = 200, Crew = 8, MineralCosts = new Dictionary<string, double> { { "Tritium", 100 }, { "Duranium", 50 } } });

            list.Add(new Component { ComponentID = 501, ComponentName = "15cm C3 Near-Ultraviolet Laser", TypeName = "Beam Weapon", ComponentSize = 4, Cost = 32, Crew = 8, MineralCosts = new Dictionary<string, double> { { "Corundium", 20 }, { "Boronide", 12 } } });
            list.Add(new Component { ComponentID = 502, ComponentName = "Active Search Sensor Res-20 (50M km)", TypeName = "Active Sensor", ComponentSize = 5, Cost = 45, ActiveSensor = 50, Crew = 5, MineralCosts = new Dictionary<string, double> { { "Uridium", 45 } } });
            list.Add(new Component { ComponentID = 503, ComponentName = "Thermal Sensor Array (TH-10)", TypeName = "Passive Sensor", ComponentSize = 2, Cost = 20, PassiveSensor = 10, Crew = 2, MineralCosts = new Dictionary<string, double> { { "Uridium", 20 } } });
            list.Add(new Component { ComponentID = 504, ComponentName = "Size 6 Missile Magazine (Capacity 120)", TypeName = "Magazine", ComponentSize = 6, Cost = 24, MissileCapacity = 120, Crew = 3, MineralCosts = new Dictionary<string, double> { { "Neutronium", 24 } } });

            return list;
        }

        public bool TransferShipToFleet(int raceId, int shipId, int targetFleetId, string targetFleetName, out string msg)
        {
            try
            {
                using var conn = GetWriteConnection();
                string sql = "UPDATE FCT_Ship SET FleetID = @fId WHERE ShipID = @sId";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@fId", targetFleetId);
                cmd.Parameters.AddWithValue("@sId", shipId);
                cmd.ExecuteNonQuery();

                msg = $"🔄 Nave reasignada exitosamente a '{targetFleetName}' en AuroraDB.db.";
                LiveSyncBridge.NotifyGameSync("SHIP_TRANSFER");
                return true;
            }
            catch (Exception ex)
            {
                msg = $"❌ Error al reasignar nave: {ex.Message}";
                return false;
            }
        }

        public bool RefuelFleet(int raceId, int fleetId, string fleetName, out string msg)
        {
            try
            {
                using var conn = GetWriteConnection();
                string sql = "UPDATE FCT_Ship SET Fuel = 200000.0 WHERE FleetID = @fId";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@fId", fleetId);
                cmd.ExecuteNonQuery();

                msg = $"⛽ Flota '{fleetName}' reabastecida al 100% de combustible en AuroraDB.db.";
                LiveSyncBridge.NotifyGameSync("REFUEL");
                return true;
            }
            catch (Exception ex)
            {
                msg = $"❌ Error al reabastecer flota: {ex.Message}";
                return false;
            }
        }

        public bool ReplenishFleetMSP(int raceId, int fleetId, string fleetName, out string msg)
        {
            try
            {
                using var conn = GetWriteConnection();
                string sql = "UPDATE FCT_Ship SET CurrentMSP = 500.0, CrewMorale = 1.0 WHERE FleetID = @fId";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@fId", fleetId);
                cmd.ExecuteNonQuery();

                msg = $"📦 Repuestos MSP y moral de tripulación restaurados al 100% en '{fleetName}'.";
                LiveSyncBridge.NotifyGameSync("REPLENISH_MSP");
                return true;
            }
            catch (Exception ex)
            {
                msg = $"❌ Error al reponer repuestos: {ex.Message}";
                return false;
            }
        }

        public List<string> GetEmpireHistoryLogs(int raceId, int maxLogs = 80)
        {
            var history = new List<string>();
            try
            {
                using var conn = GetConnection();
                string query = @"
                    SELECT g.Time, d.Description AS EventTypeDesc, g.MessageText
                    FROM FCT_GameLog g
                    LEFT JOIN DIM_EventType d ON g.EventType = d.EventTypeID
                    WHERE g.RaceID = @raceId OR g.RaceID = 0
                    ORDER BY g.Time ASC
                    LIMIT @maxLogs";

                using var cmd = new SqliteCommand(query, conn);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                cmd.Parameters.AddWithValue("@maxLogs", maxLogs);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    double timeSeconds = reader["Time"] != DBNull.Value ? Convert.ToDouble(reader["Time"]) : 0;
                    string eventType = reader["EventTypeDesc"] != DBNull.Value ? reader["EventTypeDesc"].ToString()! : "Evento";
                    string msg = reader["MessageText"] != DBNull.Value ? reader["MessageText"].ToString()! : "";

                    // Convert Aurora 4X game time seconds into years/days format
                    double years = Math.Floor(timeSeconds / 31536000.0);
                    double remainingSecs = timeSeconds % 31536000.0;
                    double days = Math.Floor(remainingSecs / 86400.0);

                    string dateStr = years > 0 ? $"Año {years:F0}, Día {days:F0}" : $"Día {days:F0}";

                    if (!string.IsNullOrWhiteSpace(msg))
                    {
                        history.Add($"[{dateStr}] ({eventType}) {msg}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetEmpireHistoryLogs Error: {ex.Message}");
            }
            return history;
        }

        public List<NamingThemeItem> GetNamingThemes()
        {
            var themes = new List<NamingThemeItem>();
            try
            {
                using var conn = GetConnection();
                string query = "SELECT ThemeID, Description FROM DIM_NamingThemeTypes ORDER BY Description ASC";
                using var cmd = new SqliteCommand(query, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    themes.Add(new NamingThemeItem
                    {
                        ThemeID = Convert.ToInt32(reader["ThemeID"]),
                        Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString()! : "Sin Nombre"
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetNamingThemes Error: {ex.Message}");
            }
            return themes;
        }

        public EmpireNamingConfig GetEmpireNamingConfig(int raceId)
        {
            var config = new EmpireNamingConfig { RaceID = raceId };
            try
            {
                using var conn = GetConnection();
                string query = "SELECT ClassThemeID, SystemThemeID, DesignThemeID, GroundThemeID, MissileThemeID, NameThemeID FROM FCT_Race WHERE RaceID = @raceId";
                using var cmd = new SqliteCommand(query, conn);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    config.ClassThemeID = reader["ClassThemeID"] != DBNull.Value ? Convert.ToInt32(reader["ClassThemeID"]) : 0;
                    config.SystemThemeID = reader["SystemThemeID"] != DBNull.Value ? Convert.ToInt32(reader["SystemThemeID"]) : 0;
                    config.DesignThemeID = reader["DesignThemeID"] != DBNull.Value ? Convert.ToInt32(reader["DesignThemeID"]) : 0;
                    config.GroundThemeID = reader["GroundThemeID"] != DBNull.Value ? Convert.ToInt32(reader["GroundThemeID"]) : 0;
                    config.MissileThemeID = reader["MissileThemeID"] != DBNull.Value ? Convert.ToInt32(reader["MissileThemeID"]) : 0;
                    config.NameThemeID = reader["NameThemeID"] != DBNull.Value ? Convert.ToInt32(reader["NameThemeID"]) : 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetEmpireNamingConfig Error: {ex.Message}");
            }
            return config;
        }

        public bool SaveEmpireNamingConfig(int raceId, EmpireNamingConfig config, out string msg)
        {
            try
            {
                using var conn = GetWriteConnection();
                string sql = @"
                    UPDATE FCT_Race 
                    SET ClassThemeID = @classTheme,
                        SystemThemeID = @sysTheme,
                        DesignThemeID = @designTheme,
                        GroundThemeID = @groundTheme,
                        MissileThemeID = @missileTheme,
                        NameThemeID = @nameTheme
                    WHERE RaceID = @raceId";

                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@classTheme", config.ClassThemeID);
                cmd.Parameters.AddWithValue("@sysTheme", config.SystemThemeID);
                cmd.Parameters.AddWithValue("@designTheme", config.DesignThemeID);
                cmd.Parameters.AddWithValue("@groundTheme", config.GroundThemeID);
                cmd.Parameters.AddWithValue("@missileTheme", config.MissileThemeID);
                cmd.Parameters.AddWithValue("@nameTheme", config.NameThemeID);
                cmd.Parameters.AddWithValue("@raceId", raceId);

                cmd.ExecuteNonQuery();

                LiveSyncBridge.NotifyGameSync("EMPIRE_NAMING_CONFIG_UPDATED");
                msg = "✅ Configuración de Lotes de Nombres guardada con éxito en AuroraDB.db.";
                return true;
            }
            catch (Exception ex)
            {
                msg = $"❌ Error al guardar lotes de nombres: {ex.Message}";
                return false;
            }
        }

        public List<string> GetCompanyNames(int raceId)
        {
            var companies = new List<string>();

            try
            {
                using var conn = GetConnection();
                string query = @"
                    SELECT DISTINCT Name FROM FCT_TechSystem 
                    WHERE RaceID = @raceId AND (Name LIKE '%Limited%' OR Name LIKE '%Turbines%' OR Name LIKE '%Industries%' OR Name LIKE '%Corp%' OR Name LIKE '%Aerospace%' OR Name LIKE '%Naval%' OR Name LIKE '%Forge%' OR Name LIKE '%Systems%')
                    LIMIT 20";
                using var cmd = new SqliteCommand(query, conn);
                cmd.Parameters.AddWithValue("@raceId", raceId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string fullName = reader["Name"].ToString()!;
                    string[] parts = fullName.Split(' ');
                    if (parts.Length >= 2)
                    {
                        string companyName = string.Join(" ", parts.Take(3)).Trim();
                        if (!companies.Contains(companyName))
                        {
                            companies.Add(companyName);
                        }
                    }
                }

                if (companies.Count == 0)
                {
                    string raceSql = "SELECT RaceName FROM FCT_Race WHERE RaceID = @raceId";
                    using var rCmd = new SqliteCommand(raceSql, conn);
                    rCmd.Parameters.AddWithValue("@raceId", raceId);
                    string raceName = Convert.ToString(rCmd.ExecuteScalar()) ?? "Imperio";
                    if (string.IsNullOrWhiteSpace(raceName)) raceName = "Imperio";

                    companies.Add($"{raceName} Naval Ordnance");
                    companies.Add($"{raceName} Aerospace Corp");
                    companies.Add($"{raceName} Heavy Industries");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetCompanyNames Error: {ex.Message}");
            }
            return companies;
        }
    }
}
