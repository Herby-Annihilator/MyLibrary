using System;
using System.Collections.Generic;
using System.Text;

namespace MyLibrary.Extensions.StringExtensions
{
	public static class RabinKarpAlgorithm
	{
		public static int[] SearchSubstringOccurrences(this string text, string substring)
		{
			List<int> substringOccurrences = new List<int>();
			ulong hashText = 0;
			ulong hashSubstring = 0;
			ulong Q = 100007;
			ulong D = 256;

			for (int i = 0; i < substring.Length; i++)
			{
				hashText = (hashText * D + (ulong)text[i]) % Q;
				hashSubstring = (hashSubstring * D + (ulong)substring[i]) % Q;
			}

			if (hashText == hashSubstring)
				substringOccurrences.Add(0);

			ulong pow = 1;

			for (int k = 1; k <= substring.Length - 1; k++)
				pow = (pow * D) % Q;

			for (int j = 1; j <= text.Length - substring.Length; j++)
			{
				hashText = (hashText + Q - pow * (ulong)text[j - 1] % Q) % Q;
				hashText = (hashText * D + (ulong)text[j + substring.Length - 1]) % Q;

				if (hashText == hashSubstring)
					if (text.Substring(j, substring.Length) == substring)
						substringOccurrences.Add(j);
			}

			return substringOccurrences.ToArray();
		}
	}
}
