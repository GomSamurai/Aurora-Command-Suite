import sqlite3
import os
import re
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

print("=== 1. TESTING SQLITE DATABASE QUERIES ===")

# Test race list query
try:
    cursor.execute("SELECT RaceID, GameID, COALESCE(NULLIF(RaceTitle, ''), RaceName) as DisplayName FROM FCT_Race WHERE (NPR = 0 OR NPR IS NULL) ORDER BY GameID DESC, DisplayName")
    races = cursor.fetchall()
    print(f"✅ FCT_Race query OK. Found {len(races)} player races.")
    sample_race_id = races[0]['RaceID'] if races else 779
except Exception as e:
    print(f"❌ FCT_Race query ERROR: {e}")

# Test full empire details query
try:
    cursor.execute("""
        SELECT r.RaceID, r.GameID, r.RaceName, r.RaceTitle, r.FlagPic,
               s.SpeciesID, s.SpeciesName, s.RacePic, s.Temperature, s.TempDev,
               s.Gravity, s.GravDev, s.Oxygen, s.PressMax, s.ProductionRateModifier,
               s.ResearchRateModifier, s.PopulationGrowthModifier, s.Xenophobia, s.Diplomacy, s.Militancy
        FROM FCT_Race r
        LEFT JOIN FCT_Species s ON r.GameID = s.GameID
        WHERE r.RaceID = ?
        LIMIT 1""", (sample_race_id,))
    emp_row = cursor.fetchone()
    print(f"✅ Full Empire Details query OK: {dict(emp_row) if emp_row else 'None'}")
except Exception as e:
    print(f"❌ Full Empire Details query ERROR: {e}")

# Test colonies query
try:
    cursor.execute("""
        SELECT PopulationID, PopName, Population, Capital, FuelStockpile,
               Duranium, Sorium, Neutronium, Corundium, Uridium, Gallicite,
               Boronide, Mercassium, Vendarite, Corbomite
        FROM FCT_Population
        WHERE RaceID = ?
        ORDER BY Capital DESC, Population DESC""", (sample_race_id,))
    cols = cursor.fetchall()
    print(f"✅ Colonies query OK. Found {len(cols)} colonies.")
except Exception as e:
    print(f"❌ Colonies query ERROR: {e}")

# Test active fleets query
try:
    cursor.execute("""
        SELECT FleetID, FleetName, RaceID, SystemID, Xcor, Ycor, Speed
        FROM FCT_Fleet
        WHERE RaceID = ?
        ORDER BY FleetName""", (sample_race_id,))
    fleets = cursor.fetchall()
    print(f"✅ Active Fleets query OK. Found {len(fleets)} fleets.")
except Exception as e:
    print(f"❌ Active Fleets query ERROR: {e}")

# Test industrial projects query
try:
    cursor.execute("""
        SELECT p.ProjectID, p.PopulationID, p.SystemID, p.ProjectTitle, p.Amount, p.Completed,
               pop.PopName
        FROM FCT_IndustrialProjects p
        LEFT JOIN FCT_Population pop ON p.PopulationID = pop.PopulationID
        WHERE pop.RaceID = ?
        LIMIT 5""", (sample_race_id,))
    projs = cursor.fetchall()
    print(f"✅ Industrial Projects query OK. Found {len(projs)} projects.")
except Exception as e:
    print(f"❌ Industrial Projects query ERROR: {e}")

# Test research projects query
try:
    cursor.execute("""
        SELECT TechID, TechName, TechCost, TechCategory
        FROM FCT_Tech
        LIMIT 5""")
    techs = cursor.fetchall()
    print(f"✅ Research Techs query OK. Sample: {len(techs)} techs.")
except Exception as e:
    print(f"❌ Research Techs query ERROR: {e}")

# Test officers query
try:
    cursor.execute("""
        SELECT CommanderID, Name, RankID, BonusType, BonusValue
        FROM FCT_Commander
        LIMIT 5""")
    cmds = cursor.fetchall()
    print(f"✅ Officers query OK. Sample: {len(cmds)} commanders.")
except Exception as e:
    print(f"❌ Officers query ERROR: {e}")

# Test system exploration query
try:
    cursor.execute("""
        SELECT SystemID, SystemName
        FROM FCT_System
        LIMIT 5""")
    sys_list = cursor.fetchall()
    print(f"✅ Systems query OK. Sample: {len(sys_list)} systems.")
except Exception as e:
    print(f"❌ Systems query ERROR: {e}")

conn.close()
