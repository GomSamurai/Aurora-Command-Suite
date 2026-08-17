import os

for root, dirs, files in os.walk(r"C:\VSCODE"):
    for f in files:
        if f.lower() == "auroradb.db":
            print(os.path.join(root, f))
