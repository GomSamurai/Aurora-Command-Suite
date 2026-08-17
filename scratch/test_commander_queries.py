import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

print("--- Check Commanders with CommandID > 0 ---")
cursor.execute("SELECT CommanderID, Name, CommandType, CommandID, RankID, RaceID FROM FCT_Commander WHERE CommandID > 0 OR CommandType > 0 LIMIT 15")
rows = cursor.fetchall()
for r in rows:
    print(" ", dict(r))

print("\n--- Check DIM_CommanderBonusType or FCT_CommanderBonuses ---")
cursor.execute("PRAGMA table_info(FCT_CommanderBonuses)")
for c in cursor.fetchall():
    print(f"  {c['name']} ({c['type']})")

cursor.execute("SELECT * FROM FCT_CommanderBonuses LIMIT 10")
for r in cursor.fetchall():
    print(" ", dict(r))

print("\n--- Check DIM_CommanderBonusType ---")
cursor.execute("PRAGMA table_info(DIM_CommanderBonusType)")
for c in cursor.fetchall():
    print(f"  {c['name']} ({c['type']})")

cursor.execute("SELECT * FROM DIM_CommanderBonusType LIMIT 10")
for r in cursor.fetchall():
    print(" ", dict(r))

conn.close()
