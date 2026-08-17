import re

with open(r"C:\VSCODE\Aurora271Full\Aurora.exe", "rb") as f:
    data = f.read()

# Find strings or types starting with Form or Map or main window classes
matches = set()
for m in re.finditer(rb'Form[A-Za-z0-9_]+', data):
    matches.add(m.group(0).decode('ascii', errors='ignore'))

print("--- Form classes in Aurora.exe ---")
for m in sorted(matches):
    print(m)
