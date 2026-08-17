import os

aurora_path = r"c:\VSCODE\Aurora271Full"
print("Scanning aurora_path:", aurora_path)
for root, dirs, files in os.walk(aurora_path):
    rel = os.path.relpath(root, aurora_path)
    depth = rel.count(os.sep)
    if depth <= 2:
        img_files = [f for f in files if f.lower().endswith(('.jpg', '.jpeg', '.png', '.bmp', '.gif'))]
        if img_files:
            print(f"  Folder '{rel}': {len(img_files)} images (e.g. {img_files[:4]})")
