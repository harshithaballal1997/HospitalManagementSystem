with open('csharp_patch.txt', 'r', encoding='utf-8') as f:
    hospitals_list = f.read()

with open('Hospital.Utilities/DbIntializer.cs', 'r', encoding='utf-8') as f:
    src = f.read()

# 1. Remove the early return
src = src.replace('if (_context.Rooms.Any()) return;', '// removed early return')

# 2. Inject hospitals list
# We replace the empty var defaultHospitals = new List<HospitalInfo> { }; block we messed up previously
# So we need to find "var defaultHospitals = new List<HospitalInfo>" and the closing "};" and replace
import re
new_code = hospitals_list + '                _context.HospitalInfos.AddRange(defaultHospitals);\n                _context.SaveChanges();\n                hospitals.AddRange(defaultHospitals);'
src = re.sub(r'var defaultHospitals = new List<HospitalInfo>\s*\{\s*\};\s*_context\.HospitalInfos\.AddRange\(defaultHospitals\);\s*_context\.SaveChanges\(\);\s*hospitals\.AddRange\(defaultHospitals\);', new_code, src, flags=re.DOTALL)

with open('Hospital.Utilities/DbIntializer.cs', 'w', encoding='utf-8') as f:
    f.write(src)
print("DbInitializer modified successfully.")
