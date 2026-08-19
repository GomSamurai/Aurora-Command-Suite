import os
import sqlite3
import time

print("--- DEEP SEARCH FOR YEAR 2041 DATABASE (70 Mines, 154 Construction Factories) ---")

search_dirs = [
    r"C:\Users\Fran\Desktop",
    r"C:\Users\Fran\Downloads",
    r"C:\Users\Fran\Documents",
    r"c:\VSCODE",
    r"C:\Users\Fran\AppData",
    r"C:\$Recycle.Bin"
]

found_matches = []

for sdir in search_dirs:
    if not os.path.exists(sdir): continue
    for root, dirs, files in os.walk(sdir):
        for f in files:
            ext = os.path.splitext(f)[1].lower()
            if ext in ['.db', '.bak', '.tmp', '.sqlite', '.old', ''] or 'aurora' in f.lower():
                full_path = os.path.join(root, f)
                try:
                    size = os.path.getsize(full_path)
                    if size > 1000000: # Files larger than 1MB
                        conn = sqlite3.connect(full_path)
                        cursor = conn.cursor()
                        # Query FCT_Game for GameTime > 400000000
                        cursor.execute("SELECT GameID, GameName, GameTime FROM FCT_Game")
                        games = cursor.fetchall()
                        for g in games:
                            gtime = g[2]
                            if gtime > 400000000: # ~Year 2038-2045
                                days = gtime / 86400.0
                                yr = 2026 + (days / 365.25)
                                found_matches.append((full_path, g, size, os.path.getmtime(full_path), yr))
                                print(f"MATCH FOUND: {full_path} | Game: {g} | Year: {yr:.2f}")
                        conn.close()
                except Exception as e:
                    pass

print("\n--- ALL MATCHES OVER YEAR 2038 ---")
for m in found_matches:
    print(m)
