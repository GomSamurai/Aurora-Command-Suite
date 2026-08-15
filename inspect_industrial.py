import sqlite3

db_path = r"c:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
c = conn.cursor()

print("=== DIM_PlanetaryInstallation ===")
c.execute("PRAGMA table_info(DIM_PlanetaryInstallation)")
cols = [r[1] for r in c.fetchall()]
print("Columns:", cols)

c.execute("SELECT * FROM DIM_PlanetaryInstallation")
for row in c.fetchall():
    print(" ", row)

print("\n=== FCT_PopulationInstallations Schema ===")
c.execute("PRAGMA table_info(FCT_PopulationInstallations)")
for r in c.fetchall():
    print(" ", r)
