import sqlite3

db_path = r"C:\Users\Fran\Desktop\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
cursor = conn.cursor()

cursor.execute("SELECT name FROM sqlite_master WHERE type='table' AND (name LIKE '%Min%' OR name LIKE '%Fuel%')")
print("Tables:", cursor.fetchall())

cursor.execute("PRAGMA table_info(FCT_PopMinerals)")
print("FCT_PopMinerals cols:", cursor.fetchall())

cursor.execute("SELECT * FROM FCT_PopMinerals WHERE PopID = 48977")
print("Earth minerals:", cursor.fetchall())

conn.close()
