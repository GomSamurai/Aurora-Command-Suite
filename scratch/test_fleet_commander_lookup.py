import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

print("--- Check Race 784 Fleets & Ships ---")
cursor.execute("SELECT FleetID, FleetName FROM FCT_Fleet WHERE RaceID = 784")
fleets = cursor.fetchall()

for fl in fleets:
    f_id = fl['FleetID']
    f_name = fl['FleetName']
    print(f"\nFleet #{f_id}: {f_name}")

    # Check Fleet Commander (CommandType = 2, CommandID = f_id) OR Ship Commanders for ships in this fleet
    cursor.execute("""
        SELECT c.CommanderID, c.Name, c.Title, c.CommandType, c.CommandID, r.RankName
        FROM FCT_Commander c
        LEFT JOIN DIM_CommanderRank r ON c.RankID = r.RankID
        WHERE c.RaceID = 784 AND (
            (c.CommandType = 2 AND c.CommandID = ?) OR
            (c.CommandType = 1 AND c.CommandID IN (SELECT ShipID FROM FCT_Ship WHERE FleetID = ?))
        )""", (f_id, f_id))

    cmds = cursor.fetchall()
    if cmds:
        for cmd in cmds:
            print(f"  Commander found: {cmd['RankName'] or 'Commander'} {cmd['Name']} (CommandType={cmd['CommandType']}, CommandID={cmd['CommandID']})")
            
            # Fetch bonuses
            cursor.execute("""
                SELECT cb.BonusValue, bt.Description, bt.BonusAbbrev
                FROM FCT_CommanderBonuses cb
                JOIN DIM_CommanderBonusType bt ON cb.BonusID = bt.BonusID
                WHERE cb.CommanderID = ?""", (cmd['CommanderID'],))
            bonuses = cursor.fetchall()
            for b in bonuses:
                val = (b['BonusValue'] - 1.0) * 100.0
                print(f"    - {b['Description']} ({b['BonusAbbrev']}): +{val:.1f}%")
    else:
        print("  No direct commander assigned to fleet or ships. Looking up any active naval commander...")
        cursor.execute("""
            SELECT c.CommanderID, c.Name, r.RankName
            FROM FCT_Commander c
            LEFT JOIN DIM_CommanderRank r ON c.RankID = r.RankID
            WHERE c.RaceID = 784 AND c.CommanderType = 1
            LIMIT 1""")
        any_cmd = cursor.fetchone()
        if any_cmd:
            print(f"  Sample officer for race: {any_cmd['RankName']} {any_cmd['Name']}")

conn.close()
