import sqlite3

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
cursor = conn.cursor()

print("--- FCT_RaceTech PRAGMA ---")
try:
    cursor.execute("PRAGMA table_info(FCT_RaceTech)")
    for col in cursor.fetchall():
        print(col)
except Exception as e:
    print(e)

print("\n--- FCT_RaceTech sample rows ---")
try:
    for row in cursor.execute("SELECT * FROM FCT_RaceTech LIMIT 10"):
        print(row)
except Exception as e:
    print(e)

conn.close()
