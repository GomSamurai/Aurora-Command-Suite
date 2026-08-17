import sys

sys.stdout.reconfigure(encoding='utf-8')

file_path = r"c:\VSCODE\AuroraDesignSuite\Services\DatabaseService.cs"

with open(file_path, "r", encoding="utf-8") as f:
    lines = f.readlines()

print("--- Searching for methods using GetConnection() in DatabaseService.cs ---")
current_func = ""
for i, line in enumerate(lines, 1):
    if "public " in line or "private " in line:
        current_func = line.strip()
    if "GetConnection()" in line or "GetConnection(readOnly: true)" in line:
        print(f"Line {i} in '{current_func}': {line.strip()}")

print("\n--- Searching for methods with UPDATE/INSERT/DELETE in DatabaseService.cs ---")
for i, line in enumerate(lines, 1):
    if "UPDATE " in line.upper() or "INSERT INTO " in line.upper() or "DELETE FROM " in line.upper():
        print(f"Line {i}: {line.strip()}")
