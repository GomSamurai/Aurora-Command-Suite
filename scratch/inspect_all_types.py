import sqlite3

conn = sqlite3.connect(r'c:\VSCODE\Aurora271Full\AuroraDB.db')
cursor = conn.cursor()

cursor.execute("SELECT ComponentTypeID, TypeDescription FROM DIM_ComponentType")
print("--- ALL Component Types ---")
for r in cursor.fetchall():
    print(r)
