import sys

sys.stdout.reconfigure(encoding='utf-8')

print("--- 23 Categories Audit Plan ---")
categories = [
    "📡 Active Sensors", "🛡️ CIWS", "👻 Cloaking Device", "🛰️ Decoy Launcher",
    "🎯 Direct Fire Control", "⚡ EM Detection Sensors", "⚙️ Engines", "✈️ Fighter Pod Bay",
    "🔫 Gauss Cannon", "📡 High Power Microwave", "🌀 Jump Engines", "💥 Lasers",
    "📦 Magazine", "🔮 Meson Cannon", "🛠️ Miscellaneous Components", "🎯 Missile Fire Control",
    "🚀 Missile Launchers", "⚡ Particle Beam", "🔥 Plasma Carronade", "⚡ Power Plants",
    "🚅 Railgun", "🛡️ Shield Generators", "🔥 Thermal Sensors"
]

print(f"Total Categories: {len(categories)}")
for i, cat in enumerate(categories, 1):
    print(f"{i:02d}. {cat}")
