import json
import sqlite3
import os

db_path = 'c:/VSCODE/Aurora271Full/AuroraDB.db'
existing_json_path = 'c:/VSCODE/AuroraDesignSuite/config/AuroraTooltipDictionary.json'

existing_dict = {}
if os.path.exists(existing_json_path):
    try:
        existing_dict = json.load(open(existing_json_path, 'r', encoding='utf-8'))
    except Exception as e:
        print("Could not load existing dictionary:", e)

# -----------------------------------------------------------------------------
# 1. CORE UI TERMS & STRATEGIC CONCEPTS ENRICHMENT
# -----------------------------------------------------------------------------
core_concepts = {
    "Population": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Población Colonial representa el total de habitantes civiles asentados en la superficie del planeta o luna (medido en millones, ej. 1,412.18m = 1,412.18 millones de habitantes).\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Fuente de Mano de Obra: La población proporciona trabajadores indispensables para operar Fábricas de Construcción, Minas, Astilleros, Refinerías y Laboratorios.\n"
        "• Ingresos por Impuestos: Los ciudadanos generan Riqueza (Wealth) continua a través de impuestos anuales.\n"
        "• Reclutamiento Militar: La población es la cantera para formar tropas terrestres y tripulantes navales.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Si despliegas 100 Fábricas de Construcción pero solo tienes 2 millones de habitantes, no habrá suficientes trabajadores para operar la industria y la producción caerá en picado.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Fomenta el crecimiento poblacional manteniendo el Costo Colonial en 0.00 y utilizando Barcos Colonizadores para trasladar colonos desde la Tierra hacia mundos aptos."
    ),

    "Planetary Suitability (Colony Cost)": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Índice de habitabilidad o Costo Colonial (Colony Cost) mide el nivel de hostilidad ambiental de un cuerpo celeste respecto a la biología de tu especie.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Colony Cost 0.00: El mundo es ideal y totalmente habitable (ej. la Tierra). La población crece libremente sin necesitar domos de Infraestructura.\n"
        "• Colony Cost > 0.00: Requiere Infraestructura Poblacional para mantener viva a la población civil. Cada millón de habitantes requerirá N unidades de Infraestructura proporcional al costo colonial.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "En Marte con Colony Cost 2.00, cada millón de habitantes necesita 200 unidades de Infraestructura para no sofocarse ni perecer.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Utiliza Barcos Terraformadores o Instalaciones de Terraformación para inyectar gases atmosféricos y reducir el Costo Colonial a 0.00, liberando tu industria del gasto en Infraestructura."
    ),

    "Duranium": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Duranium es el mineral trans-newtoniano estructural primario. Es el metal sintético más utilizado en todo el universo de Aurora 4X.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Construcción de Edificios: Se requiere en la fabricación de CADA instalación industrial, laboratorio y refinería.\n"
        "• Cascos Navales & Blindaje: Forma la osamenta y las placas protectoras de todas las naves espaciales y cazas.\n"
        "• Misiles y Munición: Se consume en la producción de misiles y torpedos.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Si tu reserva de Duranium llega a 0, TODA la producción industrial del imperio se congelará instantáneamente.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Mantén siempre la mayor tasa de extracción posible de Duranium y asegura reservas estratégicas en la Tierra."
    ),

    "Sorium": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Sorium es el mineral líquido/cristalino más codiciado en la galaxia, utilizado exclusivamente para el refinado de combustible espacial LPH.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Las Refinerías de Combustible convierten el Sorium extraído en millones de litros de combustible hidrocarburo para tus naves.\n"
        "• También se utiliza en cabezas de guerra de energía y reactores.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un gigante gaseoso con 100,000,000 de toneladas de Sorium es una mina de oro estratégica. Puedes desplegar Harvesters (Mineros de Sorium Orbitales) para extraerlo sin aterrizar en la superficie.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Establece estaciones de refinado y depósitos de combustible en los Puntos de Salto clave del imperio."
    ),

    "Retooling (Re-equipamiento)": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Proceso industrial mediante el cual un Astillero (Shipyard) reconfigura sus herramientas y gradas para fabricar una nueva clase de nave espacial.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Un astillero solo puede construir la clase para la que está equipado actualmente.\n"
        "• Si la nueva clase es un derivado o variante de la anterior (ej. 'Fragata Mk2' basada en 'Fragata Mk1'), el costo y tiempo de retooling disminuye drásticamente.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Reequipar para un diseño nuevo desde cero puede demorar 12 meses; mientras que reequipar para una variante del mismo casco solo tomará 2 meses.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Diseña tus naves en familias o generaciones modulares para aprovechar la bonificación de re-equipamiento rápido."
    ),

    "HS - Hull Size (Tamaño de Casco)": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Unidad de medida estándar de desplazamiento naval en Aurora 4X. 1 HS equivale exactamente a 50 toneladas métricas.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Cazas y Pequeñas Naves: < 10 HS (500 toneladas).\n"
        "• Corbetas y Fragatas: 20 - 100 HS (1,000 - 5,000 toneladas).\n"
        "• Destructores y Cruceros: 100 - 400 HS (5,000 - 20,000 toneladas).\n"
        "• Acorazados y Cargueros Pesados: > 500 HS (25,000+ toneladas).\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Una nave de 50 HS pesa exactamente 2,500 toneladas. Requiere un astillero militar con capacidad asignada de al menos 2,500 toneladas.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Expande la capacidad de tus astilleros en bloques continuos antes de diseñar naves de mayor tonelaje."
    ),

    "TCS - Thermal & Cross Section (Firma Térmica)": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Índice que mide la visibilidad de tu nave ante los sensores térmicos pasivos y radares activos del enemigo.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• A mayor tamaño (HS) y mayor potencia de motor, mayor será la firma TCS.\n"
        "• Los sensores enemigos detectan tus naves a distancias proporcionales a su valor TCS.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un carguero pesado comercial tiene una firma TCS gigante y es detectado desde el otro extremo del sistema. Una nave sigilosa con motores reducidos tiene una firma TCS ínfima.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Equipa componentes de sigilo (Stealth Coatings) y reduce el multiplicador de potencia de los motores en naves de exploración o emboscada."
    ),

    "DCR - Damage Control Rating (Control de Daños)": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Capacidad operativa de las brigadas de control de averías a bordo de una nave espacial militar.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Un valor DCR elevado permite reparar múltiples sistemas destruidos o dañados durante el combate consumiendo repuestos MSP.\n"
        "• Aumenta la supervivencia de la nave frente a impactos de misiles o disparos de energía.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Si un cañón láser destruye el motor principal de tu nave, las brigadas con alto DCR repararán el motor en pleno combate si llevan pañoles MSP a bordo.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Añade siempre pañoles 'Engineering Spaces' y mantén un DCR equilibrado en todos tus buques de guerra."
    )
}

