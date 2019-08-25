using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	static class StringUtility
	{
		public static readonly char[] WordBreakChars = new char[] { ' ', '_', '\t', '.', '+', '-', '(', ')', '[', ']', '\"', /*'\'',*/ '{', '}', '!', '<', '>', '~', '`', '*', '$', '#', '@', '!', '\\', '/', ':', ';', ',', '?', '^', '%', '&', '|', '\n', '\r', '\v', '\f', '\0' };
		public static IEnumerable<string> SplitWords(this string text, params char[] wordBreakChars)
		{
			if (null == wordBreakChars || 0 == wordBreakChars.Length)
				wordBreakChars = WordBreakChars;
			if (string.IsNullOrEmpty(text))
				yield break;
			int i = text.IndexOfAny(wordBreakChars);
			if (0 > i)
			{
				yield return text;
				yield break;
			}
			if (0 < i)
			{
				yield return text.Substring(0, i);
				i++;
			}
			int si = i;
			while (si < text.Length)
			{
				i = text.IndexOfAny(wordBreakChars, si);
				if (0 > i)
					i = text.Length;
				if (1 < i - si)
				{
					yield return text.Substring(si, i - si);
				}
				si = i + 1;
			}
		}
	}
}
