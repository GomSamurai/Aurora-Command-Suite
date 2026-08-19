import sqlite3

db = r"C:\Users\Fran\Desktop\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db)
cursor = conn.cursor()

print("--- VERIFYING RESTORED 2041 SAVE STATE ---")
cursor.execute("SELECT GameID, GameName, GameTime FROM FCT_Game WHERE GameID = 140")
grow = cursor.fetchone()
gtime = grow[2]
days = gtime / 86400.0
yr = 2026 + (days / 365.25)
print(f"Game: {grow[1]} | GameTime: {gtime} sec (~Year {yr:.2f})")

cursor.execute("SELECT PopulationID, PopName, Population, FuelStockpile, MaintenanceStockpile, Duranium, Gallicite FROM FCT_Population WHERE PopulationID = 48977")
prow = cursor.fetchone()
print(f"Earth Pop: {prow[2]}m | Fuel: {prow[3]} | Maint: {prow[4]} | Duranium: {prow[5]} | Gallicite: {prow[6]}")

cursor.execute("""
    SELECT dpi.Name, pi.Amount 
    FROM FCT_PopulationInstallations pi
    JOIN DIM_PlanetaryInstallation dpi ON pi.PlanetaryInstallationID = dpi.PlanetaryInstallationID
    WHERE pi.PopID = 48977 AND pi.Amount > 0
    ORDER BY pi.Amount DESC
""")
print("\nEarth Installations:")
for r in cursor.fetchall():
    print("  ", r)

conn.close()
