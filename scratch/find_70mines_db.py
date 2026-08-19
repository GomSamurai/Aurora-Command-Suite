import os
import sqlite3
import time

print("--- SEARCHING FOR 70 MINES OR 154 CONSTRUCTION FACTORIES IN ALL DATABASES ---")

search_dirs = [
    r"C:\Users\Fran\Desktop",
    r"C:\Users\Fran\Downloads",
    r"C:\Users\Fran\Documents",
    r"c:\VSCODE",
    r"C:\Users\Fran\AppData\Local\Temp",
    r"C:\$Recycle.Bin"
]

for sdir in search_dirs:
    if not os.path.exists(sdir): continue
    for root, dirs, files in os.walk(sdir):
        for f in files:
            if f.lower().endswith(".db") or f.lower().endswith(".bak"):
                full_path = os.path.join(root, f)
                try:
                    size = os.path.getsize(full_path)
                    if size > 1000000:
                        conn = sqlite3.connect(full_path)
                        cursor = conn.cursor()
                        cursor.execute("SELECT PopID, Population FROM FCT_Population WHERE Population > 1300 AND Population < 1500")
                        pops = cursor.fetchall()
                        if pops:
                            print(f"\nFOUND MATCHING POPULATION IN: {full_path}")
                            print("   Pops:", pops)
                            print("   Modified:", time.ctime(os.path.getmtime(full_path)))
                            cursor.execute("SELECT GameID, GameName, GameTime FROM FCT_Game")
                            print("   Games:", cursor.fetchall())
                        conn.close()
                except Exception as e:
                    pass
