import sqlite3

conn = sqlite3.connect(r'c:\VSCODE\Aurora271Full\AuroraDB.db')
cursor = conn.cursor()

cursor.execute("PRAGMA table_info(FCT_ShipComponentTemplate)")
print("FCT_ShipComponentTemplate columns:", [r[1] for r in cursor.fetchall()])

cursor.execute("SELECT ShipComponentTemplateID, ComponentName, ComponentTypeID, ComponentSize, ComponentValue, Crew FROM FCT_ShipComponentTemplate LIMIT 10")
print("\nSample rows:", cursor.fetchall())
