import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path, timeout=10.0)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

try:
    cursor.execute("UPDATE FCT_Race SET FlagPic = 'flag0517.jpg' WHERE RaceID = 784")
    conn.commit()
    print("SUCCESS: FlagPic updated to flag0517.jpg in AuroraDB.db")
except Exception as e:
    print("FAILURE:", e)

cursor.execute("SELECT FlagPic FROM FCT_Race WHERE RaceID = 784")
print("Current FlagPic in DB:", cursor.fetchone()['FlagPic'])

conn.close()
