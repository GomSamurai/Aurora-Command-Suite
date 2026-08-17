import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

cursor.execute("SELECT name FROM sqlite_master WHERE type='table' AND name LIKE '%rank%'")
tables = cursor.fetchall()
print("Rank tables:")
for t in tables:
    print(" ", t['name'])

print("\nAll DIM tables:")
cursor.execute("SELECT name FROM sqlite_master WHERE type='table' AND name LIKE 'DIM_%'")
for t in cursor.fetchall():
    print(" ", t['name'])

print("\nAll FCT tables for Commander/Rank:")
cursor.execute("SELECT name FROM sqlite_master WHERE type='table' AND name LIKE 'FCT_%Rank%' OR name LIKE 'FCT_%Commander%'")
for t in cursor.fetchall():
    print(" ", t['name'])

conn.close()
