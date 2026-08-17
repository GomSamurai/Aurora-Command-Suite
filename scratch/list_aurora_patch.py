import os

dir_path = r"C:\VSCODE\Aurora271Full\Unaltered base files\AurouraPatch compiled"
if os.path.exists(dir_path):
    for f in os.listdir(dir_path):
        print(f)
