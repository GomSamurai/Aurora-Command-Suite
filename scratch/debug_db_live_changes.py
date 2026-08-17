import os
import sqlite3
import psutil

print("--- AURORA 4X LIVE DB DIAGNOSTIC ---")

# Check running processes
aurora_procs = [p for p in psutil.process_iter(['pid', 'name']) if 'aurora' in p.info['name'].lower()]
print(f"Running Aurora processes: {[p.info for p in aurora_procs]}")

db_paths = [
    r"c:\VSCODE\Aurora271Full\AuroraDB.db",
    r"C:\VSCODE\Aurora_Command_Suite_v2.7.1_Portable\App\AuroraDB.db"
]

for db in db_paths:
    print(f"\nChecking: {db}")
    if os.path.exists(db):
        stat = os.stat(db)
        print(f"  Main DB Size: {stat.st_size} bytes, MTime: {stat.st_mtime}")
        wal = db + "-wal"
        shm = db + "-shm"
        print(f"  WAL File Exists? {os.path.exists(wal)} (Size: {os.stat(wal).st_size if os.path.exists(wal) else 0})")
        print(f"  SHM File Exists? {os.path.exists(shm)}")

        try:
            conn = sqlite3.connect(f"file:{db}?mode=ro", uri=True)
            cursor = conn.cursor()
            cursor.execute("SELECT GameID, GameName, GameTime FROM FCT_Game")
            rows = cursor.fetchall()
            print(f"  FCT_Game Rows: {rows}")
            conn.close()
        except Exception as e:
            print(f"  Error reading DB: {e}")
    else:
        print("  File DOES NOT EXIST!")
