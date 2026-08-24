import sqlite3
import os

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
if not os.path.exists(db_path):
    db_path = r"C:\VSCODE\AuroraDesignSuite\bin\Debug\net7.0-windows\AuroraDB.db"

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

cursor.execute("PRAGMA table_info(FCT_GameLog)")
cols = cursor.fetchall()
print("FCT_GameLog Columns:")
for c in cols:
    print(f"  - {c[1]} ({c[2]})")

cursor.execute("SELECT * FROM FCT_GameLog LIMIT 10")
rows = cursor.fetchall()
print("\nSample rows:")
for r in rows:
    print(r)

conn.close()
