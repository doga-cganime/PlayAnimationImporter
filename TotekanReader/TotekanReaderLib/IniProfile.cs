using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace TotekanReaderLib
{
    public class IniProfile
    {
        public Dictionary<string, Dictionary<string, string>> profiles = new Dictionary<string, Dictionary<string, string>>();

        public IniProfile()
        {
        }

        public IniProfile(string nameFile)
        {
            this.load(nameFile);
        }

        public bool load(string nameFile)
        {
            using (var reader = new StreamReader(nameFile, Encoding.GetEncoding("Shift_JIS"))) {
				string nameSection = "";
				string line;
				while ((line = reader.ReadLine()) != null) {
					if (line.Length == 0) continue;
					var regexComment = new Regex(@"^\s*;");
					if (regexComment.IsMatch(line)) continue;
					var regexSection = new Regex(@"^\s*\[([^\]]+)+\]");
					var matchedSection = regexSection.Match(line);
					if (matchedSection.Success) {
						var group = matchedSection.Groups[1];
						nameSection = group.Captures[0].Value;
						continue;
					} 
					var regexParam = new Regex(@"^\s*([^\s=]+)\s*=\s*([^\s].*)$");
					var matchedParam = regexParam.Match(line);
					if (matchedParam.Success && matchedParam.Groups.Count == 3) {
						var key = StringUtil.stripQuote(matchedParam.Groups[1].Captures[0].Value);
						var value = StringUtil.stripQuote(matchedParam.Groups[2].Captures[0].Value);
						if (!this.profiles.ContainsKey(nameSection)) {
							this.profiles.Add(nameSection, new Dictionary<string, string>());
						}
						var section = this.profiles[nameSection];
						section[key] = value;

					}


				}
				
                
            }
            return false;
        }
    }
}
