import sqlite3
import os

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
if not os.path.exists(db_path):
    db_path = r"C:\VSCODE\AuroraDesignSuite\bin\Debug\net7.0-windows\AuroraDB.db"

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

cursor.execute("PRAGMA table_info(FCT_ShipClass);")
cols = cursor.fetchall()
print("FCT_ShipClass columns related to Fuel:")
for c in cols:
    if "Fuel" in c[1] or "Cap" in c[1]:
        print(f"  {c[1]} ({c[2]})")

cursor.execute("""
    SELECT p.PopulationID, p.PopName, p.FuelStockpile, p.Sorium
    FROM FCT_Population p
    WHERE p.RaceID = 1
    ORDER BY p.FuelStockpile DESC
    LIMIT 10
""")
print("\nSample Colony Fuel Stockpiles:")
for r in cursor.fetchall():
    print(r)

cursor.execute("""
    SELECT s.ShipID, s.ShipName, s.Fuel, sc.FuelCapacity, f.FleetName
    FROM FCT_Ship s
    JOIN FCT_ShipClass sc ON s.ShipClassID = sc.ShipClassID
    LEFT JOIN FCT_Fleet f ON s.FleetID = f.FleetID
    WHERE s.RaceID = 1
    LIMIT 10
""")
print("\nSample Ship Fuel Stockpiles:")
for r in cursor.fetchall():
    print(r)

conn.close()
