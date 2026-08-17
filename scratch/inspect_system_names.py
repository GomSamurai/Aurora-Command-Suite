import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

def inspect_sample(tbl, sql):
    print(f"\n--- {tbl} ---")
    try:
        cursor.execute(sql)
        rows = cursor.fetchall()
        for r in rows:
            print(" ", dict(r))
    except Exception as e:
        print("  ERROR:", e)

inspect_sample("FCT_SystemBodyName", "SELECT * FROM FCT_SystemBodyName LIMIT 5")
inspect_sample("DIM_KnownSystems", "SELECT * FROM DIM_KnownSystems LIMIT 5")

conn.close()
