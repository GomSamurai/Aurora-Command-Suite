import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

print("--- FCT_Commander Columns ---")
cursor.execute("PRAGMA table_info(FCT_Commander)")
cols = cursor.fetchall()
for c in cols:
    print(f"  {c['name']} ({c['type']})")

print("\n--- FCT_Commander Sample Data ---")
cursor.execute("SELECT CommanderID, Name, Title, RankID, CommandType, CommandID, TransportShipID, RaceID, SpeciesID FROM FCT_Commander LIMIT 10")
cmds = cursor.fetchall()
for c in cmds:
    print(" ", dict(c))

print("\n--- FCT_Fleet Columns ---")
cursor.execute("PRAGMA table_info(FCT_Fleet)")
for c in cursor.fetchall():
    print(f"  {c['name']} ({c['type']})")

print("\n--- FCT_Ship Columns ---")
cursor.execute("PRAGMA table_info(FCT_Ship)")
for c in cursor.fetchall():
    print(f"  {c['name']} ({c['type']})")

print("\n--- FCT_CommanderBonuses Sample Data ---")
cursor.execute("SELECT cb.*, bt.BonusName FROM FCT_CommanderBonuses cb LEFT JOIN DIM_CommanderBonusType bt ON cb.BonusID = bt.BonusID LIMIT 10")
bonuses = cursor.fetchall()
for b in bonuses:
    print(" ", dict(b))

conn.close()
