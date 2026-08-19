import sqlite3

db_path = r"C:\Users\Fran\Desktop\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
cursor = conn.cursor()

print("--- Population Installations in Desktop AuroraDB.db ---")
cursor.execute("""
    SELECT pi.PopulationID, i.Name, pi.Amount 
    FROM FCT_PopulationInstallations pi
    JOIN FCT_Installations i ON pi.InstallationID = i.InstallationID
    WHERE pi.PopulationID = 48977
""")
for row in cursor.fetchall():
    print("  ", row)

conn.close()
