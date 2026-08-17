import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

print("--- Custom Created Projects in FCT_TechSystem for Race 784 ---")
cursor.execute("""
    SELECT TechSystemID, Name, CategoryID, TechDescription, DevelopCost, RaceID
    FROM FCT_TechSystem
    WHERE RaceID = 784 AND (TechDescription LIKE '%Race-designed%' OR TechDescription LIKE '%Custom%' OR CategoryID > 0)
    ORDER BY TechSystemID DESC""")
rows = cursor.fetchall()
print(f"Total Custom Projects Found for Race 784: {len(rows)}")
for r in rows:
    print(" ", dict(r))

print("\n--- Component Templates in FCT_ShipComponentTemplate ---")
cursor.execute("SELECT ShipComponentTemplateID, ComponentName, ComponentValue, ComponentSize FROM FCT_ShipComponentTemplate ORDER BY ShipComponentTemplateID DESC LIMIT 15")
for r in cursor.fetchall():
    print(" ", dict(r))

print("\n--- Saved Missiles in FCT_MissileType ---")
cursor.execute("SELECT MissileID, Name, Size, Speed, WarheadStrength, Cost FROM FCT_MissileType ORDER BY MissileID DESC LIMIT 15")
for r in cursor.fetchall():
    print(" ", dict(r))

conn.close()
