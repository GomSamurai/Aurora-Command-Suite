import os

log_paths = [
    r"C:\Users\Fran\Desktop\Aurora271Full\AuroraSpanish.log",
    r"C:\Users\Fran\Desktop\Aurora271Full\Patches\AuroraSpanish\AuroraSpanish.log",
    r"C:\VSCODE\Aurora271Full\AuroraSpanish.log"
]

for p in log_paths:
    print(f"Checking: {p} (Exists: {os.path.exists(p)})")
    if os.path.exists(p):
        try:
            with open(p, "r", encoding="utf-8", errors="ignore") as f:
                print(f.read())
        except Exception as e:
            print(e)
