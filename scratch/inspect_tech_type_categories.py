import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

print("--- Inspecting TechTypeID vs Name for Race 784 in FCT_RaceTech ---")
cursor.execute("""
    SELECT DISTINCT ts.TechTypeID, ts.CategoryID, ts.Name
    FROM FCT_RaceTech rt
    JOIN FCT_TechSystem ts ON rt.TechID = ts.TechSystemID
    WHERE rt.RaceID = 784
    ORDER BY ts.TechTypeID""")
rows = cursor.fetchall()

type_map = {}
for r in rows:
    tt_id = r['TechTypeID']
    if tt_id not in type_map:
        type_map[tt_id] = []
    type_map[tt_id].append(r['Name'])

for tt_id, names in type_map.items():
    print(f"\nTechTypeID #{tt_id} (Sample: '{names[0]}'):")
    for n in names[:5]:
        print(f"  - {n}")

conn.close()
