import sqlite3
import os

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
if not os.path.exists(db_path):
    db_path = r"C:\VSCODE\AuroraDesignSuite\bin\Debug\net7.0-windows\AuroraDB.db"

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

def print_cols(table):
    cursor.execute(f"PRAGMA table_info({table});")
    cols = cursor.fetchall()
    print(f"\n--- TABLE {table} ---")
    for c in cols:
        print(f"  {c[1]} ({c[2]})")

print_cols("FCT_AlienRace")
print_cols("FCT_AlienClass")
print_cols("FCT_AlienShip")
print_cols("FCT_Contacts")

cursor.execute("SELECT AlienRaceID, RaceID, AlienRaceName, Abbreviation, TranslationProgress FROM FCT_AlienRace LIMIT 10")
rows = cursor.fetchall()
print("\nSample Alien Races:")
for r in rows:
    print(r)

conn.close()
