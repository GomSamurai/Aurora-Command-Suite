import sqlite3
import os

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
if not os.path.exists(db_path):
    db_path = r"C:\VSCODE\AuroraDesignSuite\bin\Debug\net7.0-windows\AuroraDB.db"

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

cursor.execute("PRAGMA table_info(FCT_Game);")
print("FCT_Game columns:")
for c in cursor.fetchall():
    print(f"  {c[1]} ({c[2]})")

cursor.execute("SELECT GameID, GameName, GameTime, StartYear FROM FCT_Game LIMIT 5;")
print("\nFCT_Game sample:")
for r in cursor.fetchall():
    print(r)

conn.close()
