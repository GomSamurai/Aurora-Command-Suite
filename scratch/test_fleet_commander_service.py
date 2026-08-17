import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

def get_fleet_commander(race_id, fleet_id):
    # 1. Fleet commander
    cursor.execute("""
        SELECT c.CommanderID, c.Name, c.Title, r.RankName, r.RankAbbrev
        FROM FCT_Commander c
        LEFT JOIN FCT_Ranks r ON c.RankID = r.RankID
        WHERE c.RaceID = ? AND c.CommandType = 2 AND c.CommandID = ?
        LIMIT 1""", (race_id, fleet_id))
    row = cursor.fetchone()

    # 2. Ship commander on ships in this fleet
    if not row:
        cursor.execute("""
            SELECT c.CommanderID, c.Name, c.Title, r.RankName, r.RankAbbrev, s.ShipName
            FROM FCT_Commander c
            LEFT JOIN FCT_Ranks r ON c.RankID = r.RankID
            JOIN FCT_Ship s ON c.CommandID = s.ShipID
            WHERE c.RaceID = ? AND c.CommandType = 1 AND s.FleetID = ?
            ORDER BY r.Priority ASC, c.Seniority DESC
            LIMIT 1""", (race_id, fleet_id))
        row = cursor.fetchone()

    # 3. Fallback active officer
    if not row:
        cursor.execute("""
            SELECT c.CommanderID, c.Name, c.Title, r.RankName, r.RankAbbrev
            FROM FCT_Commander c
            LEFT JOIN FCT_Ranks r ON c.RankID = r.RankID
            WHERE c.RaceID = ? AND (c.CommanderType = 1 OR c.CommanderType = 2)
            ORDER BY r.Priority ASC, c.Seniority DESC
            LIMIT 1""", (race_id,))
        row = cursor.fetchone()

    if not row:
        return "Sin Comandante Asignado", ["+0% Sin Bonificación"]

    cid = row['CommanderID']
    rank = row['RankName'] or "Oficial"
    name = row['Name']
    full_name = f"{rank} {name}"

    cursor.execute("""
        SELECT cb.BonusValue, bt.Description, bt.BonusAbbrev
        FROM FCT_CommanderBonuses cb
        JOIN DIM_CommanderBonusType bt ON cb.BonusID = bt.BonusID
        WHERE cb.CommanderID = ?
        ORDER BY cb.BonusValue DESC""", (cid,))
    b_rows = cursor.fetchall()
    bonuses = []
    for b in b_rows:
        val = (b['BonusValue'] - 1.0) * 100.0
        bonuses.append(f"+{val:.1f}% {b['Description']} ({b['BonusAbbrev']})")

    return full_name, bonuses

print("--- Testing Fleet Commanders for Race 784 ---")
cursor.execute("SELECT FleetID, FleetName FROM FCT_Fleet WHERE RaceID = 784")
fleets = cursor.fetchall()
for fl in fleets:
    fn, b_list = get_fleet_commander(784, fl['FleetID'])
    print(f"\nFleet: {fl['FleetName']} (ID {fl['FleetID']})")
    print(f"  Commander: {fn}")
    print(f"  Bonuses: {b_list}")

conn.close()
