import os
import sqlite3

search_dirs = [
    r"c:\VSCODE",
    r"C:\Users\Fran\Desktop",
    r"C:\Users\Fran\Downloads",
    r"C:\Users\Fran\Documents"
]

print("--- Searching for user's real game database with 'Imperio Epistocrático' ---")

for root_dir in search_dirs:
    if not os.path.exists(root_dir): continue
    for root, dirs, files in os.walk(root_dir):
        for f in files:
            if f.endswith(".db") or f.endswith(".bak"):
                full_path = os.path.join(root, f)
                try:
                    conn = sqlite3.connect(full_path)
                    cursor = conn.cursor()
                    cursor.execute("SELECT RaceID, RaceName, GameID FROM FCT_Race WHERE RaceName LIKE '%Epistocrático%' OR RaceName LIKE '%Luz%' OR RaceName LIKE '%Imperio%'")
                    races = cursor.fetchall()
                    if races:
                        print(f"FOUND REAL SAVE DB AT: {full_path}")
                        print("   Races:", races)
                        cursor.execute("SELECT GameID, GameName, GameTime FROM FCT_Game WHERE GameID = ?", (races[0][2],))
                        print("   Game:", cursor.fetchall())
                    conn.close()
                except Exception as e:
                    pass
