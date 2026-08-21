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
# ALL SPANISH PLURAL & SINGULAR INSTALLATION ARTICLES (Deep 4-Section Format)
# -----------------------------------------------------------------------------
deep_installations = {
    # 1. Spaceport / Puertos Espaciales
    "Puertos Espaciales de Carga": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Puerto Espacial (Spaceport) es la megaestructura logística orbital y de superficie encargada del manejo masivo de carga, reabastecimiento de combustible y transferencia de pasajeros entre el planeta y la flota espacial.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Aceleración de Carga y Descarga: Incrementa drásticamente la velocidad a la que los barcos cargueros (Freighters) y colonizadores cargan y descargan mercancías, minas e infraestructura.\n"
        "• Hub Comercial Imperial: Atrae rutas comerciales civiles de freighters automatizados, generando ingresos masivos en Riqueza (Wealth) e Impuestos.\n"
        "• Capacidad de Mantenimiento y Repostaje: Actúa como punto de abastecimiento rápido de combustible Sorium LPH para naves comerciales y militares en órbita.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "En una colonia sin Puerto Espacial, desembarcar 100 Minas Automatizadas desde una flota de cargueros puede demorar 20 días. Al construir 2 Puertos Espaciales de Carga en la colonia, el tiempo de transferencia caerá a menos de 2 días.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Construye al menos 1 Puerto Espacial en cada mundo colonial principal o nudo logístico para evitar cuellos de botella en la expansión imperial."
    ),
    "Puerto Espacial": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Puerto Espacial (Spaceport) es la megaestructura logística orbital y de superficie encargada del manejo masivo de carga, reabastecimiento de combustible y transferencia de pasajeros entre el planeta y la flota espacial.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Aceleración de Carga y Descarga: Incrementa drásticamente la velocidad a la que los barcos cargueros (Freighters) y colonizadores cargan y descargan mercancías, minas e infraestructura.\n"
        "• Hub Comercial Imperial: Atrae rutas comerciales civiles de freighters automatizados, generando ingresos masivos en Riqueza (Wealth) e Impuestos.\n"
        "• Capacidad de Mantenimiento y Repostaje: Actúa como punto de abastecimiento rápido de combustible Sorium LPH para naves comerciales y militares en órbita.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "En una colonia sin Puerto Espacial, desembarcar 100 Minas Automatizadas desde una flota de cargueros puede demorar 20 días. Al construir 2 Puertos Espaciales de Carga en la colonia, el tiempo de transferencia caerá a menos de 2 días.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Construye al menos 1 Puerto Espacial en cada mundo colonial principal o nudo logístico para evitar cuellos de botella en la expansión imperial."
    ),
    "Spaceport": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Puerto Espacial (Spaceport) es la megaestructura logística orbital y de superficie encargada del manejo masivo de carga, reabastecimiento de combustible y transferencia de pasajeros entre el planeta y la flota espacial.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Aceleración de Carga y Descarga: Incrementa drásticamente la velocidad a la que los barcos cargueros (Freighters) y colonizadores cargan y descargan mercancías, minas e infraestructura.\n"
        "• Hub Comercial Imperial: Atrae rutas comerciales civiles de freighters automatizados, generando ingresos masivos en Riqueza (Wealth) e Impuestos.\n"
        "• Capacidad de Mantenimiento y Repostaje: Actúa como punto de abastecimiento rápido de combustible Sorium LPH para naves comerciales y militares en órbita.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "En una colonia sin Puerto Espacial, desembarcar 100 Minas Automatizadas desde una flota de cargueros puede demorar 20 días. Al construir 2 Puertos Espaciales de Carga en la colonia, el tiempo de transferencia caerá a menos de 2 días.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Construye al menos 1 Puerto Espacial en cada mundo colonial principal o nudo logístico para evitar cuellos de botella en la expansión imperial."
    ),

    # 2. Naval HQ / Cuartel General Naval de Sector
    "Cuartel General Naval de Sector": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Cuartel General Naval (Naval HQ / Sector HQ) es la instalación de mando y comunicaciones estratégicas que administra las operaciones navales en un sector estelar.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Extensión del Rango de Mando: Permite asignar Almirantes de Sector para otorgar bonificaciones operativas a todas las flotas desplegadas en los sistemas estelares coordinados por este HQ.\n"
        "• Bonificación de Eficiencia y Moral: Reduce el desgaste operativo y mantiene la moral de las tripulaciones de combate a niveles óptimos.\n"
        "• Coordinación de Inteligencia: Mejora la velocidad de procesamiento de eventos y contactos hostiles detectados en el sector.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Construir un Cuartel General Naval de Sector en Alfa Centauri permite asignar a un Vicealmirante que aumentará un +15% la precisión de tiro y un +10% la velocidad de todos los buques en ese sector.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Despliega un Naval HQ en cada sistema estelar estratégico que albergue bases navales avanzadas o puntos de salto disputados."
    ),
    "Cuartel General Naval": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Cuartel General Naval (Naval HQ / Sector HQ) es la instalación de mando y comunicaciones estratégicas que administra las operaciones navales en un sector estelar.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Extensión del Rango de Mando: Permite asignar Almirantes de Sector para otorgar bonificaciones operativas a todas las flotas desplegadas en los sistemas estelares coordinados por este HQ.\n"
        "• Bonificación de Eficiencia y Moral: Reduce el desgaste operativo y mantiene la moral de las tripulaciones de combate a niveles óptimos.\n"
        "• Coordinación de Inteligencia: Mejora la velocidad de procesamiento de eventos y contactos hostiles detectados en el sector.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Construir un Cuartel General Naval de Sector en Alfa Centauri permite asignar a un Vicealmirante que aumentará un +15% la precisión de tiro y un +10% la velocidad de todos los buques en ese sector.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Despliega un Naval HQ en cada sistema estelar estratégico que albergue bases navales avanzadas o puntos de salto disputados."
    ),
    "Naval HQ": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Cuartel General Naval (Naval HQ / Sector HQ) es la instalación de mando y comunicaciones estratégicas que administra las operaciones navales en un sector estelar.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Extensión del Rango de Mando: Permite asignar Almirantes de Sector para otorgar bonificaciones operativas a todas las flotas desplegadas en los sistemas estelares coordinados por este HQ.\n"
        "• Bonificación de Eficiencia y Moral: Reduce el desgaste operativo y mantiene la moral de las tripulaciones de combate a niveles óptimos.\n"
        "• Coordinación de Inteligencia: Mejora la velocidad de procesamiento de eventos y contactos hostiles detectados en el sector.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Construir un Cuartel General Naval de Sector en Alfa Centauri permite asignar a un Vicealmirante que aumentará un +15% la precisión de tiro y un +10% la velocidad de todos los buques en ese sector.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Despliega un Naval HQ en cada sistema estelar estratégico que albergue bases navales avanzadas o puntos de salto disputados."
    ),

    # 3. Ground Force Training Complex / Complejo de Entrenamiento Terrestre
    "Complejo de Entrenamiento Terrestre": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Complejo de Entrenamiento Terrestre (Ground Force Training Complex) es la instalación militar dedicada a la instrucción, equipamiento y reclutamiento de brigadas de infantería, tanques y artillería planetaria.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Adiestramiento de Tropas: Permite entrenar y desplegar unidades terrestres defensivas y de invasión militar (Infantería de Marina, Guarniciones, Artillería de Campo, Blindados).\n"
        "• Defensa Planetaria: Las tropas producidas defienden la colonia contra invasiones anfibias enemigas y mantienen el orden social.\n"
        "• Capacidad de Producción de Tropas: A mayor número de complejos, menor será el tiempo necesario para formar divisiones de combate.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Para conquistar un planeta alienígena fortificado, necesitarás operar 10 Complejos de Entrenamiento Terrestre en la Tierra para producir 20 brigadas de Asalto Planetario en tiempo récord.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Mantén guarniciones defensivas de tropas en todas las colonias con instalaciones industriales valiosas para disuadir incursiones de abordaje."
    ),
    "Complejo de Tropas Terrestres": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Complejo de Entrenamiento Terrestre (Ground Force Training Complex) es la instalación militar dedicada a la instrucción, equipamiento y reclutamiento de brigadas de infantería, tanques y artillería planetaria.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Adiestramiento de Tropas: Permite entrenar y desplegar unidades terrestres defensivas y de invasión militar (Infantería de Marina, Guarniciones, Artillería de Campo, Blindados).\n"
        "• Defensa Planetaria: Las tropas producidas defienden la colonia contra invasiones anfibias enemigas y mantienen el orden social.\n"
        "• Capacidad de Producción de Tropas: A mayor número de complejos, menor será el tiempo necesario para formar divisiones de combate.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Para conquistar un planeta alienígena fortificado, necesitarás operar 10 Complejos de Entrenamiento Terrestre en la Tierra para producir 20 brigadas de Asalto Planetario en tiempo récord.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Mantén guarniciones defensivas de tropas en todas las colonias con instalaciones industriales valiosas para disuadir incursiones de abordaje."
    ),
    "Ground Force Training Complex": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Complejo de Entrenamiento Terrestre (Ground Force Training Complex) es la instalación militar dedicada a la instrucción, equipamiento y reclutamiento de brigadas de infantería, tanques y artillería planetaria.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Adiestramiento de Tropas: Permite entrenar y desplegar unidades terrestres defensivas y de invasión militar (Infantería de Marina, Guarniciones, Artillería de Campo, Blindados).\n"
        "• Defensa Planetaria: Las tropas producidas defienden la colonia contra invasiones anfibias enemigas y mantienen el orden social.\n"
        "• Capacidad de Producción de Tropas: A mayor número de complejos, menor será el tiempo necesario para formar divisiones de combate.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Para conquistar un planeta alienígena fortificado, necesitarás operar 10 Complejos de Entrenamiento Terrestre en la Tierra para producir 20 brigadas de Asalto Planetario en tiempo récord.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Mantén guarniciones defensivas de tropas en todas las colonias con instalaciones industriales valiosas para disuadir incursiones de abordaje."
    ),

    # 4. Deep Space Tracking Station / Estaciones de Tracking Espacial Profundo
    "Estaciones de Tracking Espacial Profundo": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Estación de Seguimiento de Espacio Profundo (Deep Space Tracking Station) es un complejo planetario de antenas pasivas electromagnéticas y térmicas de ultra-largo alcance.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Detección Pasiva Sistema-Completo: Actúa como un sensor pasivo gigante integrado en la colonia que escanea todo el sistema solar.\n"
        "• Vigilancia Temprana: Detecta la presencia de flotas y misiles enemigos en el espacio profundo sin revelar la colonia.\n"
        "• Acumulación de Potencia: Cada estación adicional incrementa el nivel de sensibilidad pasiva de la colonia.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Construir 5 Estaciones de Tracking Espacial Profundo en la colonia de Marte permitirá detectar cualquier flota hostil cruzando el sistema solar antes de que se aproxime a la Tierra.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Construye estaciones de tracking en todos los mundos fronterizos para garantizar una alerta temprana impecable."
    ),
    "Estación de Espacio Profundo": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Estación de Seguimiento de Espacio Profundo (Deep Space Tracking Station) es un complejo planetario de antenas pasivas electromagnéticas y térmicas de ultra-largo alcance.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Detección Pasiva Sistema-Completo: Actúa como un sensor pasivo gigante integrado en la colonia que escanea todo el sistema solar.\n"
        "• Vigilancia Temprana: Detecta la presencia de flotas y misiles enemigos en el espacio profundo sin revelar la colonia.\n"
        "• Acumulación de Potencia: Cada estación adicional incrementa el nivel de sensibilidad pasiva de la colonia.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Construir 5 Estaciones de Tracking Espacial Profundo en la colonia de Marte permitirá detectar cualquier flota hostil cruzando el sistema solar antes de que se aproxime a la Tierra.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Construye estaciones de tracking en todos los mundos fronterizos para garantizar una alerta temprana impecable."
    ),
    "Deep Space Tracking Station": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Estación de Seguimiento de Espacio Profundo (Deep Space Tracking Station) es un complejo planetario de antenas pasivas electromagnéticas y térmicas de ultra-largo alcance.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Detección Pasiva Sistema-Completo: Actúa como un sensor pasivo gigante integrado en la colonia que escanea todo el sistema solar.\n"
        "• Vigilancia Temprana: Detecta la presencia de flotas y misiles enemigos en el espacio profundo sin revelar la colonia.\n"
        "• Acumulación de Potencia: Cada estación adicional incrementa el nivel de sensibilidad pasiva de la colonia.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Construir 5 Estaciones de Tracking Espacial Profundo en la colonia de Marte permitirá detectar cualquier flota hostil cruzando el sistema solar antes de que se aproxime a la Tierra.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Construye estaciones de tracking en todos los mundos fronterizos para garantizar una alerta temprana impecable."
    ),

    # 5. Mass Driver / Catapulta de Masa Orbital
    "Catapulta de Masa Orbital": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Catapulta de Masa (Mass Driver) es un acelerador electromagnético lineal que dispara paquetes de mineral trans-newtoniano a velocidades interplanetarias entre colonias.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Transferencia Automática de Minerales: Permite enviar miles de toneladas de Duranium, Sorium y Gallicite desde una colonia minera hacia el planeta capital sin usar barcos cargueros.\n"
        "• Red Objetivo (Destination Tag): Se configura para apuntar a un mundo receptor equipado con otra catapulta de masa para capturar los minerales lanzados.\n"
        "• Peligro de Bombardeo Kinetico: Si lanzas minerales a una colonia que NO tiene catapulta de masa receptora, los minerales impactarán la superficie como meteoritos provocando la destrucción de la población e industria.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Al colocar 1 Catapulta de Masa en el asteroide Vesta y apuntarla hacia la Tierra (que cuenta con 1 Catapulta de Masa receptora), todo el mineral extraído en Vesta volará automáticamente a la Tierra cada turno.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Asegúrate SIEMPRE de que el planeta de destino tenga una Catapulta de Masa instalada antes de iniciar los envíos masivos."
    ),
    "Catapulta de Masa": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Catapulta de Masa (Mass Driver) es un acelerador electromagnético lineal que dispara paquetes de mineral trans-newtoniano a velocidades interplanetarias entre colonias.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Transferencia Automática de Minerales: Permite enviar miles de toneladas de Duranium, Sorium y Gallicite desde una colonia minera hacia el planeta capital sin usar barcos cargueros.\n"
        "• Red Objetivo (Destination Tag): Se configura para apuntar a un mundo receptor equipado con otra catapulta de masa para capturar los minerales lanzados.\n"
        "• Peligro de Bombardeo Kinetico: Si lanzas minerales a una colonia que NO tiene catapulta de masa receptora, los minerales impactarán la superficie como meteoritos provocando la destrucción de la población e industria.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Al colocar 1 Catapulta de Masa en el asteroide Vesta y apuntarla hacia la Tierra (que cuenta con 1 Catapulta de Masa receptora), todo el mineral extraído en Vesta volará automáticamente a la Tierra cada turno.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Asegúrate SIEMPRE de que el planeta de destino tenga una Catapulta de Masa instalada antes de iniciar los envíos masivos."
    ),
    "Mass Driver": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Catapulta de Masa (Mass Driver) es un acelerador electromagnético lineal que dispara paquetes de mineral trans-newtoniano a velocidades interplanetarias entre colonias.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Transferencia Automática de Minerales: Permite enviar miles de toneladas de Duranium, Sorium y Gallicite desde una colonia minera hacia el planeta capital sin usar barcos cargueros.\n"
        "• Red Objetivo (Destination Tag): Se configura para apuntar a un mundo receptor equipado con otra catapulta de masa para capturar los minerales lanzados.\n"
        "• Peligro de Bombardeo Kinetico: Si lanzas minerales a una colonia que NO tiene catapulta de masa receptora, los minerales impactarán la superficie como meteoritos provocando la destrucción de la población e industria.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Al colocar 1 Catapulta de Masa en el asteroide Vesta y apuntarla hacia la Tierra (que cuenta con 1 Catapulta de Masa receptora), todo el mineral extraído en Vesta volará automáticamente a la Tierra cada turno.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Asegúrate SIEMPRE de que el planeta de destino tenga una Catapulta de Masa instalada antes de iniciar los envíos masivos."
    ),

    # 6. Terraforming Station / Estación de Terraformación Atmosférica
    "Estación de Terraformación Atmosférica": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Instalación de Terraformación (Terraforming Station) es un complejo industrial planetario dedicado a la inyección y extracción masiva de gases atmosféricos.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Alteración Atmosférica: Inyecta Oxígeno, Nitrógeno o Gases Inertes (Aetherium) para crear una atmósfera respirable y ajustar la presión atmosférica a 1.0 atm.\n"
        "• Regulación Térmica: Inyecta o extrae gases de efecto invernadero (GHG / Anti-GHG) para elevar o reducir la temperatura del planeta.\n"
        "• Reducción del Costo Colonial (Colony Cost 0.00): Al transformar el mundo en habitable, la población civil deja de requerir domos de Infraestructura.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Operar 20 Estaciones de Terraformación en Marte durante 5 años inyectará suficiente Oxígeno e Invernadero para convertirlo en un mundo idéntico a la Tierra con Costo Colonial 0.00.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Combina instalaciones terrestres de terraformación con naves terraformadoras orbitales para acelerar el proceso."
    ),
    "Estación de Terraformación": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Instalación de Terraformación (Terraforming Station) es un complejo industrial planetario dedicado a la inyección y extracción masiva de gases atmosféricos.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Alteración Atmosférica: Inyecta Oxígeno, Nitrógeno o Gases Inertes (Aetherium) para crear una atmósfera respirable y ajustar la presión atmosférica a 1.0 atm.\n"
        "• Regulación Térmica: Inyecta o extrae gases de efecto invernadero (GHG / Anti-GHG) para elevar o reducir la temperatura del planeta.\n"
        "• Reducción del Costo Colonial (Colony Cost 0.00): Al transformar el mundo en habitable, la población civil deja de requerir domos de Infraestructura.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Operar 20 Estaciones de Terraformación en Marte durante 5 años inyectará suficiente Oxígeno e Invernadero para convertirlo en un mundo idéntico a la Tierra con Costo Colonial 0.00.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Combina instalaciones terrestres de terraformación con naves terraformadoras orbitales para acelerar el proceso."
    ),
    "Terraforming Station": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Instalación de Terraformación (Terraforming Station) es un complejo industrial planetario dedicado a la inyección y extracción masiva de gases atmosféricos.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Alteración Atmosférica: Inyecta Oxígeno, Nitrógeno o Gases Inertes (Aetherium) para crear una atmósfera respirable y ajustar la presión atmosférica a 1.0 atm.\n"
        "• Regulación Térmica: Inyecta o extrae gases de efecto invernadero (GHG / Anti-GHG) para elevar o reducir la temperatura del planeta.\n"
        "• Reducción del Costo Colonial (Colony Cost 0.00): Al transformar el mundo en habitable, la población civil deja de requerir domos de Infraestructura.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Operar 20 Estaciones de Terraformación en Marte durante 5 años inyectará suficiente Oxígeno e Invernadero para convertirlo en un mundo idéntico a la Tierra con Costo Colonial 0.00.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Combina instalaciones terrestres de terraformación con naves terraformadoras orbitales para acelerar el proceso."
    ),

    # Plural names from UI DataGrid
    "Infraestructura de Hábitat Urbano": (
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
    "Fábricas de Construcción Industrial": (
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
    "Minas Convencionales": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Las Minas Convencionales (Conventional Mines) representan la infraestructura minera inicial basada en tecnología de combustibles fósiles pre-trans-newtoniana.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Extracción Inicial: Extraen minerales trans-newtonianos a una tasa reducida (10 t/año por mina vs 12 t/año de la mina automatizada).\n"
        "• Reconversión Industrial: Pueden ser convertidas progresivamente en Fábricas de Construcción, Minas Automatizadas o Refinerías mediante proyectos industriales.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Al inicio de una partida convencional en la Tierra, reconvertir 500 Minas Convencionales en Minas Automatizadas duplicará tu flujo de materia prima.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Reconvierte todas tus minas convencionales tan pronto como investigues la tecnología de minería trans-newtoniana."
    ),
    "Centros Financieros y Comerciales": (
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
    "Refinerías de Sorium (Combustible)": (
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
    "Fábricas de Ordenanza y Municiones": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Fábrica de Ordenanza (Ordnance Factory) es la instalación industrial pesada militar encargada de la producción en masa de misiles, torpedos y minas espaciales.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Fabricación de Misiles: Transforma Puntos de Construcción (BP) y minerales exóticos en unidades físicas de misiles diseñados en la app.\n"
        "• Abastecimiento de Pañoles: Mantiene repletos los depósitos planetarios y de naves de combate.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Para mantener abastecida una flota de 10 destructores de misiles que consumen 200 misiles por batalla, necesitarás al menos 20 Fábricas de Ordenanza produciendo municiones continuamente.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Almacena reservas estratégicas de misiles de interceptación (AMM) y anti-buque (ASM) en tus bases navales avanzadas."
    ),
    "Fábricas de Cazas Navales": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Fábrica de Cazas (Fighter Factory) es una planta industrial aeronáutica especializada en el ensamblaje de embarcaciones parásito ligeras (< 500 toneladas / < 10 HS).\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Construcción de Cazas: Fabrica unidades de interceptores, cazas de bombardeo y lanzaderas de reconocimiento sin consumir gradas de astillero naval.\n"
        "• Liberación de Astilleros: Permite reservar los grandes astilleros navales exclusivamente para cruceros y acorazados.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Con 30 Fábricas de Cazas activas, podrás botar un escuadrón entero de 12 cazas de combate cada 30 días.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Asigna un grupo de fábricas de cazas a renovar continuamente las pérdidas sufridas por tus portacazas."
    ),
    "Laboratorios de I+D e Investigación": (
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
    "Instalaciones de Mantenimiento Naval": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "La Instalación de Mantenimiento Naval (Maintenance Facility) proporciona soporte de infraestructura orbital y de astillero para reparar y prevenir fallos en naves militares atracadas.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Capacidad de Mantenimiento Colonial: Cada instalación incrementa la capacidad máxima de tonelaje naval que la colonia puede mantener en puerto sin desgaste.\n"
        "• Eliminación del Reloj de Desgaste: Si una nave militar de 5,000t permanece en una colonia con 5,000t de capacidad de mantenimiento, su temporizador de fallos de componentes se congelará al 100% de fiabilidad.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Construir 100 Instalaciones de Mantenimiento Naval en el puerto de la Tierra otorgará 20,000 toneladas de capacidad de mantenimiento libre para tu flota de combate.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Despliega instalaciones de mantenimiento en tus estaciones navales de frontera para estacionar patrulleras sin sufrir averías internas."
    ),
    "Academias Militares de Oficiales": (
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
    )
}

# Update dictionary with deep custom articles
for k, v in deep_installations.items():
    existing_dict[k] = v

with open(existing_json_path, 'w', encoding='utf-8') as f:
    json.dump(existing_dict, f, ensure_ascii=False, indent=2)

print("Enriched all Spanish plural and singular installations. Total count:", len(existing_dict))

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
