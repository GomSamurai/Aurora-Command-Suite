import os
import json

path = r"C:\Users\Fran\Desktop\Aurora271Full"

print("--- Inspecting Desktop AuroraPatch files ---")
for f in os.listdir(path):
    if "patch" in f.lower() or "setting" in f.lower() or f.endswith(".json") or f.endswith(".xml"):
        print(f"File: {f}")
        try:
            with open(os.path.join(path, f), "r") as file:
                print(file.read()[:500])
        except Exception as e:
            print("Error reading:", e)

patches_dir = os.path.join(path, "Patches")
print(f"\nPatches directory exists: {os.path.exists(patches_dir)}")
if os.path.exists(patches_dir):
    print("Patches folder contents:", os.listdir(patches_dir))
    for p in os.listdir(patches_dir):
        p_path = os.path.join(patches_dir, p)
        if os.path.isdir(p_path):
            print(f"   Subfolder {p}:", os.listdir(p_path))
