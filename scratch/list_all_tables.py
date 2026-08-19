import sqlite3

db_path = r"C:\Users\Fran\Desktop\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
cursor = conn.cursor()

cursor.execute("SELECT name FROM sqlite_master WHERE type='table' ORDER BY name")
tables = [t[0] for t in cursor.fetchall()]

print("ALL TABLES IN AURORADB:")
for t in tables:
    if "Pop" in t or "Fuel" in t or "Res" in t or "Stock" in t or "Mat" in t or "Item" in t:
        print("  ", t)

cursor.execute("PRAGMA table_info(FCT_Population)")
print("\nFCT_Population cols:", cursor.fetchall())

cursor.execute("SELECT PopID, Population, Fuel, MSP FROM FCT_Population WHERE PopID = 48977")
print("\nEarth Pop & Fuel:", cursor.fetchall())

conn.close()
