import os

for root, dirs, files in os.walk(r"C:\VSCODE"):
    for f in files:
        if f.lower() in ("aurorapatch.dll", "0harmony.dll", "aurora.exe"):
            print(os.path.join(root, f))
