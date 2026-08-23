import sqlite3

conn = sqlite3.connect(r'c:\VSCODE\Aurora271Full\AuroraDB.db')
cursor = conn.cursor()

print("--- DIM_ComponentType ---")
cursor.execute("SELECT ComponentTypeID, ComponentTypeID, Description FROM DIM_ComponentType")
for r in cursor.fetchall():
    print(r)

print("\n--- FCT_TechSystem for Crew/Habitation ---")
cursor.execute("SELECT TechSystemID, Name, ComponentTypeID FROM FCT_TechSystem WHERE Name LIKE '%Crew%' OR Name LIKE '%Quarter%' OR Name LIKE '%Habitation%' OR Name LIKE '%Alojamiento%' LIMIT 20")
for r in cursor.fetchall():
    print(r)
