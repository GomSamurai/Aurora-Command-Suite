import json
import os

json_paths = [
    r"c:\VSCODE\AuroraDesignSuite\config\AuroraTooltipDictionary.json",
    r"c:\VSCODE\Aurora_Command_Suite_v2.7.1_Portable\App\config\AuroraTooltipDictionary.json",
    r"c:\VSCODE\Aurora271Full\Patches\AuroraSpanish\AuroraTooltipDictionary.json"
]

component_tooltips = {
    "Commercial Nuclear Thermal Engine (HS 50)": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Commercial Nuclear Thermal Engine (HS 50) es un motor de propulsión espacial de gran tonelaje optimizado para cargueros, colonizadores y tanqueros comerciales.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Uso Comercial: Posee un factor de consumo de combustible hiper-eficiente pero un desplazamiento masivo (50 HS = 2,500 toneladas).\n"
        "• Resistencia y Costo: Diseñado para operar durante décadas en espacio profundo sin requerir mantenimiento militar frecuente (MSP).\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Equipar 4 de estos motores comerciales en un carguero pesado de 50,000 toneladas proporcionará velocidad de tránsito interplanetario con consumo mínimo de Sorium.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "No utilices este motor en buques de combate militar, ya que su gran tamaño aumentará drásticamente la firma térmica de la nave facilitando su detección enemiga."
    ),
    "Magneto-Plasma Drive (HS 10)": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Magneto-Plasma Drive (HS 10) es un motor espacial de empuje militar acelerado para naves de guerra y escolta táctica.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Aceleración Táctica: Genera una alta relación de empuje por tonelada (Engine Power), permitiendo a la nave alcanzar velocidades de combate de 3,000+ km/s.\n"
        "• Consumo Militar: Consume Sorium a una tasa superior a los motores comerciales, requiriendo tanques de combustible de apoyo.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Montar 4 de estos motores en un destructor de 9,000 toneladas le otorgará la velocidad necesaria para esquivar salvas de misiles y perseguir cruceros enemigos.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Ajusta el multiplicador de potencia del motor (Engine Power Modifier) al 1.5x en naves de intercepción corta para maximizar la velocidad máxima."
    ),
    "Standard Fuel Tank (50k Liters)": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Standard Fuel Tank (50k Liters) es un depósito de almacenamiento compacto de combustible hidrocarburo Sorium LPH para naves ligeras y cazas.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Capacidad de Combustible: Almacena 50,000 litros de combustible hidrocarburo procesado por cada unidad de 1 HS (50 toneladas).\n"
        "• Autonomía Operativa: Aumenta el rango de navegación en kilómetros y horas de vuelo a máxima velocidad.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Instalar 2 tanques estándar en una corbeta de exploración le proporcionará combustible suficiente para mapear 3 sistemas estelares vecinos.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Combina con motores de bajo consumo en buques de patrulla para lograr autonomías de más de 5,000 millones de kilómetros."
    ),
    "Large Fuel Tank (250k Liters)": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Large Fuel Tank (250k Liters) es un tanque de almacenamiento de combustible de gran capacidad para naves capitales, cruceros y cargueros.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Capacidad de Combustible: Almacena 250,000 litros de Sorium LPH ocupando 5 HS (250 toneladas) en la estructura del casco.\n"
        "• Densidad de Almacenamiento: Ofrece una relación ideal entre volumen y costo de construcción en Duranium/Sorium.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un crucero de combate equipado con 8 tanques grandes tendrá autonomía para operar en el frente de batalla durante más de 3 años sin regresar a la base.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Protege los tanques de combustible colocándolos detrás de varias capas de blindaje de Neutronium para evitar detonaciones catastróficas por penetración."
    ),
    "Very Large Fuel Tank (1M Liters)": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Very Large Fuel Tank (1M Liters) es un supertanque masivo de combustible para buques nodriza, petroleros de flota y estaciones de almacenamiento.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Capacidad Masiva: Almacena 1,000,000 de litros de Sorium LPH ocupando 20 HS (1,000 toneladas).\n"
        "• Logística de Reabastecimiento: Actúa como reserva flotante para reabastecer a flotas completas de destructores mediante mangueras de repostaje.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Equipar 10 de estos tanques en un buque petrolero permitirá reabastecer al 100% las reservas de toda la Battle Fleet en espacio profundo.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Asigna buques tanque con este componente a la Cargo Fleet para abastecer colonias exteriores que no dispongan de Refinerías."
    ),
    "Crew Quarters (50 Crew Capacity)": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Crew Quarters (50 Crew Capacity) es el módulo de habitabilidad y alojamiento para oficiales y marineros imperiales a bordo.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Soporte Vital (Habitation): Aloja a 50 miembros de la tripulación en condiciones ambientales adecuadas por cada 1 HS de módulo.\n"
        "• Moral y Tiempo de Despliegue: Evita motines y caídas en la efectividad operativa cuando la travesía supera los meses previstos.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Si tu nave requiere 200 tripulantes para operar armas y motores, necesitarás instalar exactamente 4 módulos de Crew Quarters.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Utiliza la herramienta de auto-balance de habitabilidad en el Diseñador para no olvidar añadir los dormitorios requeridos por la tripulación."
    ),
    "Engineering Spaces": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Engineering Spaces es el taller mecánico de ingeniería a bordo y pañol de repuestos de mantenimiento (MSP).\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Repuestos MSP (Maintenance Supplies): Almacena materiales de repuesto para reparar fallos de motor o averías en combate.\n"
        "• Tasa de Fallos Anual (AFR): Reduce la probabilidad de fallos mecánicos fortuitos en buques militares desplegados durante períodos prolongados.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Cuando un cañón láser sufra un fallo por fatiga en medio de una batalla, el equipo de ingeniería usará los repuestos MSP de este módulo para repararlo al instante.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Instala siempre suficientes espacios de ingeniería para que la 'Vida de Mantenimiento' supere el tiempo de despliegue militar proyectado."
    ),
    "Alpha Shield Generator": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Alpha Shield Generator es un emisor de campo de fuerza deflector para la protección de cascos navales.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Absorción de Impactos: Genera un escudo de energía de Fuerza 6 que absorbe disparos de láser, proyectiles y misiles antes de que toquen la armadura.\n"
        "• Regeneración Automática: Se recarga gradualmente consumiendo energía producida por los reactores de potencia de la nave.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Instalar 4 generadores Alpha en un crucero le proporcionará 24 puntos de escudo deflector continuo en combate.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Asegúrate de llevar reactores de potencia dedicados para no desenergizar los escudos en medio de una salva enemiga."
    ),
    "Military Jump Drive (Max 10,000 Tons)": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Military Jump Drive (Max 10,000 Tons) es un motor de salto hiper-espacial para despliegue de escuadras militares a través de Puntos de Salto.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Salto Táctico: Abre un túnel gravitatorio que permite transitar a naves de hasta 10,000 toneladas de desplazamiento.\n"
        "• Capacidad de Escuadra: Transporta simultáneamente a naves acompañantes de combate que no dispongan de motor de salto propio.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Una nave insignia equipada con este motor de salto puede guiar a una escuadra de 4 destructores de 9,000 t hacia un nuevo sistema solar.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Combina con científicos especializados en 'Jump Drive Efficiency' para reducir el tonelaje ocupado por el motor en el casco."
    ),
    "15cm C3 Near-Ultraviolet Laser": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "15cm C3 Near-Ultraviolet Laser es una batería naval de haz calórico ultravioleta de alta penetración.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Daño y Penetración: Inflige 6 puntos de daño directo por disparo, penetrando profundamente en las capas de blindaje metálico enemigo.\n"
        "• Longitud de Onda UV: Mantiene una elevada densidad energética a distancias de más de 100,000 kilómetros.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Una salva de 4 láseres UV destruirá la primera capa de blindaje de un crucero hostil y dañará sus sistemas internos en el segundo turno.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Combina con directores de tiro de alta velocidad y capacitores de recarga nivel 4 para disparar salvas continuas cada 10 segundos."
    ),
    "Active Search Sensor Res-20 (50M km)": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Active Search Sensor Res-20 (50M km) es un radar activo de resolución 20 optimizado para la detección de naves de tamaño medio y cazas.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Alcance de Radar: Emite impulsos electromagnéticos que detectan naves enemigas a una distancia de hasta 50 millones de kilómetros.\n"
        "• Fijación de Blancos: Proporciona la telemetría indispensable para que los controles de tiro de misiles y cañones apunten a los objetivos.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Activar este sensor en el destructor de vanguardia revelará inmediatamente la posición de una flota alienígena que se aproxime al sistema.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Mantén el radar activo apagado hasta entrar en zona de contacto para evitar delatar la posición de tu propia flota."
    ),
    "Thermal Sensor Array (TH-10)": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Thermal Sensor Array (TH-10) es un sensor pasivo ultrasensible de firmas infrarrojas y térmicas de motores.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Detección Silenciosa: Capta el calor emitido por los propulsores de naves enemigas sin emitir señales de radar detectables.\n"
        "• Escaneo Perimétrico: Funciona continuamente en 360 grados sin gastar energía de los reactores.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Detectará la aproximación de un convoy enemigo a través del calor de sus motores a más de 30 millones de km de distancia sin revelar tu presencia.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Equipa a todos tus piquetes de exploración con sensores térmicos pasivos para mantener una vigilancia silenciosa e impecable."
    ),
    "Size 6 Missile Magazine (Capacity 120)": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Size 6 Missile Magazine (Capacity 120) es un pañol blindado de almacenamiento y suministro de misiles de tamaño 6.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Almacenamiento de Munición: Capacidad para alojar hasta 20 misiles de Tamaño 6 (o 120 MSP equivalentes en misiles menores).\n"
        "• Alimentación Automática: Conecta directamente con los tubos lanzadores para rearmar salvas en cada turno de combate.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Permite que una fragata lanzamisiles ejecute 10 salvas consecutivas de 2 misiles de asedio antes de necesitar regresar a la base para rearmar.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Añade un módulo de protección en el pañol (Magazine Armor) para evitar que impactos enemigos provoquen detonaciones de la munición almacenada."
    ),
    "MK I Commercial Active Augur Array": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "MK I Commercial Active Augur Array es un sensor de exploración de largo alcance y escaneo espacial para buques comerciales e industriales.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Detección Comercial: Permite a los cargueros y naves mineras detectar cuerpos celestes, asteroides y tráfico comercial en el sector.\n"
        "• Operatividad Autónoma: No requiere entrenamiento militar de tripulación para operar en espacio profundo.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Instalado en una nave minera o carguero comercial, proporcionará alerta temprana ante la aproximación de piratas o naves desconocidas.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Utilízalo en buques de apoyo comercial para mantener cartografiada la ruta de transporte sin consumir recursos de sensores militares."
    )
}

for path in json_paths:
    if os.path.exists(path):
        with open(path, "r", encoding="utf-8") as f:
            data = json.load(f)

        for k, v in component_tooltips.items():
            data[k] = v

        with open(path, "w", encoding="utf-8") as f:
            json.dump(data, f, ensure_ascii=False, indent=2)

        print(f"Enriched components in {path}: Added {len(component_tooltips)} items. Total items: {len(data)}")
