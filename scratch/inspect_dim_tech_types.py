import sqlite3

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
cursor = conn.cursor()

print("--- DIM_TechType ---")
try:
    for row in cursor.execute("SELECT * FROM DIM_TechType"):
        print(row)
except Exception as e:
    print(e)

print("\n--- Check how TechTypeID or CategoryID maps to Field ---")
try:
    for row in cursor.execute("SELECT t.TechSystemID, t.Name, t.CategoryID, t.TechTypeID, tt.Name, tt.FieldID FROM FCT_TechSystem t LEFT JOIN DIM_TechType tt ON t.TechTypeID = tt.TechTypeID LIMIT 30"):
        print(row)
except Exception as e:
    print(e)

print("\n--- Check distinct FieldIDs in DIM_TechType ---")
try:
    for row in cursor.execute("SELECT FieldID, COUNT(*) FROM DIM_TechType GROUP BY FieldID"):
        print(row)
except Exception as e:
    print(e)

conn.close()
