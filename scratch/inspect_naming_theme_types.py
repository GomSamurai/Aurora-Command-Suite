import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

print("--- Inspecting DIM_NamingThemeTypes (Sample 30) ---")
cursor.execute("SELECT ThemeID, Description FROM DIM_NamingThemeTypes ORDER BY Description LIMIT 40")
for r in cursor.fetchall():
    print(f"  [{r['ThemeID']}] {r['Description']}")

print("\n--- Inspecting Race 784 Theme Settings in FCT_Race ---")
cursor.execute("SELECT RaceID, RaceName, ThemeID, ClassThemeID, SystemThemeID, NameThemeID, DesignThemeID, GroundThemeID, MissileThemeID FROM FCT_Race WHERE RaceID = 784")
r = cursor.fetchone()
if r:
    print(dict(r))

print("\n--- Inspecting FCT_RaceNameThemes for Race 784 ---")
cursor.execute("""
    SELECT rnt.RaceID, rnt.NameThemeID, ntt.Description, rnt.Chance
    FROM FCT_RaceNameThemes rnt
    JOIN DIM_NamingThemeTypes ntt ON rnt.NameThemeID = ntt.ThemeID
    WHERE rnt.RaceID = 784""")
for row in cursor.fetchall():
    print("  -", dict(row))

conn.close()
