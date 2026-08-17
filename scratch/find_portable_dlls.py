import os

for root, dirs, files in os.walk(r"C:\VSCODE\Aurora_Command_Suite_v2.7.1_Portable"):
    for f in files:
        if f.lower().endswith(".dll"):
            print(os.path.join(root, f))
