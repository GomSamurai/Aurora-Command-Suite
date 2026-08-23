import sqlite3
import os

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
if not os.path.exists(db_path):
    db_path = r"C:\VSCODE\AuroraDesignSuite\bin\Debug\net7.0-windows\AuroraDB.db"

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

cursor.execute("""
    SELECT gl.Time, gl.EventType, et.Description, gl.MessageText
    FROM FCT_GameLog gl
    LEFT JOIN DIM_EventType et ON gl.EventType = et.EventTypeID
    WHERE gl.RaceID = 1
    ORDER BY gl.Time DESC
    LIMIT 15
""")

rows = cursor.fetchall()
print("Sample GameLog Entries:")
for r in rows:
    print(r)

conn.close()
