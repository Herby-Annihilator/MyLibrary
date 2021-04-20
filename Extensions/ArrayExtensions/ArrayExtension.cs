using System;
using System.Collections.Generic;
using System.Text;

namespace MyLibrary.Extensions.ArrayExtensions
{
	public static class ArrayExtension
	{
		public static int IndexOf<T>(this T[] arr, T element, Func<T, T, int> comparer)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				if (comparer(arr[i], element) == 0)
					return i;
			}
			return -1;
		}
		public static T[] Map<T>(this T[] arr, Func<T, T> func)
		{
			T[] toReturn = (T[])arr.Clone();
			for (int i = 0; i < arr.Length; i++)
			{
				toReturn[i] = func(arr[i]);
			}
			return toReturn;
		}
	}
}
