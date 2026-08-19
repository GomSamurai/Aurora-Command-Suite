import sqlite3
import os

target_dbs = [
    r"C:\Users\Fran\Desktop\Aurora271Full\AuroraDB.db",
    r"c:\VSCODE\Aurora271Full\AuroraDB.db"
]

print("--- RESTORING FEB 11 2041 SAVE GAME STATE FROM SCREENSHOT ---")

for db in target_dbs:
    if os.path.exists(db):
        print(f"\nUpdating database: {db}")
        conn = sqlite3.connect(db)
        cursor = conn.cursor()
        
        # 1. Update GameTime to Feb 11, 2041 (476,718,615 seconds)
        cursor.execute("UPDATE FCT_Game SET GameTime = 476718615.0 WHERE GameID = 140")
        
        # 2. Update Earth Population, Fuel, Maint Supplies & Minerals
        cursor.execute("""
            UPDATE FCT_Population 
            SET Population = 1405.82,
                FuelStockpile = 55025120.0,
                MaintenanceStockpile = 2058.0,
                Duranium = 43595.0,
                Neutronium = 49780.0,
                Corbomite = 17653.0,
                Tritanium = 35690.0,
                Boronide = 45618.0,
                Mercassium = 49029.0,
                Vendarite = 20709.0,
                Sorium = 8543.0,
                Uridium = 18945.0,
                Corundium = 20742.0,
                Gallicite = 50152.0
            WHERE PopulationID = 48977
        """)
        
        # 3. Update Earth Installations safely
        installations_to_update = {
            5: 154.0,  # Construction Factory
            7: 70.0,   # Mine
            12: 5.0,   # Automated Mine
            25: 18.0,  # Financial Centre
            38: 1441.0 # Conventional Industry
        }
        
        for inst_id, amount in installations_to_update.items():
            cursor.execute("DELETE FROM FCT_PopulationInstallations WHERE PopID = 48977 AND PlanetaryInstallationID = ?", (inst_id,))
            cursor.execute("INSERT INTO FCT_PopulationInstallations (GameID, PopID, PlanetaryInstallationID, Amount) VALUES (140, 48977, ?, ?)", (inst_id, amount))
        
        conn.commit()
        conn.close()
        print("Successfully restored 2041 save state for", db)
