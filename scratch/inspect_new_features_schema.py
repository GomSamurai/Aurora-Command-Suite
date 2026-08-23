import sqlite3
import os

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
if not os.path.exists(db_path):
    # Fallback to local workspace db
    db_path = r"C:\VSCODE\AuroraDesignSuite\bin\Debug\net7.0-windows\AuroraDB.db"

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

cursor.execute("SELECT name FROM sqlite_master WHERE type='table';")
tables = [row[0] for row in cursor.fetchall()]

print("--- GROUND FORCES TABLES ---")
gf_tables = [t for t in tables if "ground" in t.lower() or "formation" in t.lower() or "unit" in t.lower() or "sto" in t.lower()]
print(gf_tables)

print("\n--- OFFICERS & MEDALS TABLES ---")
off_tables = [t for t in tables if "commander" in t.lower() or "medal" in t.lower() or "memorial" in t.lower() or "honor" in t.lower()]
print(off_tables)

print("\n--- ALIENS & CONTACTS TABLES ---")
alien_tables = [t for t in tables if "alien" in t.lower() or "contact" in t.lower() or "npr" in t.lower() or "intel" in t.lower() or "race" in t.lower()]
print(alien_tables)

print("\n--- TERRAFORMING & ATMOSPHERE TABLES ---")
terra_tables = [t for t in tables if "terra" in t.lower() or "atmospher" in t.lower() or "gas" in t.lower() or "pop" in t.lower()]
print(terra_tables)

conn.close()
