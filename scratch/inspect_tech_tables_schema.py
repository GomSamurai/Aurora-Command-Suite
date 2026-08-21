import sqlite3

conn = sqlite3.connect(r'C:\VSCODE\Aurora271Full\AuroraDB.db')
c = conn.cursor()

c.execute("SELECT name FROM sqlite_master WHERE type='table' AND name LIKE '%Tech%'")
tables = [row[0] for row in c.fetchall()]
print("Tech tables:", tables)

for t in tables:
    c.execute(f"PRAGMA table_info({t})")
    cols = [row[1] for row in c.fetchall()]
    print(f"Table {t} columns:", cols)
