import os
import sqlite3

print("--- DEEP SEARCH FOR ANY DB FILE WITH AMOUNT = 70 OR AMOUNT = 154 ---")

search_roots = [
    r"C:\Users\Fran\Desktop",
    r"c:\VSCODE",
    r"C:\Users\Fran\Downloads",
    r"C:\Users\Fran\Documents",
    r"C:\Users\Fran\AppData",
    r"C:\$Recycle.Bin",
    r"C:\Users\Fran\.gemini"
]

found_2041_files = []

for sroot in search_roots:
    if not os.path.exists(sroot): continue
    for root, dirs, files in os.walk(sroot):
        for f in files:
            if f.lower().endswith(".db") or f.lower().endswith(".bak") or f.lower().endswith(".sqlite") or f.lower().endswith(".tmp"):
                full_path = os.path.join(root, f)
                try:
                    size = os.path.getsize(full_path)
                    if size > 5000000:
                        conn = sqlite3.connect(full_path)
                        cursor = conn.cursor()
                        cursor.execute("SELECT Amount FROM FCT_PopulationInstallations WHERE Amount = 70 OR Amount = 154")
                        rows = cursor.fetchall()
                        if rows:
                            print(f"FOUND FEB 2041 SAVE FILE: {full_path}")
                            print("   Matches:", rows)
                            cursor.execute("SELECT GameID, GameName, GameTime FROM FCT_Game WHERE GameID = 140")
                            print("   Game:", cursor.fetchall())
                            found_2041_files.append(full_path)
                        conn.close()
                except Exception as e:
                    pass

print("\n--- RESULT ---")
if found_2041_files:
    print("Found 2041 save files:", found_2041_files)
else:
    print("No DB file with 70 mines / 154 construction factories was found on disk.")
