import sqlite3
import os

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"

print("--- DB File Check ---")
print(f"AuroraDB.db exists: {os.path.exists(db_path)}")
print(f"AuroraDB.db-wal exists: {os.path.exists(db_path + '-wal')}")
print(f"AuroraDB.db-shm exists: {os.path.exists(db_path + '-shm')}")

if os.path.exists(db_path + '-wal'):
    print(f"WAL file size: {os.path.getsize(db_path + '-wal')} bytes")

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

print("\n--- FCT_Game state in DB ---")
try:
    for row in cursor.execute("SELECT GameID, GameTime FROM FCT_Game"):
        print(f"GameID: {row[0]}, GameTime (Seconds): {row[1]}")
except Exception as e:
    print(e)

print("\n--- Population Installations (Mines) in DB ---")
# Category 4 = Conventional Mine, Category 1 = Mine, etc.
try:
    sql = """
    SELECT p.PopID, p.PopulationName, pi.PlanetaryInstallationID, pi.Amount 
    FROM FCT_PopulationInstallations pi
    JOIN FCT_Population p ON pi.PopulationID = p.PopID
    WHERE pi.PlanetaryInstallationID IN (1, 4, 5)
    """
    for row in cursor.execute(sql):
        print(row)
except Exception as e:
    print(e)

conn.close()
