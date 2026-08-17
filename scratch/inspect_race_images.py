import sqlite3
import os

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

print("--- FCT_Race Columns ---")
cursor.execute("PRAGMA table_info(FCT_Race)")
race_cols = cursor.fetchall()
for col in race_cols:
    print(f"  {col['name']} ({col['type']})")

print("\n--- FCT_Race Sample Data ---")
cursor.execute("SELECT * FROM FCT_Race LIMIT 2")
races = cursor.fetchall()
for r in races:
    d = dict(r)
    print({k: v for k, v in d.items() if any(img_kw in k.lower() for img_kw in ['flag', 'picture', 'portrait', 'icon', 'logo', 'file', 'image', 'ship', 'theme', 'race'])})

print("\n--- FCT_Species Columns ---")
cursor.execute("PRAGMA table_info(FCT_Species)")
spec_cols = cursor.fetchall()
for col in spec_cols:
    print(f"  {col['name']} ({col['type']})")

print("\n--- FCT_Species Sample Data ---")
cursor.execute("SELECT * FROM FCT_Species LIMIT 2")
specs = cursor.fetchall()
for s in specs:
    print(dict(s))

conn.close()
