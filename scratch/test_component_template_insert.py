import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

print("--- Inspecting existing custom designs in FCT_ShipComponentTemplate ---")
cursor.execute("SELECT * FROM FCT_ShipComponentTemplate LIMIT 5")
for r in cursor.fetchall():
    print(" ", dict(r))

print("\n--- Inspecting existing designs in FCT_MissileType ---")
cursor.execute("SELECT * FROM FCT_MissileType LIMIT 5")
for r in cursor.fetchall():
    print(" ", dict(r))

print("\n--- Inspecting existing designs in FCT_TechSystem ---")
cursor.execute("SELECT * FROM FCT_TechSystem ORDER BY TechSystemID DESC LIMIT 5")
for r in cursor.fetchall():
    print(" ", dict(r))

conn.close()
