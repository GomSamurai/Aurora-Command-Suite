import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

print("--- FCT_Ranks Columns ---")
cursor.execute("PRAGMA table_info(FCT_Ranks)")
for c in cursor.fetchall():
    print(f"  {c['name']} ({c['type']})")

print("\n--- FCT_Ranks Sample Data ---")
cursor.execute("SELECT * FROM FCT_Ranks LIMIT 10")
for r in cursor.fetchall():
    print(" ", dict(r))

print("\n--- Check Commanders assigned to ships/fleets in Race 784 ---")
cursor.execute("""
    SELECT c.CommanderID, c.Name, c.Title, c.CommandType, c.CommandID, r.RankName, r.RankAbbrev,
           f.FleetName, s.ShipName
    FROM FCT_Commander c
    LEFT JOIN FCT_Ranks r ON c.RankID = r.RankID
    LEFT JOIN FCT_Fleet f ON c.CommandType = 2 AND c.CommandID = f.FleetID
    LEFT JOIN FCT_Ship s ON c.CommandType = 1 AND c.CommandID = s.ShipID
    WHERE c.RaceID = 784 AND (c.CommandType = 1 OR c.CommandType = 2)""")
rows = cursor.fetchall()
for r in rows:
    print(" ", dict(r))

conn.close()
