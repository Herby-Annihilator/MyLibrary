using System;
using System.Collections.Generic;
using System.Text;

namespace MyLibrary.Extensions
{
	public static class ListExtensions
	{
		public static void Map<T>(this List<T> list, Func<T, T> func)
		{
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = func(list[i]);
			}
		}
	}
}
