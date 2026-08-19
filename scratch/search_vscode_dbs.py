import os
import sqlite3
import time

vscode = r"c:\VSCODE"

print("--- Searching c:\\VSCODE for all DB files ---")
for root, dirs, files in os.walk(vscode):
    for f in files:
        if f.lower().endswith(".db") or f.lower().endswith(".bak"):
            full = os.path.join(root, f)
            try:
                size = os.path.getsize(full)
                mtime = os.path.getmtime(full)
                conn = sqlite3.connect(full)
                cursor = conn.cursor()
                cursor.execute("SELECT GameID, GameName, GameTime FROM FCT_Game WHERE GameID = 140 OR GameName LIKE '%Hexa%'")
                grow = cursor.fetchone()
                if grow:
                    print(f"\nFILE: {full}")
                    print(f"   Size: {size} bytes | Modified: {time.ctime(mtime)}")
                    print(f"   GameTime: {grow[2]} sec (~Year {2026 + (grow[2]/86400.0/365.25):.2f})")
                    cursor.execute("SELECT PopID, PopName, Population FROM FCT_Population WHERE GameID = 140 AND PopName LIKE '%Earth%'")
                    print("   Earth Pop:", cursor.fetchall())
                conn.close()
            except Exception as e:
                pass
