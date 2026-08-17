import sqlite3

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

print("--- DIM_ResearchField ---")
try:
    for row in cursor.execute("SELECT * FROM DIM_ResearchField"):
        print(row)
except Exception as e:
    print(e)

print("\n--- FCT_TechSystem CategoryID breakdown ---")
try:
    for row in cursor.execute("SELECT CategoryID, COUNT(*) FROM FCT_TechSystem GROUP BY CategoryID"):
        print(row)
except Exception as e:
    print(e)

print("\n--- Check FCT_TechSystem columns ---")
try:
    cursor.execute("PRAGMA table_info(FCT_TechSystem)")
    for col in cursor.fetchall():
        print(col)
except Exception as e:
    print(e)

print("\n--- FCT_TechSystem sample rows ---")
try:
    for row in cursor.execute("SELECT TechSystemID, Name, CategoryID, DevelopCost FROM FCT_TechSystem LIMIT 20"):
        print(row)
except Exception as e:
    print(e)

print("\n--- Check categories for Construction & Logistics or CategoryID values ---")
try:
    for row in cursor.execute("SELECT DISTINCT CategoryID FROM FCT_TechSystem"):
        print(row)
except Exception as e:
    print(e)

conn.close()
