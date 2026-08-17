import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

print("--- Inspecting FCT_Race for RaceID 784 ---")
cursor.execute("SELECT * FROM FCT_Race WHERE RaceID = 784")
race = cursor.fetchone()
if race:
    print(dict(race))

print("\n--- Inspecting All Tables in AuroraDB with 'Race' or 'Flag' ---")
cursor.execute("SELECT name FROM sqlite_master WHERE type='table' AND (name LIKE '%Race%' OR name LIKE '%Flag%')")
for r in cursor.fetchall():
    print(" Table:", r['name'])

conn.close()
