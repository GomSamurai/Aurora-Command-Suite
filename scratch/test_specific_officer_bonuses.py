import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

def print_officer(cid, name):
    print(f"\n--- Commander: {name} (ID: {cid}) ---")
    cursor.execute("""
        SELECT cb.BonusValue, bt.Description, bt.BonusAbbrev
        FROM FCT_CommanderBonuses cb
        JOIN DIM_CommanderBonusType bt ON cb.BonusID = bt.BonusID
        WHERE cb.CommanderID = ?""", (cid,))
    bonuses = cursor.fetchall()
    for b in bonuses:
        val = (b['BonusValue'] - 1.0) * 100.0
        print(f"  - {b['Description']} ({b['BonusAbbrev']}): +{val:.1f}%")

print_officer(620856, "Liana Villarreal Estévez")
print_officer(620782, "Adoración Duarte Domínguez")

conn.close()
