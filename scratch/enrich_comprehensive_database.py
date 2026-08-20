import json
import os
import sqlite3

existing_json_path = 'c:/VSCODE/AuroraDesignSuite/config/AuroraTooltipDictionary.json'
db_path = 'c:/VSCODE/Aurora271Full/AuroraDB.db'

existing_dict = {}
if os.path.exists(existing_json_path):
    try:
        existing_dict = json.load(open(existing_json_path, 'r', encoding='utf-8'))
    except Exception as e:
        print("Could not load existing dictionary:", e)

# -----------------------------------------------------------------------------
# 1. PLANETARY INSTALLATIONS (All 17 Facilities in Spanish & English)
# -----------------------------------------------------------------------------
installations = {
    "Academia Militar": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Academia Militar (Military Academy) es la institución de formación superior donde se educan y gradúan los oficiales de la armada, comandantes de flota, gobernadores planetarios y científicos del Imperio.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Generación de Oficiales: Cada Academia Militar graduará entre 1 y 3 nuevos oficiales por año en función de las políticas imperiales.\n"
        "• Promoción y Reclutamiento: Esencial para mantener cubiertos los puestos de mando en nuevas naves de guerra y colonias recien fundadas.\n"
        "• Entrenamiento Terrestre: Incrementa la capacidad de adiestramiento de las tropas del ejército colonial.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Si construyes 5 Academias Militares adicionales en la Tierra, pasarás de recibir 2 oficiales al año a recibir entre 8 y 12 oficiales anuales con bonificaciones de administración y combate.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Mantén al menos 6 Academias Militares operativas en tu planeta capital para no sufrir escasez de capitanes ni científicos de I+D."
    ),

    "Military Academy": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Academia Militar (Military Academy) es la institución de formación superior donde se educan y gradúan los oficiales de la armada, comandantes de flota, gobernadores planetarios y científicos del Imperio.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Generación de Oficiales: Cada Academia Militar graduará entre 1 y 3 nuevos oficiales por año en función de las políticas imperiales.\n"
        "• Promoción y Reclutamiento: Esencial para mantener cubiertos los puestos de mando en nuevas naves de guerra y colonias recien fundadas.\n"
        "• Entrenamiento Terrestre: Incrementa la capacidad de adiestramiento de las tropas del ejército colonial.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Si construyes 5 Academias Militares adicionales en la Tierra, pasarás de recibir 2 oficiales al año a recibir entre 8 y 12 oficiales anuales con bonificaciones de administración y combate.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Mantén al menos 6 Academias Militares operativas en tu planeta capital para no sufrir escasez de capitanes ni científicos de I+D."
    ),

    "Fábrica de Construcción": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Fábrica de Construcción (Construction Factory) es la columna vertebral de la industria pesada en Aurora 4X. Transforma minerales trans-newtonianos en nuevas instalaciones planetarias.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Capacidad de Producción: Genera Puntos de Construcción (BP) por turno para levantar Minas, Refinerías, Laboratorios y otras Fábricas.\n"
        "• Modificador de Infraestructura: Es la responsable de fabricar las unidades de Infraestructura necesarias para colonias hostiles.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Con 500 Fábricas de Construcción en la Tierra, podrás construir un nuevo Laboratorio de Investigación en apenas 3 meses.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Asigna siempre entre el 40% y el 60% de tu capacidad industrial a multiplicar tus Fábricas de Construcción durante la fase temprana del juego."
    ),

    "Construction Factory": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Fábrica de Construcción (Construction Factory) es la columna vertebral de la industria pesada en Aurora 4X. Transforma minerales trans-newtonianos en nuevas instalaciones planetarias.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Capacidad de Producción: Genera Puntos de Construcción (BP) por turno para levantar Minas, Refinerías, Laboratorios y otras Fábricas.\n"
        "• Modificador de Infraestructura: Es la responsable de fabricar las unidades de Infraestructura necesarias para colonias hostiles.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Con 500 Fábricas de Construcción en la Tierra, podrás construir un nuevo Laboratorio de Investigación en apenas 3 meses.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Asigna siempre entre el 40% y el 60% de tu capacidad industrial a multiplicar tus Fábricas de Construcción durante la fase temprana del juego."
    ),

    "Refinería de Combustible": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Refinería de Combustible (Fuel Refinery) es la instalación industrial encargada de procesar el mineral de Sorium y convertirlo en combustible hidrocarburo espacial LPH.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Tasa de Refinado: Cada refinería procesa Sorium en bruto generando miles de litros de combustible por año (ej. 200,000 L/año por refinería estándar).\n"
        "• Abastecimiento de Flota: Mantiene llenos los depósitos de tus cargueros, cazas, destructores y estaciones orbitales.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Si tu flota naval consume 5,000,000 de litros al año en maniobras de patrulla, necesitarás al menos 25 Refinerías de Combustible activas para no agotar las reservas.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Ubica refinerías directamente en asteroides o lunas ricos en Sorium para eliminar el costo de transporte de mineral en bruto."
    ),

    "Fuel Refinery": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Refinería de Combustible (Fuel Refinery) es la instalación industrial encargada de procesar el mineral de Sorium y convertirlo en combustible hidrocarburo espacial LPH.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Tasa de Refinado: Cada refinería procesa Sorium en bruto generando miles de litros de combustible por año (ej. 200,000 L/año por refinería estándar).\n"
        "• Abastecimiento de Flota: Mantiene llenos los depósitos de tus cargueros, cazas, destructores y estaciones orbitales.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Si tu flota naval consume 5,000,000 de litros al año en maniobras de patrulla, necesitarás al menos 25 Refinerías de Combustible activas para no agotar las reservas.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Ubica refinerías directamente en asteroides o lunas ricos en Sorium para eliminar el costo de transporte de mineral en bruto."
    ),

    "Centro Financiero": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Centro Financiero (Financial Centre) es el motor bancario y comercial de la colonia. Potencia la economía bancaria y el comercio internacional de la especie.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Generación de Riqueza (Wealth): Cada centro financiero inyecta Riqueza fiscal directa en el tesoro colonial del Imperio.\n"
        "• Mantenimiento del Estado: Paga los salarios de oficiales, mantenimiento de instalaciones y subvenciones de la flota civil de freighters.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Si sufres déficit presupuestario anual, construir 50 Centros Financieros reequilibrará tus arcas imperiales generando un superávit de Riqueza.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Construye centros financieros en mundos maduros con alta densidad poblacional para maximizar el multiplicador fiscal."
    ),

    "Financial Centre": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Centro Financiero (Financial Centre) es el motor bancario y comercial de la colonia. Potencia la economía bancaria y el comercio internacional de la especie.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Generación de Riqueza (Wealth): Cada centro financiero inyecta Riqueza fiscal directa en el tesoro colonial del Imperio.\n"
        "• Mantenimiento del Estado: Paga los salarios de oficiales, mantenimiento de instalaciones y subvenciones de la flota civil de freighters.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Si sufres déficit presupuestario anual, construir 50 Centros Financieros reequilibrará tus arcas imperiales generando un superávit de Riqueza.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Construye centros financieros en mundos maduros con alta densidad poblacional para maximizar el multiplicador fiscal."
    ),

    "Laboratorio de Investigación": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Laboratorio de Investigación (Research Facility) es la instalación científica avanzada donde los científicos del Imperio desarrollan nuevas tecnologías.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Generación de RP: Cada laboratorio produce Puntos de Investigación (RP) anuales (ej. 200 RP/año por laboratorio base).\n"
        "• Asignación a Científicos: Se agrupan bajo el mando de un Científico Líder para investigar proyectos específicos en la cola de I+D.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Asignar 20 laboratorios a un científico de 'Energía' acelerará la investigación de escudos de plasma a la mitad de tiempo.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Construye y mantén al menos 30 a 50 laboratorios para liderar la carrera tecnológica frente a especies extraterrestres (NPRs)."
    ),

    "Research Facility": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Laboratorio de Investigación (Research Facility) es la instalación científica avanzada donde los científicos del Imperio desarrollan nuevas tecnologías.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Generación de RP: Cada laboratorio produce Puntos de Investigación (RP) anuales (ej. 200 RP/año por laboratorio base).\n"
        "• Asignación a Científicos: Se agrupan bajo el mando de un Científico Líder para investigar proyectos específicos en la cola de I+D.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Asignar 20 laboratorios a un científico de 'Energía' acelerará la investigación de escudos de plasma a la mitad de tiempo.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Construye y mantén al menos 30 a 50 laboratorios para liderar la carrera tecnológica frente a especies extraterrestres (NPRs)."
    ),

    "Infraestructura Poblacional": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Infraestructura Poblacional (Infrastructure) abarca los complejos domos ambientales, generadores de soporte vital, recicladores de agua y escudos térmicos que permiten la supervivencia en planetas hostiles.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Soporte Vital en Mundos Hostiles: Necesaria en planetas con Costo Colonial > 0.00 (ej. Marte, Venus, Titán).\n"
        "• Evita la Escasez y Muertes Civiles: Si la población supera la capacidad de la infraestructura presente, se producirá un colapso atmosférico y mortandad masiva.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Para trasladar 10 millones de colonos a Marte (Costo Colonial 2.00), necesitarás transportar previamente 2,000 unidades de Infraestructura.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Utiliza cargueros comerciales (Freighters) para mover infraestructura sobrante desde la Tierra hacia colonias en desarrollo."
    ),

    "Infrastructure": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Infraestructura Poblacional (Infrastructure) abarca los complejos domos ambientales, generadores de soporte vital, recicladores de agua y escudos térmicos que permiten la supervivencia en planetas hostiles.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Soporte Vital en Mundos Hostiles: Necesaria en planetas con Costo Colonial > 0.00 (ej. Marte, Venus, Titán).\n"
        "• Evita la Escasez y Muertes Civiles: Si la población supera la capacidad de la infraestructura presente, se producirá un colapso atmosférico y mortandad masiva.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Para trasladar 10 millones de colonos a Marte (Costo Colonial 2.00), necesitarás transportar previamente 2,000 unidades de Infraestructura.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Utiliza cargueros comerciales (Freighters) para mover infraestructura sobrante desde la Tierra hacia colonias en desarrollo."
    ),

    "Mina Automatizada": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Mina Automatizada (Automated Mine) es un complejo extractivo robótico diseñado para operar en mundos sin atmósfera ni población humana asentada.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Extracción Sin Colonos: Extrae minerales trans-newtonianos al 100% de rendimiento sin requerir habitantes ni infraestructura de soporte vital.\n"
        "• Portátil: Puede ser desmontada por barcos cargueros y relocalizada cuando las vetas del asteroide se agoten.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Desplegar 50 Minas Automatizadas en el asteroide Ceres extraerá Duranium y Sorium de forma continua sin gastar comida ni domos.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Asigna flotas de cargueros para trasladar minas automatizadas a asteroides con accesibilidad mineral alta (1.0)."
    ),

    "Automated Mine": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Mina Automatizada (Automated Mine) es un complejo extractivo robótico diseñado para operar en mundos sin atmósfera ni población humana asentada.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Extracción Sin Colonos: Extrae minerales trans-newtonianos al 100% de rendimiento sin requerir habitantes ni infraestructura de soporte vital.\n"
        "• Portátil: Puede ser desmontada por barcos cargueros y relocalizada cuando las vetas del asteroide se agoten.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Desplegar 50 Minas Automatizadas en el asteroide Ceres extraerá Duranium y Sorium de forma continua sin gastar comida ni domos.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Asigna flotas de cargueros para trasladar minas automatizadas a asteroides con accesibilidad mineral alta (1.0)."
    )
}

