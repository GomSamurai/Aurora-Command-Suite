import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

print("--- Inspecting Tables with 'Name', 'Theme', or 'Company' ---")
cursor.execute("SELECT name FROM sqlite_master WHERE type='table' AND (name LIKE '%Name%' OR name LIKE '%Theme%' OR name LIKE '%Company%' OR name LIKE '%Race%')")
tables = [r['name'] for r in cursor.fetchall()]
print("Matching Tables:", tables)

for t in tables:
    print(f"\nSchema of '{t}':")
    cursor.execute(f"PRAGMA table_info({t})")
    cols = [r['name'] for r in cursor.fetchall()]
    print("  Columns:", cols)
    cursor.execute(f"SELECT COUNT(*) as cnt FROM {t}")
    cnt = cursor.fetchone()['cnt']
    print(f"  Row Count: {cnt}")

print("\n--- Inspecting DIM_NameTheme / FCT_NameTheme ---")
try:
    cursor.execute("SELECT * FROM DIM_NameTheme LIMIT 20")
    for r in cursor.fetchall():
        print("  -", dict(r))
except Exception as e:
    print("  DIM_NameTheme error:", e)

try:
    cursor.execute("SELECT * FROM FCT_RaceNameTheme WHERE RaceID = 784")
    for r in cursor.fetchall():
        print("  - FCT_RaceNameTheme:", dict(r))
except Exception as e:
    print("  FCT_RaceNameTheme error:", e)

conn.close()
