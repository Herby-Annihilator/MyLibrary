using System;
using System.Collections.Generic;
using System.Text;

namespace MyLibrary.Generator
{
	public class Generator<T> where T : IGeneratable, new()
	{
		public int ObjectsCount { get; set; }

		public T[] Generate()
		{
			T[] toReturn = new T[ObjectsCount];
			for (int i = 0; i < ObjectsCount; i++)
			{
				toReturn[i] = new T();
				toReturn[i].Randomize();
			}
			return toReturn;
		}

		public Generator(int numberOfGenerations)
		{
			ObjectsCount = numberOfGenerations;
		}
	}
}
