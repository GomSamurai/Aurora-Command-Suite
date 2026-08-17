import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

print("--- FCT_Species Schema ---")
cursor.execute("PRAGMA table_info(FCT_Species)")
for r in cursor.fetchall():
    print("  -", r['name'])

print("\n--- FCT_Species Rows for Game 2026 ---")
cursor.execute("SELECT * FROM FCT_Species")
rows = cursor.fetchall()
for r in rows:
    print("  - Species:", dict(r))

print("\n--- FCT_Population SpeciesID for Race 784 ---")
cursor.execute("SELECT DISTINCT SpeciesID FROM FCT_Population WHERE RaceID = 784")
for r in cursor.fetchall():
    print("  - Pop SpeciesID:", dict(r))

print("\n--- FCT_Race Columns with 'Spec' ---")
cursor.execute("PRAGMA table_info(FCT_Race)")
cols = [r['name'] for r in cursor.fetchall() if 'spec' in r['name'].lower() or 'race' in r['name'].lower()]
print("  - Matching cols in FCT_Race:", cols)

conn.close()
