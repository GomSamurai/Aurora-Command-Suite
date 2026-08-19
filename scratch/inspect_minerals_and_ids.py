import sqlite3

db_path = r"C:\Users\Fran\Desktop\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
cursor = conn.cursor()

print("--- DIM_PlanetaryInstallation IDs ---")
cursor.execute("SELECT PlanetaryInstallationID, Name FROM DIM_PlanetaryInstallation WHERE Name IN ('Mine', 'Automated Mine', 'Construction Factory', 'Conventional Industry', 'Financial Centre')")
for r in cursor.fetchall():
    print("  ", r)

print("\n--- FCT_PopulationMinerals for Earth ---")
cursor.execute("PRAGMA table_info(FCT_PopulationMinerals)")
print("Cols:", cursor.fetchall())

cursor.execute("SELECT * FROM FCT_PopulationMinerals WHERE PopID = 48977")
print("Minerals:", cursor.fetchall())

conn.close()
