import os

for root, dirs, files in os.walk(r"C:\VSCODE\Aurora271Full"):
    for f in files:
        if f.lower().endswith(".dll"):
            print(os.path.join(root, f))
