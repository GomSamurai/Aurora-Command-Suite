import sqlite3

db_path = r"C:\Users\Fran\Desktop\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
cursor = conn.cursor()

cursor.execute("PRAGMA table_info(FCT_PopulationInstallations)")
print("FCT_PopulationInstallations cols:", cursor.fetchall())

cursor.execute("PRAGMA table_info(DIM_PlanetaryInstallation)")
print("DIM_PlanetaryInstallation cols:", cursor.fetchall())

cursor.execute("""
    SELECT pi.PopID, dpi.Name, pi.Amount 
    FROM FCT_PopulationInstallations pi
    JOIN DIM_PlanetaryInstallation dpi ON pi.PlanetaryInstallationID = dpi.PlanetaryInstallationID
    WHERE pi.PopID = 48977
""")
print("\nEarth Installations:")
for r in cursor.fetchall():
    print("  ", r)

conn.close()
