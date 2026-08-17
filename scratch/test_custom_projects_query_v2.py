import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

print("--- Testing Fetch of Custom Projects for Race 784 ---")

projects = []

# 1. TechSystem
cursor.execute("""
    SELECT TechSystemID, Name, CategoryID, TechDescription, DevelopCost, TechTypeID
    FROM FCT_TechSystem
    WHERE RaceID = 784 OR TechDescription LIKE '%Race-designed%' OR TechDescription LIKE '%Custom%' OR CategoryID > 0
    ORDER BY TechSystemID DESC""")
ts_rows = cursor.fetchall()
for r in ts_rows:
    projects.append({
        'id': r['TechSystemID'],
        'name': r['Name'],
        'category': f"I+D (Cat #{r['CategoryID']})",
        'source': '🎮 Aurora 4X (Juego)',
        'cost_rp': r['DevelopCost'],
        'spec': r['TechDescription'] or "Proyecto Personalizado"
    })

print(f"Total Projects Found: {len(projects)}")
ybarra = [p for p in projects if 'Ybarra' in p['name']]
print(f"Ybarra Engine Found: {ybarra}")

conn.close()
