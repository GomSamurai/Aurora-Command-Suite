import os
import sqlite3
import time

print("--- Searching for ALL AuroraDB files across C: drive ---")

search_roots = [
    r"C:\Users\Fran\Desktop",
    r"c:\VSCODE",
    r"C:\Users\Fran\Downloads",
    r"C:\Users\Fran\Documents",
    r"C:\Users\Fran\AppData\Local",
    r"C:\Users\Fran\AppData\Roaming",
    r"C:\Users\Fran\.gemini"
]

found_files = []

for sroot in search_roots:
    if not os.path.exists(sroot): continue
    for root, dirs, files in os.walk(sroot):
        for f in files:
            if "auroradb" in f.lower() or f.lower().endswith(".db") or f.lower().endswith(".bak"):
                full_path = os.path.join(root, f)
                try:
                    size = os.path.getsize(full_path)
                    if size > 5000000: # Only SQLite DBs larger than 5MB
                        mtime = os.path.getmtime(full_path)
                        conn = sqlite3.connect(full_path)
                        cursor = conn.cursor()
                        cursor.execute("SELECT GameID, GameName, GameTime FROM FCT_Game WHERE GameID = 140 OR GameName LIKE '%Hexa%'")
                        game_row = cursor.fetchone()
                        
                        pop_row = None
                        if game_row:
                            cursor.execute("SELECT PopID, PopName, Population FROM FCT_Population WHERE GameID = ?", (game_row[0],))
                            pop_row = cursor.fetchall()
                        
                        conn.close()
                        
                        if game_row:
                            gtime = game_row[2]
                            days = gtime / 86400.0
                            year = 2026 + (days / 365.25)
                            found_files.append({
                                'path': full_path,
                                'mtime': time.ctime(mtime),
                                'size': size,
                                'game_time': gtime,
                                'estimated_year': f"{year:.2f}",
                                'pops': pop_row
                            })
                except Exception as e:
                    pass

print(f"\nFOUND {len(found_files)} POTENTIAL SAVE DATABASES:")
# Sort by game_time descending to find the MOST ADVANCED save!
found_files.sort(key=lambda x: x['game_time'], reverse=True)

for i, ff in enumerate(found_files):
    print(f"\n[{i+1}] Path: {ff['path']}")
    print(f"    Modified: {ff['mtime']} | Size: {ff['size']} bytes")
    print(f"    GameTime: {ff['game_time']} sec (~Year {ff['estimated_year']})")
    print(f"    Populations: {ff['pops']}")
