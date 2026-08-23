import sqlite3
import os

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
if not os.path.exists(db_path):
    db_path = r"C:\VSCODE\AuroraDesignSuite\bin\Debug\net7.0-windows\AuroraDB.db"

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

cursor.execute("PRAGMA table_info(FCT_Gas)")
print("FCT_Gas columns:", cursor.fetchall())

cursor.execute("SELECT GasID, GasName, Dangerous, AntiDangerous, FreezeTemp, BoilingTemp FROM FCT_Gas LIMIT 15")
rows = cursor.fetchall()
print("\nSample Gases:")
for r in rows:
    print(r)

cursor.execute("""
    SELECT p.PopulationID, p.PopName, p.Population, p.LastColonyCost, sb.SurfaceTemp, sb.AtmosPress, sb.Gravity, sb.HydroExt
    FROM FCT_Population p
    JOIN FCT_SystemBody sb ON p.SystemBodyID = sb.SystemBodyID
    WHERE p.RaceID = 1
    LIMIT 10
""")

prows = cursor.fetchall()
print("\nSample Populations with SystemBody climate:")
for pr in prows:
    print(pr)

conn.close()
