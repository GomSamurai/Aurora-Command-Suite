import sqlite3

db_path = r"c:\VSCODE\Aurora271Full\AuroraDB.db"

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

print("--- Games in c:\\VSCODE\\Aurora271Full\\AuroraDB.db ---")
cursor.execute("SELECT GameID, GameName, GameTime FROM FCT_Game")
for g in cursor.fetchall():
    print("  ", g)

print("\n--- Races in c:\\VSCODE\\Aurora271Full\\AuroraDB.db ---")
cursor.execute("SELECT RaceID, RaceTitle, GameID FROM FCT_Race")
for r in cursor.fetchall():
    print("  ", r)

print("\n--- Populations in c:\\VSCODE\\Aurora271Full\\AuroraDB.db ---")
cursor.execute("SELECT PopulationID, PopName, Population, GameID FROM FCT_Population WHERE GameID = 140")
for p in cursor.fetchall():
    print("  ", p)

conn.close()
