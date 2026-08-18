import os

log1 = r"C:\Users\Fran\Desktop\Aurora271Full\AuroraSpanish.log"
log2 = r"C:\Users\Fran\Desktop\Aurora271Full\Patches\AuroraSpanish\AuroraSpanish.log"
log3 = r"C:\Users\Fran\Desktop\Aurora271Full\AuroraPatch.log"

print(f"Checking {log1}: Exists={os.path.exists(log1)}")
if os.path.exists(log1):
    with open(log1, "r", encoding="utf-8", errors="ignore") as f:
        print(f.read())

print(f"\nChecking {log2}: Exists={os.path.exists(log2)}")
if os.path.exists(log2):
    with open(log2, "r", encoding="utf-8", errors="ignore") as f:
        print(f.read())

print(f"\nChecking last lines of {log3}: Exists={os.path.exists(log3)}")
if os.path.exists(log3):
    with open(log3, "r", encoding="utf-8", errors="ignore") as f:
        lines = f.readlines()
        print("".join(lines[-30:]))
