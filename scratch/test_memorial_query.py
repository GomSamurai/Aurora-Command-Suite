import sqlite3
import os

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
if not os.path.exists(db_path):
    db_path = r"C:\VSCODE\AuroraDesignSuite\bin\Debug\net7.0-windows\AuroraDB.db"

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

cursor.execute("""
    SELECT c.CommanderID, c.Name, c.Deceased, c.RetireStatus, c.KillTonnageMilitary, c.KillTonnageCommercial, c.CareerStart
    FROM FCT_Commander c
    WHERE c.Deceased = 1 OR c.RetireStatus > 0
    LIMIT 10
""")

rows = cursor.fetchall()
print("Deceased / Retired Commanders count:", len(rows))
for r in rows:
    print(r)

conn.close()
