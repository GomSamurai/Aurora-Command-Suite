import os

for root, dirs, files in os.walk(r"C:\VSCODE"):
    for f in files:
        if "aurorapatch" in f.lower() or "patch" in f.lower():
            print(os.path.join(root, f))
