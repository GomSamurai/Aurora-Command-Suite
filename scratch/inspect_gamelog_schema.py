import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

print("--- Inspecting Tables with 'Log' or 'Event' or 'Msg' ---")
cursor.execute("SELECT name FROM sqlite_master WHERE type='table' AND (name LIKE '%log%' OR name LIKE '%event%' OR name LIKE '%msg%' OR name LIKE '%message%')")
tables = [r['name'] for r in cursor.fetchall()]
print("Tables:", tables)

for t in tables:
    print(f"\n--- Schema of {t} ---")
    cursor.execute(f"PRAGMA table_info({t})")
    cols = [r['name'] for r in cursor.fetchall()]
    print("  Cols:", cols)
    cursor.execute(f"SELECT * FROM {t} ORDER BY 1 DESC LIMIT 5")
    rows = cursor.fetchall()
    print(f"  Sample rows ({len(rows)}):")
    for r in rows:
        print("   -", dict(r))

conn.close()
