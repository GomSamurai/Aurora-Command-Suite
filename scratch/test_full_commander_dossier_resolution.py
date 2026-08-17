import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
conn = sqlite3.connect(db_path)
conn.row_factory = sqlite3.Row
cursor = conn.cursor()

TRAIT_TRANSLATIONS = {
    'Follows orders without question': 'Obediencia Ciega',
    'Ambitious': 'Ambicioso',
    'Doesn\'t accept change easily': 'Conservador',
    'Callous': 'Insensible',
    'Cheerful': 'Alegre',
    'Gloomy': 'Melancólico',
    'Inconsiderate': 'Poco Considerado',
    'Combative': 'Combativo',
    'Aggressive': 'Agresivo',
    'Cautious': 'Cauteloso',
    'Strange Medical Condition': '🏥 Condición Médica Extraña',
    'Impoverished': 'Origen Humilde',
    'Self-confident': 'Autoconfiante',
    'Authoritarian': 'Autoritario',
    'Patient': 'Paciente',
    'Astronomy Geek': '🔭 Apasionado de la Astronomía',
    'Philosophy Buff': '📜 Aficionado a la Filosofía',
    'Professional': 'Profesional',
    'Results-oriented': 'Orientado a Resultados',
    'Survivalist': 'Superviviente',
    'Observant': 'Observador',
    'Jealous': 'Receloso',
    'Intolerant': 'Intolerante',
    'Neurotic': '⚠️ Neurótico / Inestable',
    'Dispassionate': 'Imparcial',
    'Insightful': 'Perspicaz',
    'Science Fiction Buff': '🛸 Fan de Ciencia Ficción',
    'Wealthy': 'Cuna Acaudalada',
    'Analytical': 'Analítico',
    'Imaginative': 'Imaginativo',
    'Modest': 'Modesto'
}

def resolve_fleet_commander(race_id, fleet_id, ship_count):
    # 1. Check Fleet Commander
    cursor.execute("""
        SELECT c.CommanderID, c.Name, c.Title, c.Seniority, c.Loyalty, c.HealthRisk,
               c.KillTonnageMilitary, c.KillTonnageCommercial, r.RankName, r.RankAbbrev
        FROM FCT_Commander c
        LEFT JOIN FCT_Ranks r ON c.RankID = r.RankID
        WHERE c.RaceID = ? AND c.CommandType = 2 AND c.CommandID = ?
        LIMIT 1""", (race_id, fleet_id))
    row = cursor.fetchone()

    # 2. Check Ship Commander if ships exist
    if not row and ship_count > 0:
        cursor.execute("""
            SELECT c.CommanderID, c.Name, c.Title, c.Seniority, c.Loyalty, c.HealthRisk,
                   c.KillTonnageMilitary, c.KillTonnageCommercial, r.RankName, r.RankAbbrev
            FROM FCT_Commander c
            LEFT JOIN FCT_Ranks r ON c.RankID = r.RankID
            JOIN FCT_Ship s ON c.CommandID = s.ShipID
            WHERE c.RaceID = ? AND c.CommandType = 1 AND s.FleetID = ?
            ORDER BY r.Priority ASC, c.Seniority DESC
            LIMIT 1""", (race_id, fleet_id))
        row = cursor.fetchone()

    # 3. Fallback only if ships exist
    if not row and ship_count > 0:
        cursor.execute("""
            SELECT c.CommanderID, c.Name, c.Title, c.Seniority, c.Loyalty, c.HealthRisk,
                   c.KillTonnageMilitary, c.KillTonnageCommercial, r.RankName, r.RankAbbrev
            FROM FCT_Commander c
            LEFT JOIN FCT_Ranks r ON c.RankID = r.RankID
            WHERE c.RaceID = ? AND (c.CommanderType = 1 OR c.CommanderType = 2)
            ORDER BY r.Priority ASC, c.Seniority DESC
            LIMIT 1""", (race_id,))
        row = cursor.fetchone()

    if not row:
        return {
            'has_commander': False,
            'name': '⚠️ Sin Comandante (Flota Inactiva / Sin Naves)',
            'rank': '',
            'traits': 'Ninguno (Agrupación vacía)',
            'health': 'N/A',
            'bonuses': ['0% (Sin Naves Asignadas)']
        }

    cid = row['CommanderID']
    rank = row['RankName'] or "Oficial"
    name = row['Name']
    full_name = f"{rank} {name}"

    # Traits
    cursor.execute("""
        SELECT t.Name as TraitName
        FROM FCT_CommanderTraits ct
        JOIN DIM_TraitsList t ON ct.TraitID = t.TraitID
        WHERE ct.CmdrID = ?""", (cid,))
    t_rows = cursor.fetchall()
    traits = [TRAIT_TRANSLATIONS.get(t['TraitName'], t['TraitName']) for t in t_rows]
    traits_str = ", ".join(traits) if traits else "Sin Rasgos Destacados"

    # Bonuses
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

    health_risk = row['HealthRisk']
    health_str = "Saludable (Riesgo Bajo)" if health_risk == 0 else ("Salud Normal" if health_risk == 1 else "⚠️ Salud Delicada / Observación")

    return {
        'has_commander': True,
        'name': full_name,
        'rank': rank,
        'seniority': row['Seniority'],
        'loyalty': f"{row['Loyalty']}%",
        'health': health_str,
        'kills': f"Militar: {row['KillTonnageMilitary']}t | Comercial: {row['KillTonnageCommercial']}t",
        'traits': traits_str,
        'bonuses': bonuses
    }

print("--- Testing Enriched Dossier Resolution ---")
cursor.execute("SELECT FleetID, FleetName FROM FCT_Fleet WHERE RaceID = 784")
fleets = cursor.fetchall()
for fl in fleets:
    cursor.execute("SELECT COUNT(*) as Cnt FROM FCT_Ship WHERE FleetID = ?", (fl['FleetID'],))
    sc = cursor.fetchone()['Cnt']
    dossier = resolve_fleet_commander(784, fl['FleetID'], sc)
    print(f"\nFlota: {fl['FleetName']} ({sc} Naves)")
    print(f"  Comandante: {dossier['name']}")
    if dossier['has_commander']:
        print(f"  Salud: {dossier['health']} | Lealtad: {dossier['loyalty']} | Antigüedad: {dossier['seniority']}")
        print(f"  Rasgos / Personalidad: {dossier['traits']}")
        print(f"  Victorias / Kills: {dossier['kills']}")
        print(f"  Bonificaciones ({len(dossier['bonuses'])}): {dossier['bonuses']}")

conn.close()
