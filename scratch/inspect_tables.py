import sqlite3

conn = sqlite3.connect(r'c:\VSCODE\Aurora271Full\AuroraDB.db')
cursor = conn.cursor()
cursor.execute("SELECT name FROM sqlite_master WHERE type='table'")
tables = [r[0] for r in cursor.fetchall()]
print("Tables containing Component or System or Tech:")
for t in sorted(tables):
    if 'Comp' in t or 'Tech' in t or 'Ship' in t or 'Race' in t:
        print(t)
