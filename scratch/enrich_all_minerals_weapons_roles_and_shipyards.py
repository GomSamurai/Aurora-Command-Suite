import json
import os

existing_json_path = 'c:/VSCODE/AuroraDesignSuite/config/AuroraTooltipDictionary.json'

existing_dict = {}
if os.path.exists(existing_json_path):
    try:
        existing_dict = json.load(open(existing_json_path, 'r', encoding='utf-8'))
    except Exception as e:
        print("Could not load existing dictionary:", e)

# -----------------------------------------------------------------------------
# DEEP 4-SECTION ARTICLES FOR ALL MINERALS, WEAPONS, ROLES, AND SHIPYARDS
# -----------------------------------------------------------------------------
deep_master_additions = {
    # --- 11 TRANS-NEWTONIAN MINERALS ---
    "Duranium": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Duranium es el elemento trans-newtoniano estructural primario en Aurora 4X. Es el equivalente espacial del acero de alta densidad.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Requisito Universal: Requerido en prácticamente todas las estructuras planetarias, blindajes metálicos de naves y componentes navales.\n"
        "• Consumo Masivo: Es el mineral de mayor consumo en el Imperio (representa habitualmente más del 40% del tonelaje consumido).\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Si te quedas sin Duranium, la producción de tus Fábricas de Construcción y Astilleros Navales se detendrá por completo al instante.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Establece colonias mineras automatizadas en todos los asteroides y cometas que tengan yacimientos de Duranium con accesibilidad 1.00x."
    ),

    "Sorium": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Sorium es el cristal energético trans-newtoniano utilizado para sintetizar combustible hidrocarburo espacial LPH y alimentar reactores de fusión.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Refinado de Combustible: Procesado en Refinerías para generar millones de litros de combustible hidrocarburo.\n"
        "• Motores y Reactores: Se utiliza directamente en la fabricación de motores de propulsión espacial y reactores nucleares.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Sin reservas de Sorium refinado, tus flotas navales quedarán a la deriva en el espacio profundo sin poder maniobrar ni usar salvas.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Construye estaciones orbitales cosechadoras de Sorium sobre Gigantes Gaseosos para obtener combustible ilimitado."
    ),

    "Neutronium": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Neutronium es un mineral súper denso de materia degenerada utilizado para forjar blindajes de cascos pesados y astilleros navales.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Blindaje Naval (Armour): Es el componente principal para fabricar capas protectoras de blindaje en acorazados y cruceros.\n"
        "• Astilleros y Estructuras Defensivas: Requerido en la construcción de gradas de astillero y fuertes militares terrestres.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Añadir 5 capas de blindaje a un acorazado consumirá cientos de toneladas de Neutronium en el astillero.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Mantén reservas estratégicas de Neutronium para no paralizar la botadura de naves de guerra tras batallas decisivas."
    ),

    "Corium": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Corium es un elemento piezoeléctrico y superconductor utilizado en la fabricación de sistemas ópticos, láseres y sensores activos.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Óptica y Láseres: Esencial para ensamblar cañones láser, sensores infrarrojos térmicos y radares de búsqueda activa.\n"
        "• Generación Térmica: Se aplica en recubrimientos de disipación de calor para motores militares.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Diseñar un sensor activo Res 100 de gran tamaño requerirá decenas de toneladas de Corium de alta pureza.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Monitorea el consumo de Corium cuando inicies programas masivos de construcción de sensores o naves láser."
    ),

    "Tritanium": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Tritanium es una aleación metálica hiper-resistente empleada en la estructura de tubos lanzamisiles, cañones cinéticos y motores.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Armamento Kinetico y Misiles: Utilizado para fabricar lanzadores de misiles (Missile Launchers) y estructuras de cañones Gauss.\n"
        "• Estructuras de Reacción: Forma el armazón interno de los motores espaciales.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Construir una batería de 20 lanzadores de misiles en un destructor consumirá un volumen significativo de Tritanium.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Garantiza el suministro de Tritanium en tus colonias industriales especializadas en ordenanza y municiones."
    ),

    "Boronide": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Boronide es un cristal refractario y aislante utilizado en generadores de escudos de fuerza y reactores de potencia.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Escudos de Fuerza: Mineral básico indispensable para fabricar generadores de escudo (Shield Generators).\n"
        "• Reactores de Energía: Utilizado en la fabricación de plantas de energía de fusión y capacitores de recarga.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Equipar a tus cruceros con escudos de energía requerirá una provisión constante de Boronide procesado.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Si tu doctrina naval prioriza escudos sobre blindaje metálico, el Boronide se convertirá en un mineral crítico."
    ),

    "Mercassium": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Mercassium es un elemento superconductor de temperatura ambiente utilizado en la industria del transporte comercial y soporte vital.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Motores Comerciales y Vida: Empleado en motores de naves civiles, cargueros, módulos de colonización e Infraestructura.\n"
        "• Desarrollo Urbano: Requerido para fabricar domos de Hábitat Urbano en planetas hostiles.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "La producción masiva de Infraestructura para poblar Marte consume miles de toneladas de Mercassium al año.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Asegura yacimientos de Mercassium antes de iniciar expansiones de colonización a gran escala."
    ),

    "Vendarite": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Vendarite es un mineral trans-newtoniano maleable utilizado en instalaciones industriales terrestres y centros financieros.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Fábricas e Infraestructura: Utilizado en la construcción de Fábricas de Construcción, Centros Financieros y Academias Militares.\n"
        "• Elementos Civiles: Necesario para mantener la expansión industrial planetaria.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Construir 100 nuevas Fábricas de Construcción en la capital imperial consumirá una gran cuota de Vendarite.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Equilibra la extracción de Vendarite con la de Duranium para mantener activa la cola de construcción en la capital."
    ),

    "Uridium": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Uridium es un cristal emisor electromagnético utilizado en sensores pasivos EM, directores de tiro y sistemas de guerra electrónica.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Control de Tiro y Sensores: Elemento clave en Directores de Tiro (Beam/Missile Fire Control) y Sensores EM.\n"
        "• Modulos de Guerra Electrónica: Utilizado en sistemas ECM y ECCM de combate espacial.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un director de tiro de misiles de ultra-largo alcance requiere Uridium de alta pureza para sus lentes de enfoque.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "El Uridium suele ser escaso; protege los asteroides que contengan yacimientos con alta accesibilidad."
    ),

    "Corundium": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Corundium es un cristal sintético hiper-duro utilizado en la fabricación de maquinaria pesada de minería y refinerías.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Minas y Refinerías: Mineral esencial para construir Minas Automatizadas, Minas Convencionales y Refinerías de Combustible.\n"
        "• Maquinaria Industrial: Imprescindible para escalar la capacidad de extracción de recursos del Imperio.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Para fabricar 50 Minas Automatizadas para una colonia exterior, necesitarás varias toneladas de Corundium.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "El Corundium es el 'mineral de la minería'; asegúrate de tener reservas antes de planificar nuevos frentes mineros."
    ),

    "Gallicite": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Gallicite es un elemento superconductor exótico ultraligero indispensable para la fabricación de motores de alta velocidad y misiles.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Motores y Misiles: Requerido en la fabricación de motores espaciales militares y en la ojiva/propulsión de misiles.\n"
        "• Cuello de Botella Militar: Es habitualmente el mineral más escaso y codiciado por las potencias espaciales.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Una salva masiva de 500 misiles anti-buque puede agotar tus reservas planetarias de Gallicite en pocos meses.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "El Gallicite es el recurso militar estratégico N° 1. Prioriza su prospección y extracción por encima de cualquier otro mineral."
    ),

    # --- NAVAL WEAPONS & UTILITY MODULES ---
    "Beam Fire Control": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Director de Tiro para Armas de Energía (Beam Fire Control / BFC) es la computadora táctica de cálculo balístico encargada de fijar y rastrear objetivos para cañones láser, Gauss, Meson y Railguns.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Velocidad de Seguimiento (Tracking Speed): Dicta la velocidad máxima del blanco que el director puede rastrear sin perder precisión de tiro.\n"
        "• Alcance Máximo de Fijación: Determina la distancia máxima a la que tus armas de energía pueden disparar.\n"
        "• Asignación de Armas: Cada BFC puede controlar un número limitado de armas de energía simultáneamente.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un director de tiro con Tracking Speed de 10,000 km/s permitirá a tus cañones Gauss impactar misiles enemigos entrantes a alta velocidad durante la defensa de punto (Point Defense).\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Monta directores de tiro en torretas orientables o con tecnología de alto seguimiento para interceptar salvas de misiles hiperrápidas."
    ),

    "High Power Microwave": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Microondas de Alta Potencia (High Power Microwave / HPM) es un arma de energía no letal que emite un pulso electromagnético concentrado diseñado para freír los circuitos electrónicos del objetivo.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Destrucción de Electrónica: Atraviesa escudos y blindaje para destruir directamente los sensores, radares y directores de tiro enemigos.\n"
        "• Sin Daño Estructural: No destruye el casco físico de la nave enemiga, permitiendo dejarla 'ciega e inhabilitada' para su posterior abordaje.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un disparo acertado de HPM dejará a un acorazado enemigo sin radares ni control de tiro, dejándolo completamente indefenso sin destruir la nave.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Utiliza buques rápidos equipados con HPM para inhabilitar naves alienígenas de alto valor y capturarlas mediante abordaje con Infantería de Marina."
    ),

    "Decoy Launcher": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Lanzador de Señuelos (Decoy Launcher) es un sistema defensivo que despliega boyas de despiste electromagnético y térmico para desviar misiles entrantes.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Intercepción Pasiva: Engaña los sensores de guiado de los misiles enemigos, reduciendo su probabilidad de impacto contra la nave.\n"
        "• Consumo de Señuelos: Requiere pañoles de señuelos a bordo para recargar tras cada salvas defensivas.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Al ser atacado por una salva de misiles anti-buque, lanzar señuelos desviará el 30% de los misiles entrantes hacia espacio vacío.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Combina lanzadores de señuelos con escudos de fuerza y CIWS para crear un sistema defensivo multicapa impenetrable."
    ),

    "Gauss Cannon": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Cañón Gauss (Gauss Cannon) es un cañón de aceleración magnética de tiro rápido diseñado principalmente como arma de Defensa de Punto (Point Defense).\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Múltiples Disparos por Turno: Dispara una ráfaga de proyectiles cinéticos (ej. de 1 a 8 disparos por turno según la tecnología investigada).\n"
        "• Excelente Defensa Antimisil: Es la mejor arma directa para destruir misiles entrantes a 10,000 km.\n"
        "• Sin Consumo de Energía por Disparo: Opera sin requerir grandes plantas de energía o reactores dedicados.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Una torreta cuádruple Gauss montada en un director de tiro de alta velocidad destruirá hasta 4 misiles enemigos por cada pulso de 5 segundos.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Diseña torretas Gauss con velocidad de seguimiento ajustada a la velocidad proyectada de los misiles enemigos."
    ),

    "Meson Cannon": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Cañón Mesónico (Meson Cannon) es un arma de partículas subatómicas que atraviesa escudos y blindaje sólido sin sufrir atenuación, impactando directamente en los componentes internos.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Ignora Escudos y Blindaje: Causa 1 punto de daño directo a los componentes internos (motores, reactores, pañoles) sin importar cuantas capas de blindaje tenga el enemigo.\n"
        "• Daño Interno Asegurado: Excelente contra naves hiper-blindadas o con escudos indestructibles.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Frente a una estación orbital aliada con 20 capas de blindaje, los cañones Meson ignorarán todo el blindaje provocando incendios y fallos internos de inmediato.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Equipa tus escoltas de ataque rápido con cañones Meson para inutilizar los reactores de naves capitales enemigas sin necesidad de desgastar sus escudos."
    ),

    "Particle Beam": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Haz de Partículas (Particle Beam) es un arma de energía pesada que proyecta un torrente concentrado de protones o iones con alcance y daño constante a cualquier distancia.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Sin Atenuación por Distancia: A diferencia del láser, causa el 100% de su daño nominal tanto a corta como a máxima distancia de alcance.\n"
        "• Gran Alcance Operativo: Permite atacar blancos fuera del alcance de respuesta de las armas cinéticas enemigas.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un Haz de Partículas de 15 puntos de daño infligirá exactamente 15 puntos de daño a 300,000 km de distancia, perforando el casco enemigo sin perder fuerza.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Monta Haces de Partículas en buques francotirador de largo alcance para desgastar las naves enemigas antes de que puedan acortar distancias."
    ),

    "Plasma Carronade": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Carronada de Plasma (Plasma Carronade) es un arma de energía pesada a corta distancia que dispara una esfera de plasma supercalentado de tremendo poder destructivo.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Daño Masivo a Corta Distancia: Produce el mayor daño por disparo de todas las armas de energía a distancias quemarropa.\n"
        "• Rápida Caída de Fuerza: Su daño decae bruscamente a medida que aumenta la distancia al objetivo.\n"
        "• Económica de Desarrollar: Requiere muy pocos Puntos de Investigación (RP) en comparación con el láser pesado.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "A 10,000 km, una Carronada de Plasma pesada causará 32 puntos de daño, destrozando el blindaje y casco de una fragata enemiga en un solo disparo.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Equipa naves de emboscada o de asalto en puntos de salto con carronadas de plasma para aniquilar enemigos nada más cruzar el portal."
    ),

    "Railgun": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Cañón de Riel (Railgun) es un arma cinética electromagnética que dispara proyectiles de alta velocidad con capacidad de penetración lineal y múltiples disparos.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Ráfaga Kinetica Multi-Impacto: Dispara 4 proyectiles por unidad de disparo, causando 1 punto de daño penetrante por cada proyectil que impacte.\n"
        "• Doble Uso (PD y Ataque): Funciona tanto para defenderse de misiles a corta distancia como para desgastar blindajes enemigos a media distancia.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Una salva de 4 cañones Railgun lanzará 16 proyectiles por pulso, destruyendo misiles o erosionando la capa exterior del blindaje hostil.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Utiliza cañones Railgun en fragatas multipropósito que requieran capacidad de defensa antiaérea y combate de superficie simultáneo."
    ),

    "Cloaking Device": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Dispositivo de Camuflaje (Cloaking Device) es un generador de campo de distorsión sensorial que reduce drásticamente la firma activa y pasiva de la nave espacial.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Reducción de Sección de Radar (TCS): Reduce la masa aparente de la nave ante los radares activos enemigos (ej. una nave de 10,000t aparentará ser de 500t).\n"
        "• Sigilo Táctico: Permite aproximarse a flotas hostiles o realizar reconocimientos sin ser detectado.\n"
        "• Requisito de Espacio: Ocupa un porcentaje importante del casco de la nave y consume energía activa.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un submarino estelar de 5,000 toneladas con un Cloaking Device del 90% solo podrá ser detectado por radares enemigos a menos de 15 Mkm.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Combina el dispositivo de camuflaje con motores de reducida firma térmica para crear corbetas invisibles de ataque rápido."
    ),

    "Magazine": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Pañol de Munición / Pañol de Misiles (Magazine) es la bodega blindada e ignífuga interna diseñada para almacenar misiles, torpedos y señuelos a bordo de la nave.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Capacidad de Misiles (MSP): Define cuántos Puntos de Tamaño de Misil (MSP) puede albergar la nave (ej. un pañol de 100 MSP puede llevar 20 misiles de Tamaño 5).\n"
        "• Riesgo de Explosión Secundaria: Si un impacto de arma enemiga destruye un pañol lleno, la munición explotará internamente destruyendo la nave por completo.\n"
        "• Blindaje de Pañol (Magazine Armouring): Reduce la probabilidad de detonación interna en caso de daño estructural.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un crucero de misiles con 500 MSP de pañol podrá sostener hasta 10 salvas completas de 50 misiles anti-buque antes de agotar municiones.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Investiga siempre la tecnología de blindaje de pañoles (HTK / Armouring) para evitar que tus destructores exploten por impactos afortunados del enemigo."
    ),

    # --- COMMANDER ROLES & SHIPYARD OPERATIONS ---
    "Oficial Naval": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Comandante u Oficial de la Armada Imperial (Naval Officer). Líder militar formado en la Academia para asumir el mando de naves de combate, escuadras y flotas.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Bonificaciones de Combate: Otorga bonos directos a la velocidad de la flota, precisión de tiro, recarga de armas, sensores y velocidad de reacción.\n"
        "• Moral de Tripulación: Mantiene alta la moral de los marineros en misiones prolongadas de espacio profundo.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Asignar a un Almirante con un +20% en 'Velocidad de Mando' incrementará la velocidad táctica de toda su flota de combate.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Asigna tus mejores oficiales navales a los buques insignia (Flagships) y portaaviones principales."
    ),

    "Comandante Terrestre": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Comandante de Fuerzas Terrestres (Ground Force Commander). Oficial especializado en la táctica de combate de infantería, blindados y artillería planetaria.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Mando de Brigada y División: Otorga bonificaciones a las tropas bajo su mando durante invasiones anfibias o defensa de colonias.\n"
        "• Eficiencia Táctica: Aumenta la potencia de fuego, fortificación y resistencia de las formaciones terrestres.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un General con +15% de bonificación en 'Ataque Terrestre' reducirá las bajas propias a la mitad al asaltar una colonia enemiga fortificada.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Asigna comandantes terrestres a tus formaciones de Asalto Planetario antes de iniciar cualquier operación de desembarco."
    ),

    "Gobernador Planetario": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Gobernador Planetario o Sectorial (Planetary Governor). Administrador civil y político encargado del desarrollo socioeconómico de una colonia o sistema.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Potenciación de Industria: Otorga bonos porcentuales a la producción de fábricas, minas, astilleros, refinerías e ingresos fiscales de la colonia.\n"
        "• Reducción de Malestar Social: Mantiene bajo el nivel de agitación civil e incrementa la lealtad imperial.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un Gobernador con +25% en 'Producción de Fábricas' generará la misma producción en la capital que haber construido 250 fábricas adicionales.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Coloca a tus gobernadores con mejores atributos en la capital imperial Sol y en tus principales nodos mineros."
    ),

    "Científico": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Científico Líder de I+D (Research Scientist). Investigador de élite asignado a dirigir laboratorios de desarrollo tecnológico.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Especialidad de I+D: Cada científico cuenta con una especialidad (ej. Energía, Misiles/Cinética, Sensores, Propulsión, Biología).\n"
        "• Bonificación de Velocidad: Si investiga un proyecto de su propia especialidad, su bonificación de investigación se aplicará al 100% sobre los laboratorios.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un científico con especialidad en 'Propulsión' y bonificación del +30% reducirá el tiempo de investigación de un nuevo motor de 4 años a menos de 3 años.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Asigna SIEMPRE a cada científico proyectos que coincidan con su campo de especialidad para maximizar el rendimiento de tus laboratorios."
    ),

    "Build": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Construcción Naval en Astillero (Build Ship). Tarea industrial encargada de fabricar una nueva unidad de nave espacial en una grada libre de astillero.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Consumo de BP y Minerales: Consume Puntos de Construcción (BP) y minerales exóticos por turno hasta completar el 100% del casco.\n"
        "• Requisito de Retooling: El astillero debe estar adaptado (Retooled) a la clase de nave que se desea construir.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Botar un nuevo crucero de 10,000 toneladas tomará 1.5 años en un astillero con 2 gradas activas.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Asegúrate de contar con los minerales requeridos almacenados en la colonia antes de ordenar construcciones en masa."
    ),

    "Retool": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Reequipamiento / Adaptación de Astillero (Retool Shipyard). Proceso mediante el cual un astillero reconfigura sus gradas para fabricar una nueva clase de nave.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Modificación de Maquinaria: Adapta las herramientas y grúas del astillero al plano blueprint de la nueva clase elegida.\n"
        "• Costo y Tiempo de Retooling: Requiere tiempo y BP proporcionales a la diferencia entre la clase anterior y la nueva.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Si actualizas tu clase de Destructor de la versión MK-I a la versión MK-II, deberás ejecutar un Retool en el astillero antes de botar el nuevo modelo.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Diseña familias de naves con componentes similares para reducir el costo y tiempo de retooling en tus astilleros."
    ),

    "Expand Shipyard": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Expansión de Capacidad o Gradas de Astillero (Expand Shipyard Capacity / Add Slipway). Ampliación física del tonelaje o número de gradas de construcción.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Incremento de Tonelaje (Capacity): Aumenta la masa máxima de naves que el astillero puede construir (ej. pasar de 5,000t a 10,000t).\n"
        "• Adición de Gradas (Add Slipway): Permite construir múltiples naves de la misma clase simultáneamente.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Añadir 2 gradas adicionales a un astillero comercial te permitirá construir 3 cargueros al mismo tiempo.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Expande continuamente tus astilleros comerciales y militares para estar preparado para construir naves capitales de mayor tonelaje."
    )
}

# Update dictionary with deep master additions
for k, v in deep_master_additions.items():
    existing_dict[k] = v

with open(existing_json_path, 'w', encoding='utf-8') as f:
    json.dump(existing_dict, f, ensure_ascii=False, indent=2)

print("Enriched all missing minerals, weapons, roles, and shipyard operations. Total count:", len(existing_dict))

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
