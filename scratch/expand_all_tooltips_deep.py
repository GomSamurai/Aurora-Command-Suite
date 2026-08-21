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
# 1. EXHAUSTIVE DEEP CUSTOM TUTOR ARTICLES (Multi-Paragraph & Deeply Detailed)
# -----------------------------------------------------------------------------
deep_custom_articles = {
    "CIWS": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "En Aurora 4X, las siglas CIWS significan Close-In Weapon System (Sistema de Armamento de Corto Alcance o Defensa de Punto Terminal).\n"
        "Se trata de un módulo integrado diseñado como la última línea defensiva de una nave frente a ataques de misiles enemigos entrantes.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Todo en uno (Self-contained): Integra en un solo componente el cañón (basado en tecnología Gauss), la montura de torreta, el sensor/radar de seguimiento y el director de tiro de defensa de punto (Fire Control).\n"
        "• Defensa puramente individual: Un sistema CIWS solo defiende a la nave en la que está montado durante la fase final (Final Fire a 10,000 km). No puede proporcionar cobertura o escolta a otras naves de la flota.\n"
        "• Componente comercial/militar: No requiere diseño manual complejo de torretas ni cálculo de seguimiento de directores de tiro separados, y se genera directamente desde la ventana de diseño de componentes.\n"
        "• Sin requerimiento de energía reactiva ni munición: Al ser de diseño cerrado y basado en tecnología cinética/Gauss para PD, opera automáticamente cada vez que un misil hostil está a punto de impactar la nave.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Si una nave civil, carguero pesado o buque auxiliar es sorprendido por una salva de 4 misiles anti-buque enemiga, un módulo CIWS Phalanx-S5000 instalado a bordo abrirá fuego automáticamente a 10,000 km destruyendo los misiles entrantes sin necesidad de contar con un control de tiro de vigía ni torretas asignadas.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Instala al menos 1 o 2 módulos CIWS en todas tus naves comerciales (Freighters, Colony Ships, Harvesters) y buques auxiliares. Al ser un módulo autónomo, convertirá a tus cargueros en plataformas autorresistentes frente a pequeñas incursiones de piratas espaciales o cazas enemigos."
    ),

    "Fighter Pod Bay": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Bahía de Despliegue de Vástagos o Cazas (Fighter Pod Bay / Parasite Bay) es una estructura interna ligera especializada en transportar, reparar, rearmar y desplegar pequeñas naves parásito (< 500 toneladas o < 10 HS).\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Operación de Cazas e Interceptores: Mantiene a los cazas protegidos en el interior de la nave nodriza durante los saltos hiperespaciales y aproximaciones.\n"
        "• Reabastecimiento Automático: Al aterrizar en la bahía, el caza repone sus reservas de combustible hidrocarburo y recarga sus pañoles de misiles ligeros.\n"
        "• Sin Pérdida de Mantenimiento: Impide que los cazas sufran fallos de mantenimiento por desgaste mientras permanezcan atracados dentro de la bahía.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un portacazas ligero de 10,000 toneladas equipado con 12 Fighter Pod Bays puede transportar una escuadra entera de interceptores Gauss. Al detectar la flota enemiga, los cazas despegan instantáneamente, ejecutan el ataque y regresan a repostar en minutos.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Equipa siempre tus portacazas con bahías de aterrizaje suficientes y asegúrate de contar con tanques de combustible y pañoles de munición sobrantes en la nave matriz."
    ),

    "Active Search Sensor": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Radar de Búsqueda Activa (Active Search Sensor). Es un sistema emisor de energía electromagnética que escanea el espacio profundo para detectar e identificar naves, estaciones y misiles hostiles.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Detección Físicamente Precisa: El alcance de detección depende de la Potencia del Sensor (Active Sensor Strength), la Resolución elegida (Res 1 a Res 100+) y la firma del objetivo (TCS).\n"
        "• Resolución 1 (Res 1): Especializada en detectar objetivos diminutos como misiles y cazas a corta/media distancia.\n"
        "• Resolución 100 (Res 100): Diseñada para localizar naves capitales de 5,000 toneladas a cientos de millones de kilómetros de distancia.\n"
        "• Emisión Delatadora: Al encender el radar activo, tu nave emite un impulso masivo que la vuelve visible instantáneamente ante los sensores pasivos enemigos en todo el sistema estelar.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un radar activo Res 100 de 5 HS detectará un crucero enemigo a 300 Mkm, pero revelará tu presencia al enemigo al mismo tiempo.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Mantén los radares activos APAGADOS durante misiones de emboscada. Emplea sensores pasivos (Térmico y EM) para rastrear al enemigo y enciende el radar activo únicamente segundos antes de lanzar la salva de misiles."
    ),

    "Active Sensors": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Radares y Sensores Activos de Búsqueda y Guiado (Active Sensors). Componentes que emiten radiación electromagnética para detectar objetivos en el espacio.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Proporcionan coordenadas de telemetría exacta a los sistemas de control de tiro (Fire Control).\n"
        "• Permiten fijar blancos para misiles de largo alcance y cañones de energía.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Sin un sensor activo encendido o un barco piquete de radar, tus misiles anti-buque no podrán adquirir el objetivo enemigo a larga distancia.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Diseña naves especializadas 'Piquete de Radar' para liderar la flota y evitar exponer a los acorazados principales."
    ),

    "Thermal Sensor": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Sensor Térmico Pasivo (Thermal Sensor / Passive IR). Sistema de detección optoelectrónico e infrarrojo que capta el radiador térmico y el calor emitido por motores y sistemas de a bordo enemigos.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Detección 100% Silenciosa: No emite energía en absoluto. Permite rastrear flotas enemigas sin que el enemigo sepa que está siendo observado.\n"
        "• Sensibilidad Infrarroja: Mide la firma térmica (Thermal Signature / TH) de la nave enemiga, la cual aumenta con la velocidad y la potencia del motor.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Si una flota enemiga navega a 8,000 km/s con sus motores a máxima potencia, un Thermal Sensor de alta sensibilidad la detectará a más de 150 Mkm en silencio absoluto.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Instala sensores térmicos en boyas de monitoreo pasivo cerca de los Puntos de Salto para registrar todo el tráfico enemigo."
    ),

    "Thermal Sensors": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Sensor Térmico Pasivo (Thermal Sensor / Passive IR). Sistema de detección optoelectrónico e infrarrojo que capta el radiador térmico y el calor emitido por motores y sistemas de a bordo enemigos.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Detección 100% Silenciosa: No emite energía en absoluto. Permite rastrear flotas enemigas sin que el enemigo sepa que está siendo observado.\n"
        "• Sensibilidad Infrarroja: Mide la firma térmica (Thermal Signature / TH) de la nave enemiga, la cual aumenta con la velocidad y la potencia del motor.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Si una flota enemiga navega a 8,000 km/s con sus motores a máxima potencia, un Thermal Sensor de alta sensibilidad la detectará a más de 150 Mkm en silencio absoluto.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Instala sensores térmicos en boyas de monitoreo pasivo cerca de los Puntos de Salto para registrar todo el tráfico enemigo."
    ),

    "EM Sensor": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Sensor Electromagnético Pasivo (EM Sensor). Escáner pasivo que capta la radiación de frecuencia, escudos de fuerza activos y radares enemigos en funcionamiento.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Rastreo de Escudos y Radares: Si una nave enemiga activa sus escudos de fuerza o enciende su radar de búsqueda, su firma electromagnética (EM) se dispara, permitiendo detectarla a grandes distancias.\n"
        "• Pasivo y Sigiloso: Funciona en silencio absoluto sin revelar la ubicación de tu propia nave.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Cuando un acorazado enemigo enciende sus escudos de energía antes de entrar al combate, tu EM Sensor captará el pico electromagnético al instante.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Combina sensores Térmicos y EM en todas tus naves de reconocimiento para lograr una cobertura pasiva completa."
    ),

    "EM Detection Sensors": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Sensor Electromagnético Pasivo (EM Sensor). Escáner pasivo que capta la radiación de frecuencia, escudos de fuerza activos y radares enemigos en funcionamiento.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Rastreo de Escudos y Radares: Si una nave enemiga activa sus escudos de fuerza o enciende su radar de búsqueda, su firma electromagnética (EM) se dispara, permitiendo detectarla a grandes distancias.\n"
        "• Pasivo y Sigiloso: Funciona en silencio absoluto sin revelar la ubicación de tu propia nave.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Cuando un acorazado enemigo enciende sus escudos de energía antes de entrar al combate, tu EM Sensor captará el pico electromagnético al instante.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Combina sensores Térmicos y EM en todas tus naves de reconocimiento para lograr una cobertura pasiva completa."
    ),

    "Laser": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Arma de Energía Láser Focalizada (Laser Focal Weapon). Dispositivo que proyecta un haz coherente de fotones concentrados de alta energía en longitudes de onda infrarroja, visible o ultravioleta.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Daño Focal Penetrante: A diferencia de las ojivas de misiles que causan daños superficiales anchos, el láser crea un cráter profundo en columna recta a través del blindaje enemigo.\n"
        "• Munición Infinita: No consume munición de pañol; únicamente requiere recarga de energía mediante los capacitores y reactores de a bordo.\n"
        "• Atenuación por Distancia: El daño decrece progresivamente a mayor distancia del objetivo.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un láser ultravioleta de 25cm causa 16 puntos de daño en el punto de impacto a corta distancia, perforando hasta 4 capas completas de blindaje de un solo disparo.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Combina cañones láser pesados con directores de tiro de alto alcance (Beam Fire Control) para destruir los componentes internos de naves enemigas desprovistas de escudos."
    ),

    "Lasers": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Arma de Energía Láser Focalizada (Laser Focal Weapon). Dispositivo que proyecta un haz coherente de fotones concentrados de alta energía en longitudes de onda infrarroja, visible o ultravioleta.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Daño Focal Penetrante: A diferencia de las ojivas de misiles que causan daños superficiales anchos, el láser crea un cráter profundo en columna recta a través del blindaje enemigo.\n"
        "• Munición Infinita: No consume munición de pañol; únicamente requiere recarga de energía mediante los capacitores y reactores de a bordo.\n"
        "• Atenuación por Distancia: El daño decrece progresivamente a mayor distancia del objetivo.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un láser ultravioleta de 25cm causa 16 puntos de daño en el punto de impacto a corta distancia, perforando hasta 4 capas completas de blindaje de un solo disparo.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Combina cañones láser pesados con directores de tiro de alto alcance (Beam Fire Control) para destruir los componentes internos de naves enemigas desprovistas de escudos."
    ),

    "Shield Generator": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Generador de Escudos de Fuerza (Force Shield Generator). Proyector de campo electromagnético de alta densidad que envuelve el casco de la nave espacial con una capa defensiva regenerativa.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Absorción Total de Daño: Los escudos absorben impactos de misiles, láseres, plasma y cañones antes de que la energía toque el blindaje o la estructura física del barco.\n"
        "• Regeneración Continua: Una vez dañados o agotados, se recargan automáticamente a una tasa fija por minuto si la nave dispone de energía y combustible.\n"
        "• Firma EM Aumentada: Mantener los escudos activados incrementa la visibilidad electromagnética de la nave ante los sensores pasivos enemigos.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un acorazado equipado con 50 puntos de escudo absorberá una salva entera de 10 misiles enemigos sin sufrir un solo rasguño en su blindaje metálico.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Enciende los escudos antes de entrar en la zona de engagement enemiga, pero mantenlos apagados durante travesías largas para reducir la visibilidad y ahorrar combustible."
    ),

    "Shield Generators": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Generador de Escudos de Fuerza (Force Shield Generator). Proyector de campo electromagnético de alta densidad que envuelve el casco de la nave espacial con una capa defensiva regenerativa.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Absorción Total de Daño: Los escudos absorben impactos de misiles, láseres, plasma y cañones antes de que la energía toque el blindaje o la estructura física del barco.\n"
        "• Regeneración Continua: Una vez dañados o agotados, se recargan automáticamente a una tasa fija por minuto si la nave dispone de energía y combustible.\n"
        "• Firma EM Aumentada: Mantener los escudos activados incrementa la visibilidad electromagnética de la nave ante los sensores pasivos enemigos.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un acorazado equipado con 50 puntos de escudo absorberá una salva entera de 10 misiles enemigos sin sufrir un solo rasguño en su blindaje metálico.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Enciende los escudos antes de entrar en la zona de engagement enemiga, pero mantenlos apagados durante travesías largas para reducir la visibilidad y ahorrar combustible."
    ),

    "Engine": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Motor de Propulsión Espacial (Space Engine). Sistema de reacción o propulsión magnética que genera empuje vectorial (EP) para mover la nave a través del vacío espacial.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Determina la Velocidad Naval: La velocidad en km/s equivale al Empuje Total (EP) dividido por el Tamaño del Casco (HS).\n"
        "• Consumo de Combustible: La tecnología base, el tamaño del motor y el multiplicador de potencia (0.1x a 3.0x) dictan el consumo de litros por hora (LPH).\n"
        "• Firma Térmica (TH): Los motores son la mayor fuente de calor de la nave. A mayor velocidad y potencia, mayor visibilidad ante sensores térmicos pasivos.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un destructor de 5,000 toneladas equipado con 4 motores Ion Drive alcanzará los 6,500 km/s, permitiéndole dar caza a cargueros hostiles o maniobrar ante salvas de misiles.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Diseña motores comerciales grandes (50 HS) a baja potencia (0.25x) para cargueros, y motores militares pequeños de alta potencia (1.5x - 2.0x) para buques de guerra."
    ),

    "Engines": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Motor de Propulsión Espacial (Space Engine). Sistema de reacción o propulsión magnética que genera empuje vectorial (EP) para mover la nave a través del vacío espacial.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Determina la Velocidad Naval: La velocidad en km/s equivale al Empuje Total (EP) dividido por el Tamaño del Casco (HS).\n"
        "• Consumo de Combustible: La tecnología base, el tamaño del motor y el multiplicador de potencia (0.1x a 3.0x) dictan el consumo de litros por hora (LPH).\n"
        "• Firma Térmica (TH): Los motores son la mayor fuente de calor de la nave. A mayor velocidad y potencia, mayor visibilidad ante sensores térmicos pasivos.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un destructor de 5,000 toneladas equipado con 4 motores Ion Drive alcanzará los 6,500 km/s, permitiéndole dar caza a cargueros hostiles o maniobrar ante salvas de misiles.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Diseña motores comerciales grandes (50 HS) a baja potencia (0.25x) para cargueros, y motores militares pequeños de alta potencia (1.5x - 2.0x) para buques de guerra."
    ),

    "Academia Militar": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Academia Militar (Military Academy) es la institución de formación superior donde se educan y gradúan los oficiales de la armada, comandantes de flota, gobernadores planetarios y científicos del Imperio.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Generación de Oficiales: Cada Academia Militar graduará entre 1 y 3 nuevos oficiales por año en función de las políticas imperiales.\n"
        "• Promoción y Reclutamiento: Esencial para mantener cubiertos los puestos de mando en nuevas naves de guerra y colonias recién fundadas.\n"
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
    )
}

# Update dictionary with deep custom articles
for k, v in deep_custom_articles.items():
    existing_dict[k] = v

with open(existing_json_path, 'w', encoding='utf-8') as f:
    json.dump(existing_dict, f, ensure_ascii=False, indent=2)

print("Expanded deep tutor articles count:", len(existing_dict))

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
