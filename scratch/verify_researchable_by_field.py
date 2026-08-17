import sqlite3

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
cursor = conn.cursor()

# Get first race ID
cursor.execute("SELECT RaceID, RaceName FROM FCT_Race LIMIT 1")
race_row = cursor.fetchone()
race_id = race_row[0] if race_row else 779
print(f"Testing for RaceID: {race_id} ({race_row[1] if race_row else 'Unknown'})")

sql = """
SELECT 
    t.TechSystemID,
    t.Name,
    tt.FieldID,
    rf.FieldName,
    t.DevelopCost
FROM FCT_TechSystem t
JOIN DIM_TechType tt ON t.TechTypeID = tt.TechTypeID
JOIN DIM_ResearchField rf ON tt.FieldID = rf.ResearchFieldID
WHERE t.TechSystemID NOT IN (SELECT TechID FROM FCT_RaceTech WHERE RaceID = ?)
  AND (t.RaceID = 0 OR t.RaceID = ?)
  AND (t.Prerequisite1 = 0 OR t.Prerequisite1 IN (SELECT TechID FROM FCT_RaceTech WHERE RaceID = ?))
  AND (t.Prerequisite2 = 0 OR t.Prerequisite2 IN (SELECT TechID FROM FCT_RaceTech WHERE RaceID = ?))
  AND t.AutomaticResearch = 0
ORDER BY tt.FieldID, t.DevelopCost ASC
"""

cursor.execute(sql, (race_id, race_id, race_id, race_id))
rows = cursor.fetchall()
print(f"\nTotal Researchable Techs found: {len(rows)}")

field_counts = {}
for r in rows:
    fid = r[2]
    fname = r[3]
    field_counts[(fid, fname)] = field_counts.get((fid, fname), 0) + 1

print("\n--- Researchable Techs per Research Field ---")
for (fid, fname), count in sorted(field_counts.items()):
    print(f"FieldID {fid} ({fname}): {count} researchable techs")

print("\n--- Sample Construction / Production (FieldID 5) & Logistics (FieldID 6) Techs ---")
for r in rows:
    if r[2] in (5, 6):
        print(f"  [Field {r[2]}] {r[1]} (Cost: {r[4]} RP)")

conn.close()
