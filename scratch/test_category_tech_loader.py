import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

def get_researched_techs_for_category(race_id, category_name):
    print(f"\n--- Researched Techs for Race {race_id} under Category '{category_name}' ---")
    cursor.execute("""
        SELECT ts.TechSystemID, ts.Name, ts.CategoryID, ts.TechTypeID, ts.AdditionalInfo
        FROM FCT_RaceTech rt
        JOIN FCT_TechSystem ts ON rt.TechID = ts.TechSystemID
        WHERE rt.RaceID = ? AND (
            ts.Name LIKE ? OR ts.TechDescription LIKE ? OR ts.CategoryID = ?
        )
        ORDER BY ts.TechSystemID DESC""", (race_id, f"%{category_name}%", f"%{category_name}%", 1))
    rows = cursor.fetchall()
    print(f"Total Found: {len(rows)}")
    for r in rows[:10]:
        print(" ", dict(r))

get_researched_techs_for_category(784, "Engine")
get_researched_techs_for_category(784, "Sensor")
get_researched_techs_for_category(784, "Laser")
get_researched_techs_for_category(784, "Shield")

conn.close()
