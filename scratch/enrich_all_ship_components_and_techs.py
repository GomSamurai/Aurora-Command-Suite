import json
import os
import re

json_paths = [
    r"c:\VSCODE\AuroraDesignSuite\config\AuroraTooltipDictionary.json",
    r"c:\VSCODE\Aurora_Command_Suite_v2.7.1_Portable\App\config\AuroraTooltipDictionary.json",
    r"c:\VSCODE\Aurora271Full\Patches\AuroraSpanish\AuroraTooltipDictionary.json"
]

def generate_flawless_component_tooltip(key):
    k = key.strip()
    k_lower = k.lower()

    # 1. MAINTENANCE & REPAIR STORAGE / ENGINEERING
    if any(x in k_lower for x in ["maintenance", "maint", "engineering", "repuestos", "msp", "taller", "dique"]):
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es el módulo de almacenamiento de repuestos de mantenimiento (MSP) y espacios de ingeniería de la nave.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Repuestos MSP (Maintenance Supplies): Acumula materiales mecánicos para reparar averías en combate y prevenir colapsos por fatiga metálica.\n"
            f"• Control de Daños (Damage Control): Permite a las cuadrillas de reparación arreglar motores, cañones o sensores destruidos por fuego enemigo.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Si un rayo láser enemigo destruye el motor principal de tu crucero, el equipo de ingeniería usará los repuestos de este almacén para repararlo e impedir que la nave quede a la deriva.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Instala siempre suficientes espacios de mantenimiento para que la 'Vida de Mantenimiento' supere el tiempo de despliegue militar proyectado."
        )

    # 2. ENGINES & PROPULSION
    elif any(x in k_lower for x in ["engine", "drive", "motor", "propulsion", "propulsor", "ep1", "ep2", "ep3", "ep4", "ep5", "ep6", "ep7", "ep8", "ep9", "nuclear", "plasma", "magneto", "photonic", "thermal"]):
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es un impulsor de reacción espacial encargado de convertir combustible hidrocarburo Sorium LPH en empuje (EP).\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Empuje y Velocidad: Determina la velocidad máxima en km/s de la nave (Velocidad = Total Empuje / Total HS * 1000).\n"
            f"• Eficiencia de Combustible: Los motores comerciales reducen el consumo a niveles mínimos; los militares maximizan la aceleración táctica en combate.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Instalar 4 de estos motores en un destructor de 9,000 toneladas le proporcionará la velocidad necesaria para esquivar salvas de torpedos y perseguir cruceros enemigos.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Combina con multiplicadores de potencia optimizados en el Diseñador para equilibrar la velocidad táctica con el consumo de Sorium LPH."
        )

    # 3. FUEL STORAGE & TANKS
    elif any(x in k_lower for x in ["fuel", "tanque", "combustible", "storage"]):
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es un depósito de almacenamiento de combustible hidrocarburo Sorium LPH para la autonomía naval.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Almacenamiento de Litros: Acumula miles de litros de Sorium refinado para alimentar los propulsores espaciales.\n"
            f"• Rango Operativo: La capacidad total dividida por el consumo por hora del motor determina el alcance máximo en kilómetros y años-luz.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Equipar tanques de combustible de gran capacidad en un crucero de batalla le permitirá operar durante más de 3 años en el frente sin regresar a la capital.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Protege los tanques de combustible colocándolos detrás de varias capas de blindaje de Neutronium para evitar detonaciones catastróficas."
        )

    # 4. HABITATION & CREW QUARTERS
    elif any(x in k_lower for x in ["habitation", "crew", "quarters", "dormitorio", "vida", "alojamiento"]):
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es el módulo de habitabilidad y alojamiento militar para oficiales y marineros a bordo.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Capacidad de Tripulación: Proporciona dormitorios y soporte vital para el personal requerido por motores, armas y sensores.\n"
            f"• Moral y Eficiencia: Mantener los alojamientos al 100% de la tripulación requerida evita caídas de moral y fallos de puntería.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Un acorazado con 200 tripulantes requerirá 4 módulos de Crew Quarters para garantizar la salud y operatividad de la tripulación en travesías largas.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Utiliza el balanceador automático de habitabilidad en el Diseñador para no dejar nunca a la tripulación sin soporte de vida."
        )

    # 5. SENSORS (Active, Passive, Augur, Radar)
    elif any(x in k_lower for x in ["sensor", "augur", "radar", "active", "passive", "thermal", "em", "escaneo", "array"]):
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es una matriz de escaneo pasivo o activo electromagnético/térmico de exploración espacial.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Detección y Cobertura: Los sensores activos detectan naves a millones de km y guían los controles de tiro; los pasivos detectan el calor de los motores sin delatar la posición.\n"
            f"• Resolución Táctica: La resolución determina la masa mínima del objetivo detectable (Res 1 para misiles, Res 20 para cazas, Res 100 para naves capitales).\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Activar este sensor en tu nave de vanguardia revelará la aproximación de la flota enemiga a más de 50 millones de km de distancia.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Mantén los radares activos apagados durante la aproximación y usa sensores térmicos pasivos para ejecutar ataques sorpresa."
        )

    # 6. BEAM WEAPONS & KINETICS
    elif any(x in k_lower for x in ["laser", "beam", "weapon", "cannon", "gauss", "railgun", "meson", "carronade", "microwave", "arma"]):
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es un sistema de armamento naval de energía directa o proyectiles cinéticos de alta cadencia.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Perforación de Blindaje: Inflige daño calórico o cinético sobre el casco enemigo. Los cañones Gauss/Railguns interceptan misiles, mientras que lásers y mesones destruyen la armadura.\n"
            f"• Tasa de Recarga: Requiere potencia continua producida por capacitores y reactores energéticos en cada turno de combate.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Una salva de 4 de estos cañones atravesará el blindaje de un crucero enemigo y destruirá sus controles de tiro en el segundo turno.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Monta armas de energía en torretas orientables para maximizar la velocidad de seguimiento contra misiles hiper-veloces."
        )

    # 7. MISSILES & MAGAZINES
    elif any(x in k_lower for x in ["missile", "magazine", "launcher", "pañol", "ordenanza", "lanzador", "box launcher"]):
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es un pañol blindado de munición balística o tubo de lanzamiento de ordenanza espacial.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Capacidad de Munición: Almacena salvas de misiles que alimentan automáticamente los tubos lanzadores durante el combate.\n"
            f"• Disparo de Asedio: Permite ejecutar ataques de ultra-largo alcance contra flotas enemigas antes de que entren en rango de armas de energía.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Una fragata equipada con este pañol podrá ejecutar 10 salvas masivas de misiles de asedio antes de necesitar regresar a la base para rearmar.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Equipa los pañoles con blindaje de munición para evitar explosiones internas en cadena tras recibir disparos penetrantes."
        )

    # 8. SHIELDS & DEFLECTORS
    elif any(x in k_lower for x in ["shield", "escudo", "deflector"]):
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es un generador de campo de fuerza deflector para la absorción de impactos defensivos.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Absorción de Energía: Absorbe proyectiles cinéticos, lásers y misiles sin degradar la armadura metálica del casco.\n"
            f"• Recarga Automática: Se recarga continuamente consumiendo unidades de energía de los reactores de la nave.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Un crucero equipado con este generador soportará la primera salva de misiles enemiga sin sufrir rasguños en el casco.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Mantén los reactores energéticos activados para asegurar la tasa de regeneración continua de los escudos en combate."
        )

    # 9. JUMP DRIVES
    elif any(x in k_lower for x in ["jump", "salto"]):
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es un motor de salto hiper-espacial para la apertura de túneles gravitatorios en Puntos de Salto.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Transición Gravitacional: Permite que la nave y su escuadra transiten a través de Puntos de Salto hacia nuevos sistemas estelares.\n"
            f"• Capacidad Máxima (JumpMaxHS): La capacidad del motor debe igualar o superar el tonelaje total de la nave para evitar fallos de salto.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Una nave insignia equipada con este motor de salto podrá guiarse a sí misma y a 4 destructores de combate hacia un sistema inexplorado.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Diseña buques especializados 'Jump Escort' para no encarecer el costo de construcción de todas las naves de la flota."
        )

    # 10. CARGO, MINING, LOGISTICS & HARVESTERS
    elif any(x in k_lower for x in ["cargo", "mining", "harvester", "terraforming", "salvage", "tractor", "passenger", "troop", "cryogenic"]):
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es un módulo logístico o industrial especializado para operaciones comerciales y extractivas.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Operatividad Especializada: Permite cosechar Sorium en gigantes gaseosos, extraer minerales en asteroides, mover instalaciones o terraformar mundos.\n"
            f"• Capacidad Comercial: Diseñado para integrarse en buques auxiliares y flotas comerciales con bajo costo de mantenimiento.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Una nave cosechadora equipada con este módulo producirá cientos de miles de litros de combustible hidrocarburo al año sobre Júpiter.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Asigna estas naves a flotas comerciales automáticas para sostener el flujo de recursos e infraestructura en el Imperio."
        )

    # 11. FLEET & NAVAL ESCORTS
    elif any(x in k_lower for x in ["fleet", "flota", "shipyard", "battle", "survey", "colony", "task force"]):
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es una agrupación táctica u operacional de buques espaciales coordinados bajo la cadena de mando imperial.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Organización Táctica: Permite asignar órdenes grupales de movimiento, patrulla, combate y reparación en espacio profundo.\n"
            f"• Liderazgo de Oficial: Transmite las bonificaciones de mando del Comandante de Flota a todas las naves de la escuadra.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Agrupar tus buques en {k} optimiza la coordinación defensiva de punto y sincroniza las salvas de misiles.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Asigna oficiales de alto rango para maximizar el rendimiento operativo de toda la formación."
        )

    # 12. GENERAL HIGH QUALITY COMPONENT / TERM FALLBACK
    else:
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es un componente y especificación técnica de alta precisión integrada en la arquitectura naval e industrial de Aurora 4X.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Especificación Técnica: Diseñado para integrarse en el Diseñador de Naves (Class Design) o en la gestión de infraestructura colonial.\n"
            f"• Rendimiento Operativo: Optimiza el uso de masa (HS), costo de construcción (BP), consumo energético o capacidad táctica de la nave.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Incorporar {k} en tu plano de diseño mejorará el equilibrio entre peso, habitabilidad y capacidad de combate del buque.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Verifica que las necesidades de energía, tripulación y mantenimiento de {k} estén cubiertas en la telemetría de diseño."
        )

for path in json_paths:
    if os.path.exists(path):
        with open(path, "r", encoding="utf-8") as f:
            data = json.load(f)

        enriched_count = 0
        for k in list(data.keys()):
            v = data[k]
            # Replace any generic placeholders ("es una especificación y componente fundamental dentro de la arquitectura industrial")
            if "es una especificación y componente fundamental dentro de la arquitectura industrial" in v or len(v) < 150:
                data[k] = generate_flawless_component_tooltip(k)
                enriched_count += 1

        with open(path, "w", encoding="utf-8") as f:
            json.dump(data, f, ensure_ascii=False, indent=2)

        print(f"File {path}: Re-enriched {enriched_count} generic placeholders. Total items: {len(data)}")
