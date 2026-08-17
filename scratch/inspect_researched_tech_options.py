import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

print("--- Researched Technologies for Race 784 in FCT_RaceTech ---")
cursor.execute("""
    SELECT rt.TechID, ts.Name, ts.CategoryID, ts.TechTypeID, ts.AdditionalInfo, ts.TechDescription
    FROM FCT_RaceTech rt
    JOIN FCT_TechSystem ts ON rt.TechID = ts.TechSystemID
    WHERE rt.RaceID = 784
    ORDER BY ts.CategoryID, ts.TechTypeID""")
rows = cursor.fetchall()
print(f"Total Researched Techs for Race 784: {len(rows)}")

# Group by category
categories = {}
for r in rows:
    cat = r['CategoryID']
    if cat not in categories:
        categories[cat] = []
    categories[cat].append(dict(r))

for cat_id, tech_list in categories.items():
    print(f"\nCategoryID #{cat_id} ({len(tech_list)} techs):")
    for t in tech_list[:8]:
        print(f"  [{t['TechID']}] {t['Name']} (Type: {t['TechTypeID']}, AddInfo: {t['AdditionalInfo']})")

conn.close()
