import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

def category_matches(name, desc, type_id, category_name):
    cat = category_name.lower()
    name = name.lower()
    desc = desc.lower()

    if "engine" in cat or "motor" in cat:
        return (type_id in [119, 65, 127, 130, 198, 214]
                or ("engine" in name and "jump engine" not in name and "fire control" not in name)
                or ("engine" in desc and "jump engine" not in desc))
    if "active sensor" in cat:
        return type_id in [20, 152] or ("active" in name and "sensor" in name)
    if "laser" in cat:
        return type_id in [3, 1, 140] or "laser" in name or "laser" in desc
    if "shield" in cat:
        return type_id == 215 or "shield" in name or "shield" in desc
    if "missile launcher" in cat:
        return type_id in [10, 129, 216] or "launcher" in name

    return cat in name or cat in desc

print("--- Testing Filter for Category 'Engines' ---")
cursor.execute("""
    SELECT rt.TechID, ts.Name, ts.CategoryID, ts.TechTypeID, ts.TechDescription
    FROM FCT_RaceTech rt
    JOIN FCT_TechSystem ts ON rt.TechID = ts.TechSystemID
    WHERE rt.RaceID = 784""")
rows = cursor.fetchall()

engine_techs = [dict(r) for r in rows if category_matches(r['Name'], r['TechDescription'] or '', r['TechTypeID'], "Engines")]

print(f"Total Engine Techs Found: {len(engine_techs)}")
for t in engine_techs:
    print("  -", t['Name'], f"(Type #{t['TechTypeID']})")

conn.close()
