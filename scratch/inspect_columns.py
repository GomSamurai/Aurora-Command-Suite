import sqlite3

conn = sqlite3.connect(r'c:\VSCODE\Aurora271Full\AuroraDB.db')
cursor = conn.cursor()

cursor.execute("PRAGMA table_info(DIM_ComponentType)")
print("DIM_ComponentType columns:", cursor.fetchall())

cursor.execute("PRAGMA table_info(FCT_TechSystem)")
print("FCT_TechSystem columns:", [r[1] for r in cursor.fetchall()])

cursor.execute("SELECT * FROM DIM_ComponentType WHERE Name LIKE '%Crew%' OR Name LIKE '%Habit%' OR Name LIKE '%Aloj%' OR Name LIKE '%Quarter%' OR Name LIKE '%Living%' OR Name LIKE '%Berth%' OR Name LIKE '%Space%'")
print("\nMatching ComponentTypes:", cursor.fetchall())

cursor.execute("SELECT TechSystemID, Name, ComponentTypeID FROM FCT_TechSystem WHERE Name LIKE '%Crew%' OR Name LIKE '%Quarter%' OR Name LIKE '%Habit%' LIMIT 20")
print("\nMatching TechSystems:", cursor.fetchall())
