import sqlite3
import os

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
if not os.path.exists(db_path):
    db_path = r"C:\VSCODE\AuroraDesignSuite\bin\Debug\net7.0-windows\AuroraDB.db"

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

cursor.execute("SELECT RaceID, RaceName FROM FCT_Race")
races = cursor.fetchall()
print("All Races in DB:")
for r in races:
    cursor.execute("SELECT COUNT(*) FROM FCT_Fleet WHERE RaceID = ?", (r[0],))
    fc = cursor.fetchone()[0]
    cursor.execute("SELECT COUNT(*) FROM FCT_Ship s JOIN FCT_Fleet f ON s.FleetID = f.FleetID WHERE f.RaceID = ?", (r[0],))
    sc = cursor.fetchone()[0]
    print(f"RaceID={r[0]}, Name='{r[1]}', Fleets={fc}, Ships={sc}")

conn.close()
