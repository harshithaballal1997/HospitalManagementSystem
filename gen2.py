import re

with open('seed_hospitals.sql', 'r') as f:
    sql = f.read()

match = re.search(r'VALUES\s*(.*?);', sql, re.DOTALL | re.IGNORECASE)
values = match.group(1).strip()
lines = values.split('\n')

cs_lines = []
for line in lines:
    line = line.strip(', \r\n\t')
    if not line.startswith('('): continue
    line = line.replace('NULL', "'null'")
    parts = line[1:-1].split(',')
    parts = [p.strip().strip("'") for p in parts]
    if len(parts) >= 13:
        # services might not be at 13 if there are fewer commas
        services = parts[13] if len(parts) > 13 else parts[12]
        cs = f'new HospitalInfo {{ Name="{parts[1]}", Type="{parts[2]}", City="{parts[3]}", Pincode="{parts[4]}", Country="{parts[5]}", PhoneNumber="{parts[6]}", EmailAddress="{parts[7]}", WebsiteUrl="{parts[8]}", OperatingHours="{parts[9]}", RegistrationNumber="{parts[10]}", StreetAddress="{parts[11]}", Services="{services}" }}'
        cs_lines.append("                    " + cs)

code = '                var defaultHospitals = new List<HospitalInfo>\\n                {\n' + ',\\n'.join(cs_lines) + '\\n                };\\n                _context.HospitalInfos.AddRange(defaultHospitals);\\n                _context.SaveChanges();\\n                hospitals.AddRange(defaultHospitals);'

with open('csharp_patch.txt', 'w') as f:
    f.write(code)

print(f"Generated {len(cs_lines)} hospitals")
