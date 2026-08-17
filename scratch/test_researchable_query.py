import sqlite3

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
cursor = conn.cursor()

print("--- FCT_RaceTech sample rows ---")
try:
    for row in cursor.execute("SELECT RaceID, TechSystemID FROM FCT_RaceTech LIMIT 20"):
        print(row)
except Exception as e:
    print(e)

print("\n--- Check how many techs researched for RaceID 1 (or first race) ---")
try:
    for row in cursor.execute("SELECT RaceID, COUNT(*) FROM FCT_RaceTech GROUP BY RaceID"):
        print(row)
except Exception as e:
    print(e)

print("\n--- Query researchable technologies for RaceID 1 ---")
# Available tech for RaceID:
# 1. TechSystemID is NOT in FCT_RaceTech for this RaceID
# 2. Prerequisite1 is 0 OR Prerequisite1 is in FCT_RaceTech for this RaceID
# 3. Prerequisite2 is 0 OR Prerequisite2 is in FCT_RaceTech for this RaceID
# 4. AutomaticResearch is FALSE / 0
# 5. NoTechScan / RuinOnly checks
sql = """
SELECT t.TechSystemID, t.Name, tt.FieldID, rf.FieldName, t.DevelopCost, t.Prerequisite1, t.Prerequisite2
FROM FCT_TechSystem t
JOIN DIM_TechType tt ON t.TechTypeID = tt.TechTypeID
JOIN DIM_ResearchField rf ON tt.FieldID = rf.ResearchFieldID
WHERE t.TechSystemID NOT IN (SELECT TechSystemID FROM FCT_RaceTech WHERE RaceID = 1)
  AND (t.RaceID = 0 OR t.RaceID = 1)
  AND (t.Prerequisite1 = 0 OR t.Prerequisite1 IN (SELECT TechSystemID FROM FCT_RaceTech WHERE RaceID = 1))
  AND (t.Prerequisite2 = 0 OR t.Prerequisite2 IN (SELECT TechSystemID FROM FCT_RaceTech WHERE RaceID = 1))
  AND t.AutomaticResearch = 0
ORDER BY tt.FieldID, t.Name
LIMIT 30
"""
try:
    for row in cursor.execute(sql):
        print(row)
except Exception as e:
    print(e)

conn.close()
