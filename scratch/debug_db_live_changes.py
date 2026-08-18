import sqlite3
import time

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"

# Check journal mode
conn = sqlite3.connect(db_path)
cursor = conn.cursor()
cursor.execute("PRAGMA journal_mode")
mode = cursor.fetchone()
print(f"Current SQLite Journal Mode: {mode[0]}")

cursor.execute("PRAGMA busy_timeout = 5000")
cursor.execute("PRAGMA synchronous = NORMAL")

print("\n--- Testing write and immediate read on AuroraDB.db ---")
try:
    cursor.execute("SELECT GameID, GameTime FROM FCT_Game LIMIT 1")
    game_row = cursor.fetchone()
    print(f"GameID: {game_row[0]}, GameTime: {game_row[1]}")
except Exception as e:
    print(f"Error: {e}")

conn.close()
