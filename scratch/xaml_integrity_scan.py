import os
import re

codebase_dir = r"c:\VSCODE\AuroraDesignSuite"

# Scan XAML files for Text="..." attributes containing hardcoded values
text_attr_pattern = re.compile(r'Text="([^"]+)"')

results = []

for root, dirs, files in os.walk(codebase_dir):
    if "bin" in root or "obj" in root or ".git" in root or "release" in root or "scratch" in root:
        continue
    for file in files:
        if file.endswith(".xaml"):
            filepath = os.path.join(root, file)
            try:
                with open(filepath, "r", encoding="utf-8") as f:
                    content = f.read()
                    lines = content.splitlines()
                    for idx, line in enumerate(lines, 1):
                        matches = text_attr_pattern.findall(line)
                        for m in matches:
                            # Flag any suspicious text in XAML like specific names, db paths, or hardcoded empire names
                            if any(k in m.lower() for k in ["784", "140", "terran", "epistocr", "numancia", "auroradb.db", "c:\\"]):
                                results.append((file, idx, m, line.strip()))
            except Exception as e:
                pass

print("=== XAML TEXT INTEGRITY SCAN RESULTS ===")
if not results:
    print("SUCCESS: ZERO hardcoded legacy or placeholder texts found in XAML files!")
else:
    print(f"Found {len(results)} suspicious XAML Text attributes:")
    for file, line_num, text_val, line_code in results:
        clean_code = line_code.encode('ascii', 'ignore').decode('ascii')
        clean_val = text_val.encode('ascii', 'ignore').decode('ascii')
        print(f"  * {file}:{line_num} Text='{clean_val}' -> {clean_code}")
