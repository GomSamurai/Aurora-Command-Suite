import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

race_id = 784

print("--- Step 1: Querying GetFullEmpireDetails SQL ---")
query = """
    SELECT r.RaceID, r.GameID, r.RaceName, r.RaceTitle, r.FlagPic,
           s.SpeciesID, s.SpeciesName, s.RacePic, s.Temperature, s.TempDev,
           s.Gravity, s.GravDev, s.Oxygen, s.PressMax, s.ProductionRateModifier,
           s.ResearchRateModifier, s.PopulationGrowthModifier, s.Xenophobia, s.Diplomacy, s.Militancy
    FROM FCT_Race r
    LEFT JOIN FCT_Species s ON r.GameID = s.GameID
    WHERE r.RaceID = ?
    LIMIT 1"""

cursor.execute(query, (race_id,))
row = cursor.fetchone()
print("GetFullEmpireDetails result:", dict(row) if row else "None")

if row:
    game_id = row['GameID']
    species_id = row['SpeciesID']
    print(f"GameID: {game_id}, SpeciesID: {species_id}")

    print("\n--- Step 2: Testing UPDATE FCT_Race ---")
    try:
        cursor.execute("""
            UPDATE FCT_Race 
            SET RaceName = ?, 
                RaceTitle = ?, 
                FlagPic = ? 
            WHERE RaceID = ?""", ('El Imperio', 'Imperio Epistocrático', 'flag0517.jpg', race_id))
        print("UPDATE FCT_Race SUCCESS! Rows affected:", cursor.rowcount)
    except Exception as e:
        print("UPDATE FCT_Race FAILED:", e)

    print("\n--- Step 3: Testing UPDATE FCT_Species ---")
    try:
        cursor.execute("""
            UPDATE FCT_Species 
            SET SpeciesName = ?, 
                RacePic = ?,
                Temperature = ?,
                TempDev = ?,
                Gravity = ?,
                GravDev = ?,
                Oxygen = ?,
                PressMax = ?,
                ProductionRateModifier = ?,
                ResearchRateModifier = ?,
                PopulationGrowthModifier = ?,
                Xenophobia = ?,
                Diplomacy = ?,
                Militancy = ?
            WHERE SpeciesID = ?""", (
                'Human', 'Race327.bmp', 287.03, 24.0, 1.0, 0.9, 0.20, 4.0,
                1.0, 1.0, 1.0, 50, 50, 50, species_id
            ))
        print("UPDATE FCT_Species SUCCESS! Rows affected:", cursor.rowcount)
    except Exception as e:
        print("UPDATE FCT_Species FAILED:", e)

conn.close()
