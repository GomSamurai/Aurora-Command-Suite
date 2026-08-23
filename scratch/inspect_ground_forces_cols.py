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

print_cols("FCT_GroundUnitFormation")
print_cols("FCT_GroundUnitClass")
print_cols("FCT_GroundUnitFormationElement")
print_cols("FCT_GroundUnitFormationTemplate")
print_cols("DIM_GroundUnitBaseType")

conn.close()
