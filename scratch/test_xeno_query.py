import sqlite3
import os

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
if not os.path.exists(db_path):
    db_path = r"C:\VSCODE\AuroraDesignSuite\bin\Debug\net7.0-windows\AuroraDB.db"

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

cursor.execute("""
    SELECT AlienRaceID, ViewRaceID, AlienRaceName, Abbrev, CommStatus, DiplomaticPoints, TradeTreaty
    FROM FCT_AlienRace
    LIMIT 10
""")

rows = cursor.fetchall()
print("Alien Races count:", len(rows))
for r in rows:
    print(r)

cursor.execute("""
    SELECT AlienClassID, AlienRaceID, ClassName, MaxSpeed, ThermalSignature, TCS, ArmourStrength, ShieldStrength, ShipCount
    FROM FCT_AlienClass
    LIMIT 10
""")

crows = cursor.fetchall()
print("\nAlien Classes count:", len(crows))
for cr in crows:
    print(cr)

conn.close()
