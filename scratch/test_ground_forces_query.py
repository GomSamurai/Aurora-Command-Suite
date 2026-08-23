import sqlite3
import os

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
if not os.path.exists(db_path):
    db_path = r"C:\VSCODE\AuroraDesignSuite\bin\Debug\net7.0-windows\AuroraDB.db"

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

cursor.execute("""
    SELECT f.FormationID, f.Name, f.Abbreviation, p.PopName,
           SUM(e.Units * c.Size) as TotalSizeTons,
           SUM(e.Units * c.Cost) as TotalCost,
           SUM(e.Units) as TotalUnits
    FROM FCT_GroundUnitFormation f
    LEFT JOIN FCT_Population p ON f.PopulationID = p.PopulationID
    LEFT JOIN FCT_GroundUnitFormationElement e ON f.FormationID = e.FormationID
    LEFT JOIN FCT_GroundUnitClass c ON e.ClassID = c.GroundUnitClassID
    GROUP BY f.FormationID
    LIMIT 10
""")

rows = cursor.fetchall()
print("Sample Formations:")
for r in rows:
    print(r)

conn.close()
