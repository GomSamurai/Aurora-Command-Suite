import sqlite3

db_path = r"C:\Users\Fran\Desktop\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
cursor = conn.cursor()

cursor.execute("SELECT name FROM sqlite_master WHERE type='table' AND name LIKE '%Instal%' OR name LIKE '%Pop%'")
print("Tables:", cursor.fetchall())

cursor.execute("SELECT * FROM FCT_PopulationInstallations WHERE PopulationID = 48977")
for r in cursor.fetchall():
    print(r)

conn.close()
