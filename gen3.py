cs_lines = []
with open('seed_hospitals.sql', 'r', encoding='utf-8') as f:
    for line in f:
        line = line.strip(', \r\n\t;')
        if line.startswith('(') and len(line) > 50:
            line = line.replace('NULL', "'null'")
            # split by quote-aware logic or just simple split because there are no inner commas in these strings
            parts = line[1:-1].split(',')
            parts = [p.strip().strip("'") for p in parts]
            if len(parts) >= 13:
                services = parts[13] if len(parts) > 13 else parts[12]
                cs = f'new HospitalInfo {{ Name="{parts[1]}", Type="{parts[2]}", City="{parts[3]}", Pincode="{parts[4]}", Country="{parts[5]}", PhoneNumber="{parts[6]}", EmailAddress="{parts[7]}", WebsiteUrl="{parts[8]}", OperatingHours="{parts[9]}", RegistrationNumber="{parts[10]}", StreetAddress="{parts[11]}", Services="{services}" }}'
                cs_lines.append("                    " + cs)

code = '                var defaultHospitals = new List<HospitalInfo>\\n                {\n' + ',\\n'.join(cs_lines) + '\\n                };\n'

with open('csharp_patch.txt', 'w', encoding='utf-8') as f:
    f.write(code)

print(f"Generated {len(cs_lines)} hospitals")
