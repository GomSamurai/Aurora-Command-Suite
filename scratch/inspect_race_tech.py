import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

print("--- FCT_RaceTech Columns ---")
cursor.execute("PRAGMA table_info(FCT_RaceTech)")
for c in cursor.fetchall():
    print(f"  {c['name']} ({c['type']})")

print("\n--- FCT_RaceTech for Race 784 joined with FCT_TechSystem ---")
cursor.execute("""
    SELECT rt.TechID, rt.RaceID, ts.Name, ts.CategoryID, ts.DevelopCost, ts.AdditionalInfo
    FROM FCT_RaceTech rt
    JOIN FCT_TechSystem ts ON rt.TechID = ts.TechSystemID
    WHERE rt.RaceID = 784
    LIMIT 30""")
rows = cursor.fetchall()
for r in rows:
    print(" ", dict(r))

print("\n--- Check all FCT_TechSystem items created for Race 784 or custom ---")
cursor.execute("""
    SELECT ts.TechSystemID, ts.Name, ts.CategoryID, ts.DevelopCost, ts.AdditionalInfo
    FROM FCT_TechSystem ts
    LIMIT 30""")
for r in cursor.fetchall():
    print(" ", dict(r))

print("\n--- Check DIM_ComponentType ---")
cursor.execute("SELECT * FROM DIM_ComponentType LIMIT 20")
for r in cursor.fetchall():
    print(" ", dict(r))

conn.close()
