import json
import os

existing_json_path = 'c:/VSCODE/AuroraDesignSuite/config/AuroraTooltipDictionary.json'

existing_dict = {}
if os.path.exists(existing_json_path):
    existing_dict = json.load(open(existing_json_path, 'r', encoding='utf-8'))

specific_terms = {
    "Electronic Hardening": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "El Endurecimiento Electrónico (Electronic Hardening) es la protección electromagnética aplicada a los circuitos, sensores y sistemas de control de tiro de tus naves y misiles.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Protección contra HPM (High Power Microwave): Protege tus sistemas contra cañones de microondas enemigos que intentan fundir tus ordenadores de a bordo.\n"
        "• Resistencia contra Misiles y Sensores: Cada nivel de Electronic Hardening reduce la vulnerabilidad de la nave frente a interferencias electromagnéticas (ECM) hostiles.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Si una nave enemiga equipada con armas de microondas (HPM) te ataca, con Electronic Hardening Level 2 tus sistemas electrónicos permanecerán intactos en lugar de apagarse en pleno combate.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Equipa siempre al menos nivel 1 de Electronic Hardening en naves de vanguardia y buques insignia de mando."
    ),

    "Active Grav Sensor Strength": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Potencia de emisión del Radar Activo (Active Gravitational Sensor Strength). Mide la capacidad del radar de enviar impulsos electromagnéticos para detectar naves enemigas.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Alcance de Detección: A mayor potencia (Strength 16, 32, 64...), mayor será el alcance máximo en millones de kilómetros para localizar objetivos.\n"
        "• Resolución (Resolution): La potencia se combina con la Resolución asignada. Res 1 detecta misiles; Res 100 detecta cruceros a distancias gigantescas.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un radar Active Grav Sensor Strength 16 con Res 100 detectará un acorazado enemigo a más de 400 millones de kilómetros.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "No mantengas activados los radares activos constantemente; delatan tu ubicación a los sensores pasivos enemigos en todo el sistema."
    ),

    "Turret Tracking Speed": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Velocidad de Seguimiento de Torreta (Turret Tracking Speed). Es la velocidad angular en km/s a la que una torreta puede girar para rastrear y fijar objetivos hiperveloces.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Intercepción Antimisil (Point Defense): Los misiles enemigos viajan a 20,000 - 40,000 km/s. Si tu torreta solo rastrea a 5,000 km/s, la probabilidad de acertar caerá drásticamente.\n"
        "• Montaje en Torreta (Turret Mount): Permite que cañones Gauss o Láseres sigan al misil sin importar la velocidad de maniobra de la propia nave matriz.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Una torreta Gauss de 20,000 km/s de Tracking Speed interceptará salvas de misiles entrantes destruyendo hasta 4 misiles por turno de 5 segundos.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Investiga siempre las tecnologías de Turret Tracking Speed para mantener tus fragatas de defensa de punto actualizadas frente a las salvas alienígenas."
    ),

    "Meson Focusing Technology": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Tecnología de Focalización Mesónica (Meson Beam Technology). Es el desarrollo de armas de partículas subatómicas de alta energía.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Ignora Blindaje y Escudos: Los disparos mesónicos ATRAVIESAN los escudos de fuerza y las capas de blindaje metálico sin sufrir atenuación, impactando directamente en la maquinaria interna.\n"
        "• Daño Fijo: Causan 1 punto de daño interno garantizado por impacto directo.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Una nave aliada con cañones Mesón puede destruir los motores de un super-acorazado enemigo sin necesidad de destruir sus 20 capas de blindaje previo.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Los cañones Mesón son mortales en cañoneras ligeras de asalto (FACs) y en torretas planetarias defensivas."
    ),

    "Electronic Counter-countermeasures": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Contra-contramedidas Electrónicas (ECCM). Dispositivos militares que neutralizan la interferencia de radar enemiga (ECM).\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Anula la Penalización de Puntería: Si la nave enemiga activa ECM para desviar tu control de tiro, cada nivel de ECCM contrarresta exactamente 1 nivel de ECM enemigo.\n"
        "• Restablece el Alcance de Guiado: Evita que tus misiles pierdan el blanco a larga distancia.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Sin ECCM, la precisión de tus misiles cae al 20% contra una nave enemiga con jammer. Con ECCM equipado, la precisión vuelve al 100%.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Instala siempre 1 módulo ECCM en cada Control de Tiro de Misiles (MFC) y Control de Tiro de Energía (BFC)."
    ),

    "Minimum Engine Power Modifier": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Modificador Mínimo de Potencia de Motor (Minimum Engine Power Modifier). Permite diseñar motores espaciales reducidos (ej. 10%, 25%, 50% de potencia normal).\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Ultra Eficiencia de Combustible: Reducir la potencia del motor disminuye el consumo de combustible LPH de forma exponencial.\n"
        "• Firma Térmica Mínima: La radiación emitida se reduce drásticamente, haciendo que la nave sea casi invisible a los sensores enemigos.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un motor comercial con modificador x0.25 consume 10 veces menos combustible, permitiendo que un carguero o nave de exploración navegue por toda la galaxia sin repostar.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Utiliza modificadores de baja potencia en naves de prospección geológica y estaciones orbitales de carga."
    ),

    "Maximum Engine Power Modifier": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Modificador Máximo de Potencia de Motor (Maximum Engine Power Modifier). Permite forzar la potencia del motor (ej. x1.5, x2.0, x3.0).\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Velocidad Extrema: Multiplica el empuje de la nave permitiendo alcanzar velocidades militares de más de 10,000 km/s.\n"
        "• Mayor Consumo y Desgaste: Incrementa el consumo de combustible y la firma térmica TCS de la nave.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Un caza interceptor con motor a x3.0 de potencia alcanzará los 35,000 km/s, perfecto para esquivar salvas de misiles enemigos.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Equipa modificadores de alta potencia únicamente en buques de guerra militares y cazas de primera línea."
    ),

    "Base Jump Drive Efficiency": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Eficiencia Base de Motor de Salto (Base Jump Drive Efficiency). Mide la capacidad y ligereza de los motores de salto espacial.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Reduce el Tamaño del Motor de Salto: A mayor eficiencia, menor será el espacio en toneladas (HS) que ocupa el motor de salto en tu nave.\n"
        "• Transición entre Sistemas: Permite que un buque 'Jump Ship' abra un agujero de gusano para transferir una escuadra entera a través de Puntos de Salto.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Una nave de salto con alta eficiencia puede acompañar a una flota de combate sin ralentizar su velocidad ni ocupar demasiado tonelaje militar.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Diseña 1 nave 'Jump Ship' especializada por cada escuadrón naval para ahorrar espacio en las naves de combate puras."
    ),

    "Capacitor Recharge Rate": (
        "📌 CONCEPTO & DEFINICIÓN:\n"
        "Tasa de Recarga de Capacitores (Capacitor Recharge Rate). Medida en unidades de energía por segundo, determina la velocidad a la que se recargan las armas láser y de energía.\n\n"
        "⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n"
        "• Cadencia de Disparo: Un capacitor de mayor nivel permite que tus cañones láser o mesón disparen en cada turno de 5 segundos en lugar de esperar 30 segundos entre disparos.\n"
        "• Sostenibilidad Energética: Requiere reactores de potencia acordes para alimentar la recarga continua.\n\n"
        "💡 EJEMPLO PRÁCTICO EN PARTIDA:\n"
        "Con Capacitor Recharge 3, un cañón láser de 12 EU se recargará completamente en 20 segundos en lugar de 60 segundos.\n\n"
        "🛡️ CONSEJO TÁCTICO IMPERIAL:\n"
        "Investiga la velocidad de recarga de capacitores en paralelo con la potencia de las armas para maximizar el daño por segundo (DPS)."
    )
}

for k, v in specific_terms.items():
    existing_dict[k] = v

with open(existing_json_path, 'w', encoding='utf-8') as f:
    json.dump(existing_dict, f, ensure_ascii=False, indent=2)

print("Enriched specific terms count:", len(existing_dict))

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
