import re
with open('seed_hospitals.sql', 'r') as f:
    sql = f.read()

match = re.search(r'VALUES\s*(.*?);', sql, re.DOTALL | re.IGNORECASE)
lines = match.group(1).strip().split('\n')

cs_lines = []
for line in lines:
    line = line.strip(', \r\n\t')
    if not line.startswith('('): continue
    line = line.replace('NULL', "'null'")
    parts = line[1:-1].split(',')
    parts = [p.strip().strip("'") for p in parts]
    if len(parts) >= 14:
        cs = f'new HospitalInfo {{ Name="{parts[1]}", Type="{parts[2]}", City="{parts[3]}", Pincode="{parts[4]}", Country="{parts[5]}", PhoneNumber="{parts[6]}", EmailAddress="{parts[7]}", WebsiteUrl="{parts[8]}", OperatingHours="{parts[9]}", RegistrationNumber="{parts[10]}", StreetAddress="{parts[11]}", Services="{parts[13]}" }}'
        cs_lines.append("                    " + cs)

code = 'var defaultHospitals = new List<HospitalInfo>\\n                {\n' + ',\\n'.join(cs_lines) + '\\n                };\\n                _context.HospitalInfos.AddRange(defaultHospitals);\\n                _context.SaveChanges();\\n                hospitals.AddRange(defaultHospitals);'

with open('Hospital.Utilities/DbIntializer.cs', 'r') as f:
    src = f.read()

src = re.sub(r'var defaultHospital = new HospitalInfo.*?_context\.SaveChanges\(\);\s*hospitals\.Add\(defaultHospital\);', code, src, flags=re.DOTALL)

with open('Hospital.Utilities/DbIntializer.cs', 'w', encoding='utf-8') as f:
    f.write(src)

print("Done")
