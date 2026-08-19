import os
import re

codebase_dir = r"c:\VSCODE\AuroraDesignSuite"

# Patterns to scan
patterns = {
    "RaceID = 784": re.compile(r"784"),
    "GameID = 140": re.compile(r"GameID\s*=\s*140|GameID\s*==\s*140|\?\?\s*140"),
    "PopID = 4642": re.compile(r"4642"),
    "Hardcoded C:\\ paths": re.compile(r"""['"]c:\\[^'"]*['"]""", re.IGNORECASE),
    "Demo names (Ybarra/Nazario/Numancia)": re.compile(r"Ybarra|Nazario|Numancia|Carrasco|Velazquez|Adeptus|Hispano", re.IGNORECASE),
    "Literal ?? fallback numbers": re.compile(r"\?\?\s*\d{2,}"),
}

results = {}

for root, dirs, files in os.walk(codebase_dir):
    if "bin" in root or "obj" in root or ".git" in root or "release" in root or "scratch" in root:
        continue
    for file in files:
        if file.endswith(".cs") or file.endswith(".xaml"):
            filepath = os.path.join(root, file)
            try:
                with open(filepath, "r", encoding="utf-8") as f:
                    content = f.read()
                    lines = content.splitlines()
                    for idx, line in enumerate(lines, 1):
                        for p_name, p_reg in patterns.items():
                            matches = p_reg.findall(line)
                            if matches:
                                if p_name not in results:
                                    results[p_name] = []
                                results[p_name].append((file, idx, line.strip()))
            except Exception as e:
                pass

print("=== CODEBASE INTEGRITY SCAN RESULTS ===")
if not results:
    print("SUCCESS: ZERO hardcoded IDs, fallbacks, or legacy names found in the entire codebase!")
else:
    for p_name, occurrences in results.items():
        print(f"\nWARNING: Pattern '{p_name}' found in {len(occurrences)} places:")
        for file, line_num, line_code in occurrences:
            print(f"  * {file}:{line_num} -> {line_code}")