# Merge enriched core concepts into existing dict
for k, v in core_concepts.items():
    existing_dict[k] = v

# -----------------------------------------------------------------------------
# 2. ENRICH ALL DB TECHNOLOGIES (FCT_TechSystem)
# -----------------------------------------------------------------------------
if os.path.exists(db_path):
    conn = sqlite3.connect(db_path)
    cursor = conn.cursor()
    cursor.execute("""
        SELECT t.Name, c.Name, tt.Description, t.DevelopCost
        FROM FCT_TechSystem t
        LEFT JOIN DIM_ResearchCategories c ON t.CategoryID = c.CategoryID
        LEFT JOIN DIM_TechType tt ON t.TechTypeID = tt.TechTypeID
    """)
    rows = cursor.fetchall()

    for name, cat_name, type_desc, cost in rows:
        if not name:
            continue
        
        name = name.strip()
        cat_name = cat_name.strip() if cat_name else "Tecnología General"
        type_desc = type_desc.strip() if type_desc else cat_name
        cost_val = int(cost) if cost else 0

        # Expand description if missing or standard
        body = (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{name} es un avance científico en la especialidad de {cat_name} ({type_desc}).\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Costo de Investigación: {cost_val:,} Puntos de Investigación (RP).\n"
            f"• Rama Científica: {cat_name}.\n"
            f"• Tipo de Tecnología: {type_desc}.\n"
            f"• Desbloquea componentes navales, armas, sensores o eficiencias industriales de nueva generación.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Al investigar este proyecto en la pantalla de I+D (ResearchHQ), el componente o mejora estará disponible en el Diseñador de Naves (Class Design) o en la gestión de colonias.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Asigna esta investigación a un científico con especialidad en '{cat_name}' para obtener una bonificación directa de velocidad de desarrollo de hasta el 50%."
        )

        existing_dict[name] = body

    conn.close()

# Save enriched dictionary
with open(existing_json_path, 'w', encoding='utf-8') as f:
    json.dump(existing_dict, f, ensure_ascii=False, indent=2)

print(f"Master dictionary enriched successfully with {len(existing_dict)} entries!")

# Also copy to target app config paths
portable_json_path = 'c:/VSCODE/Aurora_Command_Suite_v2.7.1_Portable/App/config/AuroraTooltipDictionary.json'
game_patch_json_path = 'c:/VSCODE/Aurora271Full/Patches/AuroraSpanish/AuroraTooltipDictionary.json'

for target_path in [portable_json_path, game_patch_json_path]:
    try:
        os.makedirs(os.path.dirname(target_path), exist_ok=True)
        with open(target_path, 'w', encoding='utf-8') as f:
            json.dump(existing_dict, f, ensure_ascii=False, indent=2)
        print(f"Copied to {target_path}")
    except Exception as e:
        print(f"Error copying to {target_path}: {e}")
