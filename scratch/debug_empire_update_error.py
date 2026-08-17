import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

print("--- Inspecting FCT_Race for RaceID 784 ---")
cursor.execute("SELECT RaceID, GameID, RaceName, RaceTitle, FlagPic, SpeciesID FROM FCT_Race WHERE RaceID = 784")
race = cursor.fetchone()
if race:
    print(dict(race))

print("\n--- Inspecting FCT_Species for GameID and SpeciesID ---")
cursor.execute("SELECT * FROM FCT_Species WHERE SpeciesID = ?", (race['SpeciesID'],))
spec = cursor.fetchone()
if spec:
    print("FCT_Species row:", dict(spec))
else:
    print(f"No species found with SpeciesID = {race['SpeciesID']}")

print("\n--- Testing SQL UPDATE on FCT_Race ---")
try:
    cursor.execute("UPDATE FCT_Race SET FlagPic = 'flag0517.jpg' WHERE RaceID = 784")
    print("FCT_Race update succeeded! Rows affected:", cursor.rowcount)
except Exception as e:
    print("FCT_Race update failed:", e)

print("\n--- Testing SQL UPDATE on FCT_Species ---")
try:
    if race['SpeciesID']:
        cursor.execute("UPDATE FCT_Species SET SpeciesName = 'Human' WHERE SpeciesID = ?", (race['SpeciesID'],))
        print("FCT_Species update succeeded! Rows affected:", cursor.rowcount)
except Exception as e:
    print("FCT_Species update failed:", e)

conn.close()
