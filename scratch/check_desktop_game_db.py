import os
import sqlite3
import shutil

path = r"C:\Users\Fran\Desktop\Aurora271Full"
db_path = os.path.join(path, "AuroraDB.db")

print("--- Inspecting files in Desktop folder ---")
for f in os.listdir(path):
    if f.endswith(".db") or f.endswith(".bak") or f.endswith(".log"):
        full = os.path.join(path, f)
        print(f"File: {f} | Size: {os.path.getsize(full)} bytes | Modified: {os.path.getmtime(full)}")

if os.path.exists(db_path):
    conn = sqlite3.connect(db_path)
    cursor = conn.cursor()
    cursor.execute("SELECT GameID, GameName, GameTime FROM FCT_Game")
    games = cursor.fetchall()
    print("\n--- Games in AuroraDB.db ---")
    for g in games:
        print("  ", g)
    
    cursor.execute("SELECT RaceID, RaceName, GameID FROM FCT_Race")
    races = cursor.fetchall()
    print("\n--- Races in AuroraDB.db ---")
    for r in races:
        print("  ", r)
    
    conn.close()
