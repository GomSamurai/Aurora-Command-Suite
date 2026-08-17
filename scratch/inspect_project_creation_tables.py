import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

print("--- Tables related to Component Templates and Custom Designs ---")
cursor.execute("SELECT name FROM sqlite_master WHERE type='table' AND (name LIKE '%Component%' OR name LIKE '%Missile%' OR name LIKE '%Tech%' OR name LIKE '%Turret%' OR name LIKE '%Design%')")
for t in cursor.fetchall():
    print(" ", t['name'])

print("\n--- FCT_ShipComponentTemplate Columns ---")
cursor.execute("PRAGMA table_info(FCT_ShipComponentTemplate)")
for c in cursor.fetchall():
    print(f"  {c['name']} ({c['type']})")

print("\n--- Sample FCT_ShipComponentTemplate Data ---")
cursor.execute("""
    SELECT sct.ShipComponentTemplateID, sct.ComponentName, sct.ComponentValue, sct.ComponentSize, sct.RaceID,
           ct.Name as ComponentTypeName
    FROM FCT_ShipComponentTemplate sct
    LEFT JOIN DIM_ComponentType ct ON sct.ComponentTypeID = ct.ComponentTypeID
    LIMIT 20""")
for r in cursor.fetchall():
    print(" ", dict(r))

print("\n--- FCT_TechSystem Columns ---")
cursor.execute("PRAGMA table_info(FCT_TechSystem)")
for c in cursor.fetchall():
    print(f"  {c['name']} ({c['type']})")

print("\n--- Custom Created Tech Systems in FCT_TechSystem ---")
cursor.execute("""
    SELECT TechSystemID, Name, CategoryID, AdditionalInfo, DevelopCost, RaceID
    FROM FCT_TechSystem
    WHERE RaceID > 0 OR AdditionalInfo IS NOT NULL OR Name LIKE '%AS8%' OR Name LIKE '%Asturias%' OR Name LIKE '%Sensor%'
    LIMIT 20""")
for r in cursor.fetchall():
    print(" ", dict(r))

print("\n--- FCT_MissileType Data ---")
cursor.execute("SELECT * FROM FCT_MissileType LIMIT 10")
for r in cursor.fetchall():
    print(" ", dict(r))

conn.close()
