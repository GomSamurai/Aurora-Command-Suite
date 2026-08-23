import sqlite3
import os

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
if not os.path.exists(db_path):
    db_path = r"C:\VSCODE\AuroraDesignSuite\bin\Debug\net7.0-windows\AuroraDB.db"

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

cursor.execute("SELECT name FROM sqlite_master WHERE type='table' AND (name LIKE '%Gas%' OR name LIKE '%Atm%')")
tables = cursor.fetchall()
print("Gas / Atmos tables:", tables)

for t in tables:
    tableName = t[0]
    cursor.execute(f"PRAGMA table_info({tableName});")
    print(f"\n--- {tableName} ---")
    print(cursor.fetchall())

conn.close()
