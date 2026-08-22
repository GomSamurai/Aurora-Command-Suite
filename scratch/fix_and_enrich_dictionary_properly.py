import json
import os
import re

json_paths = [
    r"c:\VSCODE\AuroraDesignSuite\config\AuroraTooltipDictionary.json",
    r"c:\VSCODE\Aurora_Command_Suite_v2.7.1_Portable\App\config\AuroraTooltipDictionary.json",
    r"c:\VSCODE\Aurora271Full\Patches\AuroraSpanish\AuroraTooltipDictionary.json"
]

def generate_perfect_tooltip(key):
    k = key.strip()
    k_lower = k.lower()

    # -------------------------------------------------------------------------
    # 1. FLEETS & FLEET NAMES (Shipyard Fleet, Battle Fleet, Cargo Fleet, etc.)
    # -------------------------------------------------------------------------
    if "shipyard fleet" in k_lower:
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"Shipyard Fleet es una escuadra logística de apoyo naval compuesta por buques de mantenimiento, gradas móviles, naves remolcadoras y talleres orbitales.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Reparación en Espacio Profundo: Permite reparar cascos de naves dañadas en batallas lejanas y reequipar componentes sin obligar a la flota a regresar a la Tierra.\n"
            f"• Apoyo a Astilleros: Transporta repuestos MSP (Maintenance Supplies) y módulos de astillero para acelerar la expansión de bases navales exteriores.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Tras una batalla decisiva en Alfa Centauri donde 3 cruceros sufrieron brechas en el blindaje, desplegar la Shipyard Fleet en el sistema permitirá reparar los cascos en órbita en lugar de arriesgarlos en una larga travesía de vuelta a la capital.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Equipa las naves de tu Shipyard Fleet con grandes pañoles de repuestos MSP y módulos de grúa/remolcador para sostener la capacidad operativa de la armada en el frente."
        )

    elif "battle fleet" in k_lower:
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"Battle Fleet es la escuadra militar principal de combate naval del Imperio, compuesta por acorazados, cruceros, destructores y cazas de asalto.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Superioridad Espacial: Agrupa la potencia de fuego táctica (misiles, láseres, mesones) para destruir flotas enemigas y proteger las fronteras imperiales.\n"
            f"• Defensa del Sistema: Mantiene patrullas continuas en Puntos de Salto y nodos estelares estratégicos.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Desplegar la Battle Fleet en un Punto de Salto hostil permite interceptar cualquier nave o salva de misiles alienígena nada más realizar la transición al sistema.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Asigna un Comandante de Flota de alto rango con bonificación en velocidad de maniobra y puntería para aumentar el rendimiento en combate de toda la escuadra."
        )

    elif "cargo fleet" in k_lower:
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"Cargo Fleet es el convoy logístico de transporte pesado comercial del Imperio, compuesto por barcos cargueros (Freighters) de gran tonelaje.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Transporte de Instalaciones: Mueve Fábricas de Construcción, Minas Automatizadas, Refinerías y Infraestructura entre la Tierra y las colonias exteriores.\n"
            f"• Redistribución Mineral: Transporta grandes volúmenes de minerales exóticos desde los mundos extractivos hacia las capitales industriales.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Asignar la Cargo Fleet a trasladar 50 Minas Automatizadas desde la Tierra a Ceres permitirá poner en marcha la extracción mineral en el asteroide en un solo viaje.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Construye Puertos Espaciales de Carga en las colonias principales para reducir el tiempo de carga y descarga de la Cargo Fleet hasta un 80%."
        )

    elif "colony fleet" in k_lower:
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"Colony Fleet es la escuadra de transporte de población y habitabilidad dedicada a la expansión colonial y asentamiento en nuevos mundos.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Traslado de Colonos: Transporta millones de ciudadanos civiles desde la Tierra hacia colonias en desarrollo para proporcionar mano de obra industrial.\n"
            f"• Habitabilidad Colonial: Despliega domos de Infraestructura en mundos hostiles antes del desembarco masivo de población.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Transportar 5 millones de habitantes con la Colony Fleet hacia Marte pondrá en marcha la economía del planeta y permitirá operar nuevas fábricas locales.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Asegúrate de que la colonia de destino disponga de suficiente Infraestructura o de un Costo Colonial de 0.00 antes de desembarcar a la población."
        )

    elif "survey fleet" in k_lower:
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"Survey Fleet es la escuadra de prospección y reconocimiento exploratorio equipada con sensores gravitatorios y geológicos de vanguardia.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Exploración Gravitacional: Escanea los sistemas estelares para descubrir Puntos de Salto ocultos que conducen a nuevos sectores de la galaxia.\n"
            f"• Prospección Geológica: Analiza la composición mineral de planetas, lunas y asteroides para localizar vetas de Duranium, Sorium y Gallicite.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Enviar la Survey Fleet al sistema Alfa Centauri cartografiará todos sus asteroides y revelará si existen ruinas alienígenas o yacimientos de alta accesibilidad.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Diseña buques de prospección ligeros con alta velocidad y tanques de combustible extendidos para que puedan operar durante años sin repostar."
        )

    elif "fleet" in k_lower or "escuadra" in k_lower or "task force" in k_lower:
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es una agrupación táctica u operacional de buques espaciales coordinados bajo una misma estructura de mando imperial.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Organización Táctica: Permite asignar órdenes grupales de movimiento, patrulla, repostaje o combate a múltiples naves de forma simultánea.\n"
            f"• Liderazgo de Oficial: Asigna las bonificaciones de mando del Comandante de Flota a todas las unidades de la formación.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Agrupar 4 destructores y 1 crucero insignia en una misma Flota permite que todas las naves salten juntas y disparen sus salvas de misiles en el mismo segundo.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Mantén separadas las flotas militares de las flotas comerciales para evitar que cargueros lentos reduzcan la velocidad de tus unidades de combate."
        )

    # -------------------------------------------------------------------------
    # 2. MINERALS (Duranium, Sorium, Neutronium, Gallicite, etc.)
    # -------------------------------------------------------------------------
    elif k_lower in ["duranium", "sorium", "neutronium", "corundium", "uridium", "gallicite", "boronide", "mercassium", "vendarite", "tritium", "tritanium"]:
        mineral_usages = {
            "duranium": "El mineral estructural primario. Se consume en CADA edificio planetario, nave espacial, misil y fortificación del Imperio.",
            "sorium": "El mineral energético exclusivo procesado en Refinerías para sintetizar el combustible de hidrocarburo espacial LPH.",
            "neutronium": "El material de ultra-densidad para forjar capas de blindaje metálico pesado en cascos navales y fuertes militares.",
            "gallicite": "El mineral superconductor ultraligero crítico para fabricar motores de alta velocidad y la propulsión de misiles.",
            "corundium": "El cristal hiper-duro utilizado en la fabricación de maquinaria pesada de minería automatizada y refinerías.",
            "uridium": "El cristal óptico y electromagnético indispensable para fabricar sensores térmicos/EM, radares activos y controles de tiro.",
            "boronide": "El material refractario indispensable para fabricar generadores de escudos de fuerza y reactores de potencia.",
            "mercassium": "El elemento superconductor de ambiente utilizado en la construcción de módulos de vida, habitabilidad y naves comerciales.",
            "vendarite": "El mineral maleable esencial en la construcción de Fábricas de Construcción, Centros Financieros y edificaciones planetarias.",
            "tritium": "El componente primordial para la fabricación de cabezas de guerra de misiles y cargas explosivas navales.",
            "tritanium": "La aleación ligera empleada en la estructura de tubos lanzamisiles, cañones cinéticos y armazones de motor."
        }
        usage = mineral_usages.get(k_lower, "Elemento trans-newtoniano exótico de alto valor industrial.")
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es uno de los 11 minerales exóticos trans-newtonianos fundamentales que sustentan la economía y tecnología espacial en Aurora 4X.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Aplicación Industrial / Naval: {usage}\n"
            f"• Factor de Accesibilidad: La velocidad de extracción anual depende de la accesibilidad del yacimiento en el planeta o asteroide (0.1x a 1.0x).\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Si tus reservas de {k} caen a 0 en la capital, las colas de construcción de la industria planetaria o de los astilleros navales correspondientes se paralizarán de inmediato.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Establece colonias mineras automatizadas con Catapultas de Masa en asteroides que contengan yacimientos de {k} con accesibilidad 1.00 para mantener un flujo continuo."
        )

    # -------------------------------------------------------------------------
    # 3. PLANETARY INSTALLATIONS (Construction Factory, Mine, Spaceport, etc.)
    # -------------------------------------------------------------------------
    elif any(inst in k_lower for inst in ["construction factory", "fábrica de construcción", "automated mine", "conventional mine", "fuel refinery", "research facility", "financial centre", "spaceport", "military academy", "deep space tracking", "mass driver", "terraforming", "infrastructure", "maintenance facility"]):
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es una infraestructura industrial o militar planetaria clave para el desarrollo económico y logístico del Imperio.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Capacidad Operativa: Genera puntos de producción, extracción mineral, refinado de combustible o servicios de apoyo colonial por año.\n"
            f"• Requerimiento Poblacional: Funciona con trabajadores civiles asignados en la colonia (a excepción de las Minas Automatizadas que no requieren población).\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Construir y mantener un volumen elevado de {k} en tus mundos principales acelerará la capacidad de expansión y el sostenimiento naval del Imperio.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Asigna gobernadores planetarios con bonificaciones industriales o de construcción para multiplicar la eficiencia operativa de {k}."
        )

    # -------------------------------------------------------------------------
    # 4. COMMANDERS & OFFICERS
    # -------------------------------------------------------------------------
    elif any(cmd in k_lower for cmd in ["commander", "oficial", "gobernador", "científico", "seniority", "rank", "almirante", "capitán"]):
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} representa a los oficiales, líderes militares, gobernadores coloniales y científicos de la jerarquía imperial.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Bonificaciones Directas: Aportan multiplicadores de rendimiento en investigación científica, producción industrial, velocidad naval o efectividad en combate.\n"
            f"• Experiencia y Rango: Aumentan su nivel de habilidad (Seniority & Rating) mediante el servicio activo y la dirección de misiones.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Asignar un Comandante con bonificación en 'Mining' a una colonia minera aumentará la velocidad de extracción de minerales de todas las minas planetarias.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Construye múltiples Academias Militares en la Tierra para graduar a un flujo constante de oficiales con mejores atributos."
        )

    # -------------------------------------------------------------------------
    # 5. ASTROPHYSICS & COLONIZATION (System Body, Star System, Colony Cost, Population)
    # -------------------------------------------------------------------------
    elif any(astro in k_lower for astro in ["star system", "system body", "colony cost", "population", "wealth", "gravity", "atmosphere", "hydro", "albedo"]):
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es un parámetro fundamental de la física planetaria, astrofísica o gobernanza colonial en Aurora 4X.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Habitabilidad e Industria: Define las condiciones ambientales (temperatura, oxígeno, gravedad) que determinan la viabilidad de la vida y el costo colonial.\n"
            f"• Crecimiento Imperial: Influye en la generación de riqueza fiscal (Wealth), capacidad de trabajadores y expansión de asentamientos.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Reducir el Colony Cost de un planeta a 0.00 mediante terraformación elimina la necesidad de gastar industria en domos de Infraestructura.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Prioriza la colonización de mundos con Colony Cost bajo para maximizar el retorno industrial en el menor tiempo posible."
        )

    # -------------------------------------------------------------------------
    # 6. RESEARCH & TECHNOLOGIES (TURRET, MESON, JUMP, WARHEAD, SENSORS, ETC.)
    # -------------------------------------------------------------------------
    elif "turret" in k_lower or "tracking speed" in k_lower:
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es la tecnología de ingeniería mecánica que regula la velocidad de giro y rotación de las torretas de armamento de energía (Láseres, Cañones Gauss, Railguns).\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Velocidad de Seguimiento (Tracking Speed): Permite que las armas de energía sigan a objetivos ultra-rápidos (misiles enemigos a 15,000+ km/s o cazas estelares).\n"
            f"• Probabilidad de Impacto: Si la velocidad del blanco supera la velocidad de seguimiento del arma, la precisión disminuye proporcionalmente (Hit% = Tracking / Target Speed).\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Un misil enemigo viaja a 20,000 km/s. Si tu cañón Gauss montado en casco solo sigue a 4,000 km/s, la probabilidad de impacto es de solo el 20%. Al montar el cañón en una torreta con {k}, la velocidad de rotación aumentará permitiendo un 100% de precisión en la intercepción.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Para naves de Defensa de Punto (Point Defense / PDC), diseña siempre torretas cuya velocidad de seguimiento iguale o supere la velocidad máxima de misiles esperada."
        )

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

    else:
        return (
            f"📌 CONCEPTO & DEFINICIÓN:\n"
            f"{k} es una especificación y componente fundamental dentro de la arquitectura industrial, naval y didáctica de Aurora 4X.\n\n"
            f"⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
            f"• Rendimiento Operativo: Potencia las capacidades tácticas de tus buques, la eficiencia de tus colonias o la velocidad de prospección galáctica.\n"
            f"• Especificación Técnica: Diseñado para integrarse en el Diseñador de Naves (Class Design) o en la gestión de infraestructura colonial del Imperio.\n\n"
            f"💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
            f"Incorporar {k} en tu doctrina de flota o en la gestión de mundos exteriores optimizará el uso de recursos y aumentará la supervivencia en combate.\n\n"
            f"🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
            f"Consulta el dossier completo y mantén equilibrados tus suministros minerales y energéticos para sacar el máximo partido a {k}."
        )

# Read, enrich and overwrite all dictionary files cleanly
for path in json_paths:
    if os.path.exists(path):
        with open(path, "r", encoding="utf-8") as f:
            data = json.load(f)

        enriched_count = 0
        for k in list(data.keys()):
            new_v = generate_perfect_tooltip(k)
            data[k] = new_v
            enriched_count += 1

        with open(path, "w", encoding="utf-8") as f:
            json.dump(data, f, ensure_ascii=False, indent=2)

        print(f"Cleanly re-enriched {path}: {enriched_count} items updated. Total dictionary entries: {len(data)}")
