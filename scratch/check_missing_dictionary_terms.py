import json
import os

json_path = 'c:/VSCODE/AuroraDesignSuite/config/AuroraTooltipDictionary.json'
with open(json_path, 'r', encoding='utf-8') as f:
    dict_data = json.load(f)

print("Total keys in dictionary:", len(dict_data))

target_terms = [
    # 11 Minerals
    "Duranium", "Sorium", "Neutronium", "Corium", "Tritanium", "Boronide", 
    "Mercassium", "Vendarite", "Uridium", "Corundium", "Gallicite",

    # Ship Modules & Components
    "Beam Fire Control", "Missile Fire Control", "High Power Microwave", "Decoy Launcher",
    "Gauss Cannon", "Meson Cannon", "Particle Beam", "Plasma Carronade", "Railgun",
    "Cloaking Device", "Magazine", "Fuel Tank", "Engineering Spaces", "Damage Control Complex",
    "Gravitational Survey Sensors", "Geological Survey Sensors", "Troop Transport Bay",
    "Cargo Hold", "Cryogenic Transport Bay", "Colony Module", "Harvester Module",
    "Salvage Module", "Sorium Harvesting Module", "Terraforming Module",

    # Commander Roles & Bonuses
    "Oficial Naval", "Comandante Terrestre", "Gobernador Planetario", "Científico",
    "Mining Bonus", "Factory Bonus", "Wealth Bonus", "Terraforming Bonus", "Survey Bonus",

    # Shipyard Operations
    "Build", "Retool", "Refit", "Expand Shipyard", "Repair"
]

missing_or_short = []
for term in target_terms:
    val = dict_data.get(term)
    if not val or "CONCEPTO" not in val:
        missing_or_short.append(term)

print("Terms missing deep 4-section articles:", missing_or_short)
