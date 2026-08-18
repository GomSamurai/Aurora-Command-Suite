import sqlite3

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

# Set WAL mode
cursor.execute("PRAGMA journal_mode=WAL;")
res = cursor.fetchone()
print(f"Set Journal Mode: {res[0]}")

cursor.execute("PRAGMA busy_timeout=5000;")
cursor.execute("PRAGMA synchronous=NORMAL;")
cursor.execute("PRAGMA wal_checkpoint(FULL);")
checkpoint_res = cursor.fetchone()
print(f"WAL Checkpoint Result: {checkpoint_res}")

conn.close()
