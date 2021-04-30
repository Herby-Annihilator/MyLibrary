using System;
using System.Collections.Generic;
using System.Linq;
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

		public static T[] AddAnother<T>(this T[] arr, T[] anotherArr, Func<T, T, T> howToAdd)
		{
			int minLength;
			if (arr.Length < anotherArr.Length)
				minLength = arr.Length;
			else
				minLength = anotherArr.Length;
			T[] toReturn = new T[arr.Length];
			for (int i = 0; i < minLength; i++)
			{
				toReturn[i] = howToAdd(arr[i], anotherArr[i]);
			}
			return toReturn;
		}
		public static void AddAnotherToThis<T>(this T[] arr, T[] anotherArr, Func<T, T, T> howToAdd)
		{
			int minLength;
			if (arr.Length < anotherArr.Length)
				minLength = arr.Length;
			else
				minLength = anotherArr.Length;
			for (int i = 0; i < minLength; i++)
			{
				arr[i] = howToAdd(arr[i], anotherArr[i]);
			}
		}

		public static bool ContainsElementsFrom<T>(this T[] arr, T[] anotherArr)
		{
			int minLength;
			if (arr.Length < anotherArr.Length)
				minLength = arr.Length;
			else
				minLength = anotherArr.Length;
			for (int i = 0; i < minLength; i++)
			{
				if (arr.Contains(anotherArr[i]))
					return true;
			}
			return false;
		}
	}
}
