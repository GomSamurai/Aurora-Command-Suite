import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

print("--- FCT_Commander Table Schema ---")
cursor.execute("PRAGMA table_info(FCT_Commander)")
cols = cursor.fetchall()
for c in cols:
    print(f"  {c['name']} ({c['type']})")

print("\n--- Check FCT_CommanderTraits ---")
cursor.execute("PRAGMA table_info(FCT_CommanderTraits)")
for c in cursor.fetchall():
    print(f"  {c['name']} ({c['type']})")

print("\n--- Check DIM_TraitsList ---")
cursor.execute("PRAGMA table_info(DIM_TraitsList)")
for c in cursor.fetchall():
    print(f"  {c['name']} ({c['type']})")

cursor.execute("SELECT * FROM DIM_TraitsList LIMIT 10")
for r in cursor.fetchall():
    print(" ", dict(r))

print("\n--- Check FCT_CommanderTraits Data for Race 784 ---")
cursor.execute("""
    SELECT ct.*, t.Name as TraitName
    FROM FCT_CommanderTraits ct
    JOIN DIM_TraitsList t ON ct.TraitID = t.TraitID
    JOIN FCT_Commander c ON ct.CmdrID = c.CommanderID
    WHERE c.RaceID = 784 LIMIT 15""")
for r in cursor.fetchall():
    print(" ", dict(r))

print("\n--- Check Game Time to calculate Age/Service ---")
cursor.execute("SELECT GameTime FROM FCT_Game LIMIT 1")
gt = cursor.fetchone()['GameTime']
print(f"Current GameTime: {gt}")

print("\n--- Detailed Officer Sample (Liana Villarreal Estévez & Adoración Duarte) ---")
cursor.execute("""
    SELECT c.*, r.RankName, r.RankAbbrev
    FROM FCT_Commander c
    LEFT JOIN FCT_Ranks r ON c.RankID = r.RankID
    WHERE c.CommanderID IN (620856, 620782)""")
for o in cursor.fetchall():
    d = dict(o)
    print(f"\nOfficer #{d['CommanderID']}: {d['RankName']} {d['Name']}")
    service_years = (gt - d['CareerStart']) / (365.25 * 86400.0) if d['CareerStart'] > 0 else 0
    promoted_years = (gt - d['GameTimePromoted']) / (365.25 * 86400.0) if d['GameTimePromoted'] > 0 else 0
    print(f"  Service: {service_years:.1f} years, In Current Rank: {promoted_years:.1f} years, Seniority: {d['Seniority']}, Loyalty: {d['Loyalty']}, HealthRisk: {d['HealthRisk']}")
    print(f"  Military Kills: {d['KillTonnageMilitary']}t, Commercial Kills: {d['KillTonnageCommercial']}t")

    # Traits
    cursor.execute("""
        SELECT t.Name as TraitName
        FROM FCT_CommanderTraits ct
        JOIN DIM_TraitsList t ON ct.TraitID = t.TraitID
        WHERE ct.CmdrID = ?""", (d['CommanderID'],))
    traits = cursor.fetchall()
    print(f"  Traits/Personalidad: {[t['TraitName'] for t in traits]}")

    # Bonuses
    cursor.execute("""
        SELECT cb.BonusValue, bt.Description, bt.BonusAbbrev
        FROM FCT_CommanderBonuses cb
        JOIN DIM_CommanderBonusType bt ON cb.BonusID = bt.BonusID
        WHERE cb.CommanderID = ?""", (d['CommanderID'],))
    bonuses = cursor.fetchall()
    for b in bonuses:
        val = (b['BonusValue'] - 1.0) * 100.0
        print(f"  Bonus: {b['Description']} ({b['BonusAbbrev']}): +{val:.1f}%")

conn.close()
