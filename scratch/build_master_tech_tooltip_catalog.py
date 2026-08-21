import sqlite3
import json
import os
import re

db_path = r"C:\VSCODE\Aurora271Full\AuroraDB.db"
json_output_path = r"c:\VSCODE\Aurora271Full\Patches\AuroraSpanish\AuroraTooltipDictionary.json"

conn = sqlite3.connect(db_path)
c = conn.cursor()

# Load existing manually refined dictionary
existing_dict = {}
if os.path.exists(json_output_path):
    with open(json_output_path, "r", encoding="utf-8") as f:
        existing_dict = json.load(f)

# Query all unique tech names & descriptions
c.execute("SELECT DISTINCT Name FROM FCT_TechSystem WHERE Name IS NOT NULL AND Name != ''")
rows = c.fetchall()

print(f"Loaded {len(rows)} tech names from database.")

added_count = 0

for (name,) in rows:
    name_clean = name.strip()
    if not name_clean or name_clean in existing_dict:
        continue

    # Auto-generate contextual tooltip description for master tech catalog
    desc = ""
    lower = name_clean.lower()

    if "laser" in lower:
        desc = f"💡 Arma de Energía (Láser): {name_clean}.\n• Dispara haces calóricos a velocidad luz. No consume munición pero requiere reactores de potencia y condensadores de recarga."
    elif "carronade" in lower:
        desc = f"💡 Carronada de Plasma / Energía: {name_clean}.\n• Arma de energía de corto alcance y daño devastador en combate cercano."
    elif "railgun" in lower:
        desc = f"💡 Cañón Railgun: {name_clean}.\n• Dispara proyectiles cinéticos a extrema velocidad. Ideal para defensa de punto (Point Defense) contra misiles."
    elif "gauss" in lower:
        desc = f"💡 Cañón Gauss: {name_clean}.\n• Sistema de defensa antimisil de alta cadencia de tiro por segundo."
    elif "missile" in lower or "warhead" in lower:
        desc = f"💡 Tecnología de Misiles & Munición: {name_clean}.\n• Especificación táctica de alcance, guía terminal, carga explosiva o ECM/ECCM para ordenanzas navales."
    elif "sensor" in lower or "tracking" in lower or "ecm" in lower:
        desc = f"💡 Sensores & Control de Tiro: {name_clean}.\n• Dispositivo pasivo/activo de exploración o guerra electrónica para detección y fijación de blanco."
    elif "engine" in lower or "drive" in lower or "propulsion" in lower:
        desc = f"💡 Propulsión & Motores Navales: {name_clean}.\n• Tecnología de motores espaciales para incrementar la velocidad (km/s) y eficiencia de combustible."
    elif "shield" in lower:
        desc = f"💡 Generador de Escudos: {name_clean}.\n• Campo de fuerza defensivo que absorbe impactos de armas de energía y explosivos antes de dañar el blindaje."
    elif "armor" in lower or "armour" in lower:
        desc = f"💡 Aleación de Blindaje: {name_clean}.\n• Revestimiento protector de casco contra perforaciones enemigas."
    elif "cargo" in lower or "hold" in lower or "shuttle" in lower:
        desc = f"💡 Módulo de Carga & Logística: {name_clean}.\n• Capacidad de transporte de mercancías, minerales o navetas comerciales."
    elif "mining" in lower or "mina" in lower:
        desc = f"💡 Extracción Minera: {name_clean}.\n• Rendimiento de prospección y extracción de los 11 minerales trans-newtonianos."
    elif "research" in lower or "laboratorio" in lower:
        desc = f"💡 Investigación Cientiífica: {name_clean}.\n• Eficiencia y velocidad de generación de puntos de investigación (RP/año)."
    elif "construction" in lower or "fábrica" in lower:
        desc = f"💡 Manufactura Industrial: {name_clean}.\n• Capacidad de producción en puntos de construcción (BP/año)."
    elif "genetic" in lower or "genome" in lower:
        desc = f"💡 Modificación Genética: {name_clean}.\n• Secuencia genómica para adaptar especies a gravedades o temperaturas extremas."
    elif "crew" in lower or "quarters" in lower:
        desc = f"💡 Hábitats & Alojamiento de Tripulación: {name_clean}.\n• Módulos de soporte de vida y habitabilidad para marineros y oficiales."
    else:
        desc = f"💡 Tecnología / Instalación Imperial: {name_clean}.\n• Componente e innovación del catálogo oficial de investigación e industria de Aurora 4X."

    existing_dict[name_clean] = desc
    added_count += 1

print(f"Added {added_count} new master tech tooltips to dictionary. Total dictionary entries: {len(existing_dict)}")

with open(json_output_path, "w", encoding="utf-8") as f:
    json.dump(existing_dict, f, ensure_ascii=False, indent=2)

print("Saved master catalog dictionary successfully!")
