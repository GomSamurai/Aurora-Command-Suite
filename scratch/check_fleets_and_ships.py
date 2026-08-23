import sqlite3
import os

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
if not os.path.exists(db_path):
    db_path = r"C:\VSCODE\AuroraDesignSuite\bin\Debug\net7.0-windows\AuroraDB.db"

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

cursor.execute("SELECT RaceID, RaceName FROM FCT_Race")
races = cursor.fetchall()
print("Races:", races)

for race in races:
    r_id = race[0]
    cursor.execute("SELECT FleetID, FleetName, SystemID FROM FCT_Fleet WHERE RaceID = ?", (r_id,))
    fleets = cursor.fetchall()
    print(f"\n--- Race {r_id} ({race[1]}) - Fleets Count: {len(fleets)} ---")
    for f in fleets:
        f_id = f[0]
        cursor.execute("SELECT COUNT(*) FROM FCT_Ship WHERE FleetID = ?", (f_id,))
        sc = cursor.fetchone()[0]
        cursor.execute("SELECT ShipID, ShipName FROM FCT_Ship WHERE FleetID = ?", (f_id,))
        ships = cursor.fetchall()
        print(f"  Fleet {f_id}: '{f[1]}' (Ships count: {sc})")
        for s in ships:
            print(f"    - Ship {s[0]}: {s[1]}")

conn.close()
