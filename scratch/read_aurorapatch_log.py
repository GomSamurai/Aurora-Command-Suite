log_path = r"C:\Users\Fran\Desktop\Aurora271Full\AuroraPatch.log"
try:
    with open(log_path, "r", encoding="utf-8", errors="ignore") as f:
        print(f.read())
except Exception as e:
    print(e)
