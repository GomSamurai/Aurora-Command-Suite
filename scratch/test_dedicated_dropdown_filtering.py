import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

def get_engine_options(race_id):
    cursor.execute("""
        SELECT ts.TechSystemID, ts.Name, ts.TechTypeID
        FROM FCT_RaceTech rt
        JOIN FCT_TechSystem ts ON rt.TechID = ts.TechSystemID
        WHERE rt.RaceID = ? AND (ts.TechTypeID IN (40, 119) OR (ts.Name LIKE '%Engine%' AND ts.Name NOT LIKE '%Jump%' AND ts.Name NOT LIKE '%Fire%'))
        ORDER BY ts.Name""", (race_id,))
    engine_techs = cursor.fetchall()

    cursor.execute("""
        SELECT ts.TechSystemID, ts.Name, ts.TechTypeID
        FROM FCT_RaceTech rt
        JOIN FCT_TechSystem ts ON rt.TechID = ts.TechSystemID
        WHERE rt.RaceID = ? AND (ts.TechTypeID = 65 OR ts.Name LIKE '%Fuel Consumption%')
        ORDER BY ts.Name""", (race_id,))
    fuel_techs = cursor.fetchall()

    cursor.execute("""
        SELECT ts.TechSystemID, ts.Name, ts.TechTypeID
        FROM FCT_RaceTech rt
        JOIN FCT_TechSystem ts ON rt.TechID = ts.TechSystemID
        WHERE rt.RaceID = ? AND (ts.TechTypeID = 127 OR ts.Name LIKE '%Thermal Reduction%')
        ORDER BY ts.Name""", (race_id,))
    thermal_techs = cursor.fetchall()

    return engine_techs, fuel_techs, thermal_techs

e_techs, f_techs, t_techs = get_engine_options(784)

print("=== DEDICATED DROPDOWNS FOR ENGINES ===")
print("\n[Dropdown 1: Engine Propulsion Techs]")
for r in e_techs:
    print("  -", r['Name'])

print("\n[Dropdown 2: Fuel Consumption Techs]")
for r in f_techs:
    print("  -", r['Name'])

print("\n[Dropdown 3: Thermal Reduction Techs]")
for r in t_techs:
    print("  -", r['Name'])

conn.close()
