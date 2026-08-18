import sqlite3

db_path = r"C:\Users\Fran\Desktop\Aurora271Full\AuroraDB.db"

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

cursor.execute("SELECT RaceID, RaceTitle, GameID FROM FCT_Race WHERE GameID = 140")
races = cursor.fetchall()
print("Races for GameID 140 ('Hexaverso'):", races)

cursor.execute("SELECT PopulationID, PopName, Population FROM FCT_Population WHERE GameID = 140")
print("Populations for GameID 140:", cursor.fetchall())

conn.close()
