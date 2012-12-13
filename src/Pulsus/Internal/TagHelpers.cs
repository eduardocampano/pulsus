using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Pulsus.Internal
{
	internal class TagHelpers
	{
		public static string[] Clean(string[] tags, bool stripSpecialChars = true)
		{
			return Clean(string.Join(" ", tags), stripSpecialChars);
		}

		public static string[] Clean(string tagString, bool stripSpecialChars = true)
		{
			var res = new List<string>();
			var tokens = tagString.ToLower().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var token in tokens)
			{
				var temp = token.Trim();

				if (stripSpecialChars)
					temp = Regex.Replace(temp, "[^a-zA-Z0-9\\-]", "");

				if (temp.Length > 0)
					res.Add(token);
			}

			return res.ToArray();
		}
	}
}
