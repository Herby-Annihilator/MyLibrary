using System;
using System.Collections.Generic;
using System.Text;

namespace MyLibrary.Extensions.MatrixExtensions
{
	public static class MatrixExtension
	{
		public static T[][] CloneMatrix<T>(this T[][] matrix)
		{
			if (matrix == null)
				return null;
			T[][] toReturn = new T[matrix.GetLength(0)][];
			for (int i = 0; i < matrix.GetLength(0); i++)
			{
				toReturn[i] = new T[matrix[i].Length];
				for (int j = 0; j < matrix[i].Length; j++)
				{
					toReturn[i][j] = matrix[i][j];
				}
			}
			return toReturn;
		}
	}
}
