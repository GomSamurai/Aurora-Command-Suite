import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

print("--- Testing Fetch of All Custom Projects for Race 784 ---")

projects = []

# 1. TechSystem
cursor.execute("""
    SELECT TechSystemID, Name, CategoryID, TechDescription, DevelopCost
    FROM FCT_TechSystem
    WHERE RaceID = 784 OR TechDescription LIKE '%Race-designed%'
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

# 2. ShipComponentTemplate
cursor.execute("""
    SELECT ShipComponentTemplateID, ComponentName, ComponentValue, ComponentSize, EnginePowerMod, ComponentTypeID
    FROM FCT_ShipComponentTemplate
    ORDER BY ShipComponentTemplateID DESC""")
sct_rows = cursor.fetchall()
for r in sct_rows:
    projects.append({
        'id': r['ShipComponentTemplateID'],
        'name': r['ComponentName'],
        'category': 'Componente Naval / Sensor',
        'source': '🎮 Aurora 4X (Juego)',
        'cost_rp': r['ComponentValue'],
        'spec': f"Tamaño: {r['ComponentSize']} HS ({r['ComponentSize']*50} t)"
    })

# 3. MissileType
cursor.execute("""
    SELECT MissileID, Name, Size, Speed, WarheadStrength, Cost
    FROM FCT_MissileType
    ORDER BY MissileID DESC""")
mt_rows = cursor.fetchall()
for r in mt_rows:
    projects.append({
        'id': r['MissileID'],
        'name': r['Name'],
        'category': 'Misil / Torpedo',
        'source': '🎮 Aurora 4X (Juego)',
        'cost_rp': r['Cost'],
        'spec': f"MSP: {r['Size']} | Vel: {r['Speed']} km/s | Cabeza: {r['WarheadStrength']}"
    })

print(f"Total Combined Custom Projects: {len(projects)}")
for p in projects[:15]:
    print(" ", p)

conn.close()
