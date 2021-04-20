using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MyLibrary.Extensions.MatrixExtensions;
using MyLibrary.Extensions.ArrayExtensions;

namespace MyLibrary.Algorithms.Methods.Simplex
{
	public class SimplexTable
	{
		public double[][] Matrix { get; set; }
		public double[] FreeMemebers { get; set; }
		public int[] BasisVariables { get; set; }
		public double[] GoalFunctionCoefficients { get; set; }
		public double GoalFunctionValue { get; set; }
		public int CurrentLeadingRow { get; set; }
		public int CountOfVariables { get; set; }

		public SimplexTable(double[][] variables, double[] freeMemebers, int[] basis, double[] goalFunctionCoefficients, double goalFunctionValue = 0)
		{
			Matrix = variables.CloneMatrix();
			FreeMemebers = (double[])freeMemebers.Clone();
			BasisVariables = (int[])basis.Clone();
			GoalFunctionCoefficients = (double[])goalFunctionCoefficients.Clone();
			GoalFunctionValue = goalFunctionValue;
			CountOfVariables = FreeMemebers.Length;
			CurrentLeadingRow = 0;
		}
	}
}
