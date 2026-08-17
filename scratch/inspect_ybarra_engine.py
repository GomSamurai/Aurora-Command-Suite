import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

print("--- Searching for 'Ybarra' in FCT_TechSystem ---")
cursor.execute("SELECT * FROM FCT_TechSystem WHERE Name LIKE '%Ybarra%' OR TechDescription LIKE '%Ybarra%'")
for r in cursor.fetchall():
    print("  FCT_TechSystem:", dict(r))

print("\n--- Searching for 'Ybarra' in FCT_ShipComponentTemplate ---")
cursor.execute("SELECT * FROM FCT_ShipComponentTemplate WHERE ComponentName LIKE '%Ybarra%'")
for r in cursor.fetchall():
    print("  FCT_ShipComponentTemplate:", dict(r))

print("\n--- Searching for 'Ybarra' in FCT_RaceTech ---")
cursor.execute("""
    SELECT rt.RaceID, rt.TechID, ts.Name, ts.TechDescription
    FROM FCT_RaceTech rt
    JOIN FCT_TechSystem ts ON rt.TechID = ts.TechSystemID
    WHERE ts.Name LIKE '%Ybarra%'""")
for r in cursor.fetchall():
    print("  FCT_RaceTech:", dict(r))

print("\n--- Checking RaceID for player's Empire in FCT_Race ---")
cursor.execute("SELECT RaceID, RaceTitle, GameID FROM FCT_Race")
for r in cursor.fetchall():
    print("  FCT_Race:", dict(r))

conn.close()
