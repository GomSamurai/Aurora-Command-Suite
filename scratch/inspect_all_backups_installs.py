import os
import sqlite3

backup_files = [
    r"C:\Users\Fran\Desktop\Aurora271Full\AuroraDB.db",
    r"C:\Users\Fran\Desktop\Aurora271Full\AuroraDBSaveBackup.db",
    r"C:\Users\Fran\Desktop\Aurora271Full\AuroraDBPreviousSaveBackup.db",
    r"c:\VSCODE\Aurora271Full\AuroraDB.db",
    r"c:\VSCODE\Aurora271Full\AuroraDB.db.bak",
    r"c:\VSCODE\Aurora271Full\AuroraDBSaveBackup.db",
    r"c:\VSCODE\Aurora271Full\AuroraDBPreviousSaveBackup.db"
]

for bf in backup_files:
    if os.path.exists(bf):
        print(f"\n--- FILE: {bf} ---")
        try:
            conn = sqlite3.connect(bf)
            cursor = conn.cursor()
            cursor.execute("SELECT GameID, GameName, GameTime FROM FCT_Game WHERE GameID = 140")
            print("   Game:", cursor.fetchall())
            cursor.execute("""
                SELECT pi.PopID, dpi.Name, pi.Amount 
                FROM FCT_PopulationInstallations pi
                JOIN DIM_PlanetaryInstallation dpi ON pi.PlanetaryInstallationID = dpi.PlanetaryInstallationID
                WHERE pi.PopID = 48977
            """)
            print("   Earth Installations:", cursor.fetchall())
            conn.close()
        except Exception as e:
            print("   Error:", e)
