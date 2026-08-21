import json
import os

dict_path = r"c:\VSCODE\AuroraDesignSuite\config\AuroraTooltipDictionary.json"

fleet_terms = {
    "Battle Fleet": "⚓ Flota de Asalto y Combate Naval: Escuadra militar compuesta por buques de línea, fragatas y cazas configurada para misiones de superioridad espacial, intercepción de misiles y defensa del sistema.",
    "Cargo Fleet": "📦 Flota Logística de Carga: Convoy comercial de buques cargueros especializado en el transporte masivo de fábricas, minerales e instalaciones industriales entre colonias.",
    "Colony Fleet": "🪐 Flota Colonizadora: Escuadra de transportes de colonos e infraestructura urbana dedicada a la colonización y expansión poblacional en nuevos mundos.",
    "Shipyard Fleet": "🏗️ Flota de Astilleros y Construcción Orbital: Unidades de apoyo logístico y mantenimiento para la expansión de gradas navales y reparación de buques en espacio profundo.",
    "Survey Fleet": "🛸 Flota de Prospección y Reconocimiento: Naves exploradoras equipadas con sensores geológicos y gravitatorios para cartografiar Puntos de Salto y descubrir yacimientos minerales.",
    "Fleet": "⚓ Grupo Táctico Naval: Formación de buques militares o comerciales coordinados bajo la autoridad de un Comandante de Flota.",
    "Task Force": "⚔️ Fuerza Táctica Naval: Agrupación operacional de combate configurada para misiones estratégicas específicas en sectores de frontera.",
    "Active Fleet": "⚓ Flota Activa en Servicio: Escuadra desplegada en espacio profundo o en órbita con consumo continuo de mantenimiento y combustible.",
    "Active Ship": "🚀 Buque Espacial Activo: Unidad naval dotada de casco, motores, blindaje y componentes operativos.",
    "Commanders": "🎖️ Comandantes e Imperiales: Oficiales de alta graduación que aportan bonificaciones críticas a flotas, colonias y laboratorios de I+D.",
    "Star System": "🌌 Sistema Estelar: Sector compuesto por una o más estrellas, planetas, lunas, asteroides y puntos de salto interestelares.",
    "System Body": "🪐 Cuerpo Celeste: Planeta, luna o asteroide susceptible de albergar minerales exóticos, ruinas o colonias.",
    "Jump Point": "🌀 Punto de Salto Interestelar: Anomalía gravitacional que interconecta dos sistemas estelares transitables mediante motores o puertas warp."
}

if os.path.exists(dict_path):
    with open(dict_path, "r", encoding="utf-8") as f:
        data = json.load(f)
    
    for k, v in fleet_terms.items():
        data[k] = v
        
    with open(dict_path, "w", encoding="utf-8") as f:
        json.dump(data, f, ensure_ascii=False, indent=2)
    print(f"Enriched dictionary with {len(fleet_terms)} fleet terms. Total entries: {len(data)}")

# Also copy to portable folder if exists
portable_path = r"c:\VSCODE\Aurora_Command_Suite_v2.7.1_Portable\App\config\AuroraTooltipDictionary.json"
if os.path.exists(portable_path):
    with open(dict_path, "r", encoding="utf-8") as f:
        data = json.load(f)
    with open(portable_path, "w", encoding="utf-8") as f:
        json.dump(data, f, ensure_ascii=False, indent=2)
    print("Updated portable AuroraTooltipDictionary.json")
