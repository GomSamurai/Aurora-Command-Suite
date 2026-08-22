import json
import os
import re

json_paths = [
    r"c:\VSCODE\AuroraDesignSuite\config\AuroraTooltipDictionary.json",
    r"c:\VSCODE\Aurora_Command_Suite_v2.7.1_Portable\App\config\AuroraTooltipDictionary.json",
    r"c:\VSCODE\Aurora271Full\Patches\AuroraSpanish\AuroraTooltipDictionary.json"
]

def generate_rich_tooltip(key, value):
    k = key.strip()
    k_lower = k.lower()

    # If it's already a high quality hand-crafted tooltip (long and contains rich non-generic advice), keep or enhance it!
    # But if it contains generic phrases like "es un avance científico en la especialidad" or "Desbloquea componentes navales, armas, sensores o eficiencias", REWRITE IT COMPLETELY!

    is_generic = ("es un avance científico" in value or 
                  "Desbloquea componentes navales, armas, sensores o eficiencias" in value or
                  "Tecnología / Elemento Imperial" in value or
                  len(value) < 120)

    if not is_generic and "📌 CONCEPTO & DEFINICIÓN:" in value and not "avance científico" in value:
        # Already high quality hand-crafted entry!
        return value

    # -------------------------------------------------------------------------
    # MASTER PATTERN MATCHING & HIGH-QUALITY TEXT GENERATION
    # -------------------------------------------------------------------------

    # 1. TURRET TRACKING & ROTATION GEAR
    if "turret" in k_lower or "tracking speed" in k_lower:
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es la tecnología de ingeniería mecánica que regula la velocidad de giro y rotación de las torretas de armamento de energía (Láseres, Cañones Gauss, Railguns).\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Velocidad de Seguimiento (Tracking Speed): Permite que las armas de energía sigan a objetivos ultra-rápidos (misiles enemigos a 15,000+ km/s o cazas estelares).\n"
            f"• Probabilidad de Impacto: Si la velocidad del blanco supera la velocidad de seguimiento del arma, la precisión disminuye proporcionalmente (Hit% = Tracking / Target Speed).\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Un misil enemigo viaja a 20,000 km/s. Si tu cañón Gauss montado en casco solo sigue a 4,000 km/s, la probabilidad de impacto es de solo el 20%. Al montar el cañón en una torreta con {k}, la velocidad de rotación aumentará permitiendo un 100% de precisión en la intercepción.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Para naves de Defensa de Punto (Point Defense / PDC), diseña siempre torretas cuya velocidad de seguimiento iguale o supere la velocidad máxima de misiles de las facciones enemigas esperadas."
        )

    # 2. MESON FOCUSING & CANNONS
    elif "meson" in k_lower:
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es el desarrollo de tecnología de partículas mesónicas y focalización de haces de energía cuántica.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Ignora Escudos y Blindaje: Los Cañones Mesónicos son la única arma de energía que atraviesa los escudos energéticos y el blindaje metálico del casco enemigo sin atenuación.\n"
            f"• Daño Interno Directo: Cada impacto certero inflige exactamente 1 punto de daño interno directo sobre puentes de mando, motores, reactores o depósitos de misiles del blanco.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Un acorazado alienígena provisto de 15 capas de blindaje de Neutronium resistirá docenas de impactos de láser. Sin embargo, una salva de Cañones Mesónicos con {k} atravesará ese blindaje e inutilizará sus controles de tiro internos en el primer turno.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Instala armas mesónicas en naves rápidas de interdicción o en baterías de defensa planetaria para incapacitar buques capitales enemigos sin necesidad de destruir su estructura exterior."
        )

    # 3. JUMP DRIVES & SQUADRON EFFICIENCY
    elif "jump drive" in k_lower or "jump efficiency" in k_lower or "jump squadron" in k_lower or "squad jump" in k_lower:
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es la tecnología de propulsión hiper-espacial y focalización de túneles de salto gravitatorios (Jump Drives).\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Eficiencia de Salto (Jump Efficiency): Reduce el tonelaje (HS) y el consumo de minerales exóticos necesarios para construir el motor de salto en el Diseñador de Naves (Class Design).\n"
            f"• Tamaño de Escuadra (Squadron Size): Determina cuántas naves acompañantes sin motor de salto pueden transitar simultáneamente a través del túnel gravitacional generado por la nave nodriza.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Investigar {k} permite construir naves 'Jump Escort' más pequeñas y económicas, capaces de llevar hasta 5 o 10 destructores de combate a través de Puntos de Salto no explorados.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"No es necesario equipar cada nave militar con motor de salto. Diseña una nave insignia especializada en salto que guíe al resto de la escuadra militar a través de las fronteras estelares."
        )

    # 4. COMBAT INFORMATION CENTRE & COMMAND
    elif "combat information" in k_lower or "command and control" in k_lower or "flag bridge" in k_lower or "flight control" in k_lower:
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} abarca la arquitectura de sistemas de mando táctico, procesamiento de datos de flota y centros de información de combate (CIC).\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Coordinación de Flota: Proporciona la infraestructura indispensable para que los Comandantes de Flota y Almirantes ejerzan el mando táctico sobre escuadras múltiples.\n"
            f"• Bonificaciones de Mando: Transmite un porcentaje directo de las bonificaciones del oficial (velocidad de maniobra, puntería, moral) a todas las naves bajo su mando.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Un buque de guerra equipado con {k} y comandado por un Almirante otorgará un +15% de precisión de tiro a todos los destructores y fragatas de la formación.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Protege siempre la nave insignia equipada con {k} situándola en el centro de la formación naval y rodeándola de escoltas de defensa de punto."
        )

    # 5. SALVAGE MODULE & RECLAMATION
    elif "salvage" in k_lower or "recuperación" in k_lower:
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es el sistema industrial de despiece, corte por plasma y recuperación de pecios espaciales destruidos en combate.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Extracción de Pecios: Permite desmantelar los restos de naves enemigas o abandonadas en órbita, recuperando toneladas de minerales exóticos puros (Duranium, Gallicite, Sorium).\n"
            f"• Recuperación Tecnológica: Otorga la oportunidad de recuperar componentes mecánicos intactos e investigaciones avanzadas directamente del enemigo.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Tras ganar una batalla espacial, enviar una nave chatarrera equipada con {k} recuperará decenas de miles de toneladas de Duranium y Gallicite gratis en cuestión de días.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Construye buques comerciales chatarreros con módulos de recuperación y grandes bodegas para acompañar a las flotas de combate y cosechar los frutos de la victoria."
        )

    # 6. ELECTRONIC HARDENING & WARFARE (ECCM, ECM, HARDENING)
    elif "hardening" in k_lower or "eccm" in k_lower or "ecm" in k_lower:
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es la tecnología de blindaje electrónico, endurecimiento de circuitos electromagnéticos y guerra electrónica espacial.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Protege contra Pulsos EMP: {k} evita que los cañones de microondas (HPM) o detonaciones nucleares enemigas dejen fuera de servicio la electrónica de la nave.\n"
            f"• Guerra Electrónica: Neutraliza las interferencias de inhibidores (ECM/ECCM), garantizando que los controles de tiro fijen blanco sin distorsión.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Una nave enemiga con jammer ECM reducirá la precisión de tus misiles en un 30%. Equipar {k} en tu control de tiro anula la interferencia enemiga restaurando el 100% de puntería.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Instala siempre módulos de endurecimiento y ECCM en los controles de tiro de tus naves capitales y cazas interceptores."
        )

    # 7. CAPACITOR RECHARGE & POWER PLANTS
    elif "capacitor" in k_lower or "power plant" in k_lower or "reactor" in k_lower or "stellarator" in k_lower or "tokamak" in k_lower:
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es el avance en tecnología de reactores energéticos (Fusión/Fisión) y sistemas de almacenamiento y recarga de capacitores de potencia.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Tasa de Recarga (Capacitor Recharge Rate): Determina cuántos segundos tarda un arma de energía directa (Láser, Mesón, Partículas) en acumular potencia para volver a disparar.\n"
            f"• Generación de Unidades de Energía (EU): Los reactores producen la potencia continua consumida por armas y escudos en cada turno de combate.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Con recarga de capacitores nivel 1, un Láser de 10 EU tardará 50 segundos (10 turnos) en disparar. Con {k}, el tiempo de recarga caerá a 10 segundos (2 turnos), quintuplicando la cadencia de fuego.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Asegúrate de que la producción total de EU de los reactores instalados en la nave sea igual o superior al consumo por turno de todas las armas y escudos activos."
        )

    # 8. FUEL CONSUMPTION & ENGINES
    elif "fuel consumption" in k_lower or "engine power" in k_lower or "drive" in k_lower or "engine size" in k_lower or "thermal sensor" in k_lower or "em sensor" in k_lower:
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es el desarrollo en física de motores de propulsión espacial y sensores pasivos térmicos/electromagnéticos.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Eficiencia de Combustible (Fuel Consumption Rate): Reduce los litros de hidrocarburo Sorium LPH consumidos por hora de navegación a máxima velocidad.\n"
            f"• Rendimiento y Autonomía: Incrementa la velocidad máxima (km/s) y duplica el rango operativo en kilómetros de tus naves espaciales.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Investigar {k} reduce el consumo de combustible de tus cargueros de 0.8 a 0.4 Litros/EP/hora, permitiéndoles cruzar múltiples sistemas estelares sin necesidad de buques tanque de apoyo.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Para naves comerciales (cargueros, mineros, colonizadores), combina esta tecnología con multiplicadores de potencia reducidos para lograr autonomías de miles de millones de kilómetros."
        )

    # 9. MISSILE LAUNCHERS & WARHEADS
    elif "missile" in k_lower or "warhead" in k_lower or "terminal guidance" in k_lower or "retargeting" in k_lower:
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} abarca el diseño balístico de ojivas explosivas, lanzadores de misiles y cabezas de guiado terminal autónomo.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Penetración de Ojiva (Warhead Yield): El daño infligido al blindaje enemigo es igual a Yield, perforando a una profundidad de Raíz de Yield capas.\n"
            f"• Guiado Terminal Activo: Permite que el misil busque un objetivo secundario cercano si el blanco principal es destruido antes del impacto.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Una salva de misiles equipada con {k} mantendrá su curso y re-orientará su sistema de guiado hacia la siguiente nave hostil de la formación si la primera nave es destruida.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Equilibra la velocidad del misil y el rendimiento de la cabeza de guerra para asegurar que los misiles alcancen a los blancos enemigos más veloces con el máximo daño posible."
        )

    # 10. SHIELD GENERATORS & DENSITY
    elif "shield" in k_lower:
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es el desarrollo de campos de fuerza deflectores de energía y generadores de escudo protector.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Absorción de Impactos: Los escudos absorben proyectiles cinéticos, lásers y misiles antes de que afecten a la armadura metálica exterior.\n"
            f"• Regeneración Automática: Se recargan de forma continua durante el combate consumiendo unidades de energía de la nave.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Investigar {k} permite instalar escudos de mayor densidad en tus cruceros, permitiéndoles absorber impactos continuos en batallas de desgaste sin sufrir daños en el casco.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Mantén siempre reactores de potencia activos para asegurar que la tasa de regeneración de escudos opere al 100% de su capacidad."
        )

    # 11. CARGO & HABITABILITY (Cargo Hold, Crew Quarters, Habitation)
    elif "cargo" in k_lower or "quarters" in k_lower or "habitat" in k_lower or "shuttle" in k_lower:
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es la tecnología de diseño de bodegas de carga comercial, alojamientos de tripulación y módulos de transporte de pasaje.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Logística Comercial: Permite construir barcos cargueros que mueven instalaciones planetarias (Fábricas, Minas, Astilleros) entre colonias.\n"
            f"• Habitabilidad Naval: Garantiza que la tripulación mantenga la moral y el rendimiento durante travesías de larga duración en el espacio profundo.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Equipar bodegas de carga con {k} en tus buques logísticos reducirá el tamaño del casco manteniendo la máxima capacidad de transporte de instalaciones.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Para buques militares, asegúrate de mantener el indicador de 'Life Support' por encima del tiempo estimado de misión para no sufrir motines por fatiga."
        )

    # 12. GENERAL TECH FALLBACK
    else:
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es una innovación clave dentro del catálogo oficial de investigación, ingeniería e industria de Aurora 4X.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Desbloqueo Industrial / Naval: Al completar este proyecto en la pantalla de I+D (ResearchHQ), desbloquearás componentes optimizados para el Diseñador de Naves (Class Design) o mejoras en la eficiencia colonial.\n"
            f"• Especificación Táctica: Incrementa el rendimiento operativo, reduce costos de producción en minerales exóticos o mejora la efectividad en combate.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Investigar {k} permite reemplazar componentes obsoletos en tus diseños de clase, reduciendo toneladas de desplazamiento o aumentando la potencia de tus instalaciones.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Asigna esta investigación a un científico con especialidad coincidente para obtener una bonificación directa de velocidad de desarrollo de hasta el 50%."
        )

# Read, enrich and overwrite all dictionary files
for path in json_paths:
    if os.path.exists(path):
        with open(path, "r", encoding="utf-8") as f:
            data = json.load(f)

        enriched_count = 0
        for k, v in list(data.items()):
            new_v = generate_rich_tooltip(k, v)
            if new_v != v:
                data[k] = new_v
                enriched_count += 1

        with open(path, "w", encoding="utf-8") as f:
            json.dump(data, f, ensure_ascii=False, indent=2)

        print(f"File {path}: Enriched {enriched_count} items. Total items: {len(data)}")
