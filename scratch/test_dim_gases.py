import sqlite3
import os

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
if not os.path.exists(db_path):
    db_path = r"C:\VSCODE\AuroraDesignSuite\bin\Debug\net7.0-windows\AuroraDB.db"

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

cursor.execute("SELECT GasID, Name, Symbol, BoilingPoint, GHGas, AntiGHGas, Dangerous FROM DIM_Gases LIMIT 15")
gases = cursor.fetchall()
print("Sample DIM_Gases:")
for g in gases:
    print(g)

cursor.execute("""
    SELECT p.PopulationID, p.PopName, p.Population, p.LastColonyCost, sb.SystemBodyID, sb.SurfaceTemp, sb.AtmosPress, sb.Gravity, sb.HydroExt
    FROM FCT_Population p
    JOIN FCT_SystemBody sb ON p.SystemBodyID = sb.SystemBodyID
    WHERE p.RaceID = 1
    LIMIT 10
""")

prows = cursor.fetchall()
print("\nSample Populations with SystemBody climate:")
for pr in prows:
    print(pr)
    sb_id = pr[4]
    cursor.execute("""
        SELECT ag.AtmosGasID, g.Name, ag.GasAtm, ag.FrozenOut
        FROM FCT_AtmosphericGas ag
        JOIN DIM_Gases g ON ag.AtmosGasID = g.GasID
        WHERE ag.SystemBodyID = ?
    """, (sb_id,))
    ag_rows = cursor.fetchall()
    print("  Atmospheric Gases:", ag_rows)

conn.close()
