using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TotekanReaderLib
{
	public class StringUtil
	{
		static public string stripQuote(string namePathQuoted)
		{
			if (namePathQuoted == null) return null;
			var regexp = new Regex("^\"([^\"]+)\"$");
			var matched = regexp.Match(namePathQuoted);
			if (!matched.Success) return namePathQuoted;
			return matched.Groups[1].Captures[0].Value;
		}
	}
}
