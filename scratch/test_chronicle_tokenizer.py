import re

messages = [
    "Capitán de Corbeta Maximiliano Camon Pichardo has retired from the service at the age of 32. Current Assignment: Unassigned",
    "A science team led by Luana Cadena Valentín working on Earth has completed research into Ontiveros Armaments Gauss Cannon R200-85.00",
    "Fuel storage for harvester Buna II 003 is more than 90% full",
    "Seeker Thurggiss Bim Tlaorkehk has been killed in an accident. Assignment prior to death: Unassigned",
    "The Tactical bonus of R6 Frund Lorin Thissith has increased to 5%    Current Bonuses:  Crew Training 10%    Engineering 10%    Tactical 5%    Logistics 5%    Ground Support 10%     Current Assignment:  Executive Officer CLE-08 Harry Chauvel 008",
    "Production of Military Academy completed at Earth",
    "4th Garrison trained on Doxa Prime",
    "Trorsh Caaffullv promoted to Adept",
    "Buna 001 has run out of fuel"
]

ranks = r"(?:Capitán de (?:Corbeta|Navío|Fragata)|Almirante|Comandante|Adept|Seeker|Syntagmatarchis|Antisyntagmatarchis|CIV|R\d+|Dr\.|Científico)"
tech_keywords = r"(?:research into|Research Project:)\s+([A-Za-z0-9\-\.\s\/]+?)(?=\s+(?:working|completed|at|$))"

print("Parsing Sample GameLog Messages:")
for msg in messages:
    print(f"\nRAW: {msg}")

    # Check for tech
    tech_match = re.search(r"research into\s+(.+)", msg)
    if tech_match:
        print(f"  [TECH]: {tech_match.group(1)}")

    # Check for officer name
    rank_match = re.search(r"(?:Capitán de Corbeta|Capitán de Navío|Seeker|Adept|R\d+)\s+([A-Z][a-z]+(?:\s+[A-Z][a-z]+)*)", msg)
    if rank_match:
        print(f"  [OFFICER]: {rank_match.group(0)}")

    # Check for percents
    percents = re.findall(r"\b\d+%\b", msg)
    if percents:
        print(f"  [PERCENTS]: {percents}")
