import sqlite3

conn = sqlite3.connect(r'c:\VSCODE\Aurora271Full\AuroraDB.db')
cursor = conn.cursor()

cursor.execute("SELECT ShipComponentTemplateID, ComponentName, ComponentTypeID, ComponentSize FROM FCT_ShipComponentTemplate WHERE ComponentTypeID = 2 OR ComponentTypeID = 31 OR ComponentTypeID = 47")
print("Crew Quarters & Engineering templates in AuroraDB.db:")
for r in cursor.fetchall():
    print(r)
