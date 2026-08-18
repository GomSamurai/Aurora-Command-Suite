import sqlite3

db_path = r"C:\Users\Fran\Desktop\Aurora271Full\AuroraDB.db"

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

cursor.execute("""
    SELECT pi.PlanetaryInstallationID, dim.Name, pi.Amount 
    FROM FCT_PopulationInstallations pi
    JOIN DIM_PlanetaryInstallation dim ON pi.PlanetaryInstallationID = dim.PlanetaryInstallationID
    WHERE pi.PopID = 48977
""")
print(f"Installations for Earth (PopID 48977):")
for r in cursor.fetchall():
    print(f"   {r[1]} (ID {r[0]}): {r[2]}")

conn.close()
