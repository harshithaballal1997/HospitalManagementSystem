using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Hospital.Services
{
    public class GeminiShield
    {
        private readonly Dictionary<string, string> _idMap = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _nameMap = new Dictionary<string, string>();
        private int _patientCounter = 1;
        private int _doctorCounter = 1;

        public string Anonymize(string text, List<string> patientNames, List<string> doctorNames)
        {
            if (string.IsNullOrEmpty(text)) return text;

            string scrubbedText = text;

            // 1. Scrub Patient Names
            foreach (var name in patientNames.OrderByDescending(n => n.Length))
            {
                if (!string.IsNullOrWhiteSpace(name) && scrubbedText.Contains(name, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (!_nameMap.ContainsKey(name.ToLowerInvariant()))
                    {
                        string placeholder = $"[PATIENT_{_patientCounter++}]";
                        _nameMap[name.ToLowerInvariant()] = placeholder;
                    }
                    scrubbedText = Regex.Replace(scrubbedText, Regex.Escape(name), _nameMap[name.ToLowerInvariant()], RegexOptions.IgnoreCase);
                }
            }

            // 2. Scrub Doctor Names
            foreach (var name in doctorNames.OrderByDescending(n => n.Length))
            {
                if (!string.IsNullOrWhiteSpace(name) && scrubbedText.Contains(name, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (!_nameMap.ContainsKey(name.ToLowerInvariant()))
                    {
                        string placeholder = $"[DOCTOR_{_doctorCounter++}]";
                        _nameMap[name.ToLowerInvariant()] = placeholder;
                    }
                    scrubbedText = Regex.Replace(scrubbedText, Regex.Escape(name), _nameMap[name.ToLowerInvariant()], RegexOptions.IgnoreCase);
                }
            }

            // 3. Scrub potential Email patterns
            scrubbedText = Regex.Replace(scrubbedText, @"[\w-\.]+@([\w-]+\.)+[\w-]{2,4}", "[EMAIL_REDACTED]");

            // 4. Scrub potential Phone patterns
            scrubbedText = Regex.Replace(scrubbedText, @"(\+?\d{1,3}[- ]?)?\d{10}", "[PHONE_REDACTED]");

            return scrubbedText;
        }

        public string DeAnonymize(string anonymizedText)
        {
            if (string.IsNullOrEmpty(anonymizedText)) return anonymizedText;

            string restoredText = anonymizedText;

            foreach (var kvp in _nameMap)
            {
                // Restore with proper casing (Capitalize first letter of each word for the UI)
                string realName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(kvp.Key);
                restoredText = restoredText.Replace(kvp.Value, realName);
            }

            return restoredText;
        }
    }
}
