using System;
using System.Collections.Generic;
using System.Text;
using MyLibrary.Generator;

namespace MyLibrary.Extensions.StringExtensions
{
	public static class StringExtension
	{
		public static string CreateRandomString(this string str, int maxLenght)
		{
			if (maxLenght <= 0)
			{
				throw new GenerationExeption("String length cannot be less than 1");
			}
			Random random = new Random();
			int strLenght = random.Next(1, maxLenght);
			char[] charArray = new char[strLenght];
			for (int i = 0; i < strLenght; i++)
			{
				charArray[i] = (char)random.Next(32, 127);
			}
			return new string(charArray);
		}
	}
}
