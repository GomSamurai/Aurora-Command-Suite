import sqlite3
import os

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
if not os.path.exists(db_path):
    db_path = r"C:\VSCODE\AuroraDesignSuite\bin\Debug\net7.0-windows\AuroraDB.db"

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

def print_cols(table):
    cursor.execute(f"PRAGMA table_info({table});")
    cols = cursor.fetchall()
    print(f"\n--- TABLE {table} ---")
    for c in cols:
        print(f"  {c[1]} ({c[2]})")

print_cols("FCT_AtmosphericGas")
print_cols("FCT_Gas")
print_cols("FCT_Population")
print_cols("FCT_SystemBody")

cursor.execute("""
    SELECT p.PopulationID, p.PopName, p.ColonyCost, p.AtmosphericPressure, p.SurfaceTemp, p.TerraformRate
    FROM FCT_Population p
    WHERE p.RaceID = 1 OR p.ColonyCost > 0
    LIMIT 10
""")

rows = cursor.fetchall()
print("\nSample Populations with Colony Cost / Terraforming:")
for r in rows:
    print(r)

conn.close()
