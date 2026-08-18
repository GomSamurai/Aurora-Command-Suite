import os
import sqlite3
import time

print("--- Searching all AuroraDB.db files across system ---")

found_dbs = []
search_paths = [r"C:\VSCODE", r"C:\Games", r"C:\Users\Fran\Desktop", r"C:\Users\Fran\Downloads"]

for base in search_paths:
    if os.path.exists(base):
        for root, dirs, files in os.walk(base):
            for f in files:
                if f.lower() == "auroradb.db":
                    full_path = os.path.join(root, f)
                    try:
                        mtime = os.path.getmtime(full_path)
                        size = os.path.getsize(full_path)
                        
                        # Get game time from each DB
                        conn = sqlite3.connect(full_path)
                        cursor = conn.cursor()
                        cursor.execute("SELECT GameTime FROM FCT_Game LIMIT 1")
                        gt = cursor.fetchone()
                        conn.close()
                        
                        gt_val = gt[0] if gt else 0
                        found_dbs.append((full_path, mtime, size, gt_val))
                    except Exception as e:
                        found_dbs.append((full_path, mtime, size, f"Error: {e}"))

print(f"Total AuroraDB.db files found: {len(found_dbs)}\n")
for path, mtime, size, gt in sorted(found_dbs, key=lambda x: x[1], reverse=True):
    print(f"File: {path}")
    print(f"   Last Modified: {time.ctime(mtime)}")
    print(f"   Size: {size} bytes")
    print(f"   GameTime in DB: {gt} seconds\n")
