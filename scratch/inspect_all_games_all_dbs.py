import os
import sqlite3

paths = [
    r"C:\Users\Fran\Desktop\Aurora271Full\AuroraDB.db",
    r"c:\VSCODE\Aurora271Full\AuroraDB.db",
    r"c:\VSCODE\aurora4x_manual\AuroraDB.db"
]

for p in paths:
    if os.path.exists(p):
        print(f"\n================ DB: {p} ================")
        conn = sqlite3.connect(p)
        cursor = conn.cursor()
        cursor.execute("SELECT GameID, GameName, GameTime FROM FCT_Game")
        for g in cursor.fetchall():
            gtime = g[2]
            days = gtime / 86400.0
            year = 2026 + (days / 365.25)
            print(f"   GameID: {g[0]} | Name: '{g[1]}' | GameTime: {g[2]} sec (~Year {year:.2f} / ~{days} days)")
        
        cursor.execute("SELECT RaceID, RaceTitle, GameID FROM FCT_Race")
        for r in cursor.fetchall():
            print(f"   RaceID: {r[0]} | RaceTitle: '{r[1]}' | GameID: {r[2]}")
        
        conn.close()
