import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

print("--- Inspecting Exact Tech Types for Engines ---")
cursor.execute("""
    SELECT rt.TechID, ts.Name, ts.TechTypeID
    FROM FCT_RaceTech rt
    JOIN FCT_TechSystem ts ON rt.TechID = ts.TechSystemID
    WHERE rt.RaceID = 784 AND (ts.TechTypeID IN (40, 119, 65, 127) OR ts.Name LIKE '%Engine%')
    ORDER BY ts.TechTypeID""")
for r in cursor.fetchall():
    print(f"  [Type #{r['TechTypeID']}] {r['Name']}")

print("\n--- Inspecting Exact Tech Types for Lasers ---")
cursor.execute("""
    SELECT rt.TechID, ts.Name, ts.TechTypeID
    FROM FCT_RaceTech rt
    JOIN FCT_TechSystem ts ON rt.TechID = ts.TechSystemID
    WHERE rt.RaceID = 784 AND (ts.TechTypeID IN (1, 3, 140) OR ts.Name LIKE '%Laser%')
    ORDER BY ts.TechTypeID""")
for r in cursor.fetchall():
    print(f"  [Type #{r['TechTypeID']}] {r['Name']}")

print("\n--- Inspecting Exact Tech Types for Active Sensors ---")
cursor.execute("""
    SELECT rt.TechID, ts.Name, ts.TechTypeID
    FROM FCT_RaceTech rt
    JOIN FCT_TechSystem ts ON rt.TechID = ts.TechSystemID
    WHERE rt.RaceID = 784 AND (ts.TechTypeID IN (20, 152, 139) OR ts.Name LIKE '%Sensor%')
    ORDER BY ts.TechTypeID""")
for r in cursor.fetchall():
    print(f"  [Type #{r['TechTypeID']}] {r['Name']}")

conn.close()
