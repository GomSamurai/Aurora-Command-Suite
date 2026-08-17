import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

def inspect(tbl_name):
    print(f"\n--- {tbl_name} ---")
    try:
        cursor.execute(f"PRAGMA table_info({tbl_name})")
        cols = cursor.fetchall()
        for c in cols:
            print(f"  {c['name']} ({c['type']})")
    except Exception as e:
        print(f"  ERROR: {e}")

cursor.execute("SELECT name FROM sqlite_master WHERE type='table'")
tables = [t['name'] for t in cursor.fetchall()]
print("All tables in DB containing Tech, Project, Commander, System, Officer, Fleet:")
for t in sorted(tables):
    if any(k in t.lower() for k in ['tech', 'project', 'cmd', 'command', 'system', 'officer', 'fleet', 'ship']):
        print(" ", t)

inspect("FCT_IndustrialProjects")
inspect("FCT_TechSystem")
inspect("FCT_Commander")
inspect("FCT_System")
inspect("FCT_SystemBody")

conn.close()
