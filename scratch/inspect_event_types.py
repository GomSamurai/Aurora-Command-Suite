import sqlite3
import os

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
if not os.path.exists(db_path):
    db_path = r"C:\VSCODE\AuroraDesignSuite\bin\Debug\net7.0-windows\AuroraDB.db"

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

cursor.execute("SELECT DISTINCT EventType FROM FCT_GameLog")
types = cursor.fetchall()
print(f"Distinct EventType IDs in FCT_GameLog: {[t[0] for t in types]}")

for t in types:
    cursor.execute("SELECT MessageText FROM FCT_GameLog WHERE EventType = ? LIMIT 2", (t[0],))
    msgs = cursor.fetchall()
    print(f"EventType={t[0]}: {msgs}")

conn.close()
