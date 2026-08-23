import sqlite3

conn = sqlite3.connect(r'c:\VSCODE\Aurora271Full\AuroraDB.db')
cursor = conn.cursor()

print("--- Searching FCT_ComponentType ---")
cursor.execute("SELECT ComponentTypeID, Name FROM FCT_ComponentType")
types = cursor.fetchall()
for t in types:
    print(t)

print("\n--- Searching FCT_Component for Habitation/Crew/Quarter ---")
cursor.execute("SELECT ComponentID, Name, ComponentTypeID FROM FCT_Component WHERE Name LIKE '%Crew%' OR Name LIKE '%Quarter%' OR Name LIKE '%Habitation%' OR Name LIKE '%Alojamiento%'")
comps = cursor.fetchall()
for c in comps:
    print(c)