# Add installations
for k, v in installations.items():
    existing_dict[k] = v

# -----------------------------------------------------------------------------
# 2. COMPONENT CATEGORIES & SHIP SYSTEMS (All Naval & Designer Modules)
# -----------------------------------------------------------------------------
component_categories = {
    "Fighter Pod Bay": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Bahía de Despliegue de Vástagos / Cazas (Fighter Pod Bay / Parasite Bay) es una estructura naval interna para transportar, rearmar y lanzar cazas estelares (Pequeñas naves < 500t).\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Operación de Cazas: Mantiene a los cazas protegidos dentro de la nave nodriza o portacazas durante el viaje hiperespacial.\n"
        "• Reabastecimiento y Reparación: Repone la munición y el combustible de los cazas embarcados al aterrizar en la bahía.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un portacazas equipado con 10 Fighter Pod Bays puede lanzar una escuadra entera de 10 interceptores armados con misiles ligeros para realizar un ataque relámpago.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Combina Fighter Pod Bays con Hangares de Mantenimiento para evitar el deterioro de los cazas en misiones de larga duración."
    ),

    "Active Search Sensor": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Radar de Búsqueda Activa (Active Search Sensor). Emite impulsos de radiación electromagnética para detectar la presencia, rumbo y velocidad de naves y misiles enemigos.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Detección Físicamente Precisa: El alcance depende directamente del Tamaño del Sensor, la Potencia Tecnológica y la Resolución elegida.\n"
        "• Resolución 1: Diseñado para detectar pequeños objetivos y misiles veloces.\n"
        "• Resolución 100+: Diseñado para detectar naves capitales a cientos de millones de kilómetros.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un radar activo Res 100 de 5 HS localizará un destructor enemigo a 250 Mkm, permitiendo fijar blancos a larga distancia.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Mantén los radares activos apagados durante aproximaciones sigilosas; encender el radar delata tu posición exacta a los sensores pasivos enemigos."
    ),

    "Thermal Sensor": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Sensor Térmico Pasivo (Thermal Sensor / Passive IR). Detecta el calor y la firma de emisión de infrarrojos (TH) generados por los motores y reactores enemigas.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Detección 100% Silenciosa: No emite señales, por lo que el enemigo nunca sabrá que lo estás rastreando.\n"
        "• Sensibilidad Tecnológica: A mayor sensibilidad investigada, mayor distancia de detección pasiva.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un piquete de exploración con un Thermal Sensor de alta sensibilidad detectará a una flota enemiga navegando a toda velocidad sin revelar su propia posición.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Instala siempre sensores térmicos pasivos en boyas de vigilancia orbital y naves de vanguardia."
    ),

    "EM Sensor": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Sensor Electromagnético Pasivo (EM Sensor). Captura la radiación pasiva de escudos energéticos activos, emisiones de radar y frecuencias de comunicación enemigas.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Rastreo de Escudos y Radares: Detecta al instante si una nave enemiga tiene sus escudos encendidos o su radar activo operativo.\n"
        "• Totalmente Pasivo: Permite vigilar puntos de salto sin alertar a los intrusos.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Si una flota extraterrestre enciende sus escudos de fuerza al entrar en el sistema, tu EM Sensor detectará la firma energética al instante.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Utiliza la combinación de sensores pasivos Térmico + EM para obtener un cuadro de inteligencia completo sin delatar tu posición."
    ),

    "Laser": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Arma de Energía Láser Focalizada (Laser Focal Weapon). Emite haces de fotones de alta energía en longitudes de onda infrarrojas, visibles o ultravioleta.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Daño Focal Directo: Penetra profundamente en las capas de blindaje enemigo creando un cráter cónico de penetración.\n"
        "• Sin Consumo de Munición: Solo requiere energía producida por los reactores energéticos de a bordo y capacitores recargados.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un Láser Ultravioleta de 25cm causa 16 puntos de daño de impacto directo, atravesando 4 capas completas de blindaje enemigo de un solo disparo.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Combina láseres pesados con controles de tiro de alta velocidad para perforar cascos enemigos en combates a corta y media distancia."
    ),

    "Shield Generator": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Generador de Escudos de Fuerza (Force Shield Generator). Crea una barrera energética defensiva alrededor del casco de la nave espacial.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Absorción de Daño Absoluto: Los escudos absorben impactos de misiles, láseres y plasma antes de que toquen el blindaje o el casco.\n"
        "• Recarga Automática: Se regeneran de forma continua a lo largo del tiempo si la nave mantiene combustible y energía.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un crucero con 40 puntos de escudo absorberá una salva entera de 8 misiles enemigos sin sufrir una sola grieta en su blindaje metálico.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Mantén los escudos encendidos durante el combate, pero apágalos fuera de combate para ahorrar combustible y reducir la firma electromagnética."
    ),

    "Engine": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Motor de Propulsión Espacial Trans-Newtoniana (Space Engine). Proporciona empuje de aceleración (Power/EP) para mover la nave por el sistema estelar.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Determina la Velocidad Naval: La velocidad en km/s equivale al Empuje Total del Motor dividido por el Tamaño Total del Casco (HS).\n"
        "• Consumo de Combustible: La potencia, eficiencia y multiplicador de potencia determinan el consumo de litros por hora (LPH).\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un destructor con 4 motores Ion Drive de 250 EP alcanzará los 6,000 km/s, superando fácilmente la velocidad de huida enemiga.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Diseña motores comerciales grandes (50 HS) y de baja potencia para cargueros, y motores militares pequeños de alta potencia para buques de guerra."
    )
}

# Add component categories
for k, v in component_categories.items():
    existing_dict[k] = v

with open(existing_json_path, 'w', encoding='utf-8') as f:
    json.dump(existing_dict, f, ensure_ascii=False, indent=2)

print("Comprehensive database enriched with total entries:", len(existing_dict))

portable_json_path = 'c:/VSCODE/Aurora_Command_Suite_v2.7.1_Portable/App/config/AuroraTooltipDictionary.json'
game_patch_json_path = 'c:/VSCODE/Aurora271Full/Patches/AuroraSpanish/AuroraTooltipDictionary.json'

for target_path in [portable_json_path, game_patch_json_path]:
    try:
        os.makedirs(os.path.dirname(target_path), exist_ok=True)
        with open(target_path, 'w', encoding='utf-8') as f:
            json.dump(existing_dict, f, ensure_ascii=False, indent=2)
        print("Updated target path:", target_path)
    except Exception as e:
        print("Error:", e)
