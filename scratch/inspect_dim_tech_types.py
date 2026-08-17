import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

print("--- Inspecting DIM_TechType ---")
try:
    cursor.execute("SELECT TechTypeID, TypeName, CategoryID FROM DIM_TechType ORDER BY TechTypeID")
    for r in cursor.fetchall()[:40]:
        print(" ", dict(r))
except Exception as e:
    print(f"Error querying DIM_TechType: {e}")

print("\n--- Inspecting DIM_ComponentType ---")
try:
    cursor.execute("SELECT ComponentTypeID, TypeDescription FROM DIM_ComponentType ORDER BY ComponentTypeID")
    for r in cursor.fetchall()[:40]:
        print(" ", dict(r))
except Exception as e:
    print(f"Error querying DIM_ComponentType: {e}")

conn.close()
