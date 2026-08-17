import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

print("--- Testing Companies Extraction for Race 784 ---")
cursor.execute("""
    SELECT DISTINCT ts.Name
    FROM FCT_RaceTech rt
    JOIN FCT_TechSystem ts ON rt.TechID = ts.TechSystemID
    WHERE rt.RaceID = 784 AND (ts.Name LIKE '%Limited%' OR ts.Name LIKE '%Turbines%' OR ts.Name LIKE '%Corp%' OR ts.Name LIKE '%Inc%')""")
rows = cursor.fetchall()
for r in rows:
    print("  - Company Tech:", r['Name'])

print("\n--- Inspecting All 621 Naming Themes ---")
cursor.execute("SELECT ThemeID, Description FROM DIM_NamingThemeTypes ORDER BY Description")
themes = cursor.fetchall()
print(f"Total Naming Themes Loaded: {len(themes)}")
print("Sample Spanish & Warhammer Themes:")
for t in themes:
    desc = t['Description']
    if 'Spanish' in desc or 'Warhammer' in desc or 'Battle' in desc or 'City' in desc or 'Myth' in desc:
        print(f"  [{t['ThemeID']}] {desc}")

conn.close()
