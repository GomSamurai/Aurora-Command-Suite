import sqlite3
import os

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
if not os.path.exists(db_path):
    db_path = r"C:\VSCODE\AuroraDesignSuite\bin\Debug\net7.0-windows\AuroraDB.db"

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

cursor.execute("SELECT name FROM sqlite_master WHERE type='table' AND (name LIKE '%Log%' OR name LIKE '%Event%')")
tables = cursor.fetchall()
print("Log / Event tables:", tables)

for t in tables:
    tableName = t[0]
    cursor.execute(f"PRAGMA table_info({tableName});")
    print(f"\n--- {tableName} ---")
    print(cursor.fetchall())

cursor.execute("""
    SELECT GameLogID, Time, EventTypeID, MessageText
    FROM FCT_GameLog
    WHERE RaceID = 1 OR RaceID IS NULL
    ORDER BY Time DESC
    LIMIT 15
""")
print("\nSample GameLog Events:")
for r in cursor.fetchall():
    print(r)

conn.close()
