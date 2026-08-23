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

print_cols("FCT_Ship")
print_cols("FCT_Fleet")

cursor.execute("""
    SELECT PopulationID, PopName, FuelStockpile, Sorium, ReserveSorium
    FROM FCT_Population
    WHERE RaceID = 1 AND (FuelStockpile > 0 OR Sorium > 0)
    ORDER BY FuelStockpile DESC
    LIMIT 10
""")
print("\nSample Colony Fuel Stockpiles:")
for r in cursor.fetchall():
    print(r)

cursor.execute("""
    SELECT s.ShipID, s.ShipName, s.Fuel, s.FuelCapacity, f.FleetName
    FROM FCT_Ship s
    LEFT JOIN FCT_Fleet f ON s.FleetID = f.FleetID
    WHERE s.RaceID = 1
    ORDER BY s.Fuel DESC
    LIMIT 10
""")
print("\nSample Ship Fuel Stockpiles:")
for r in cursor.fetchall():
    print(r)

conn.close()
