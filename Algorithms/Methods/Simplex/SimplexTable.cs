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
		public int[] BasisVariablesIndexes { get; set; }
		public double[] GoalFunctionCoefficients { get; set; }
		public int[] FakeVariablesIndexes { get; set; }
		public double GoalFunctionValue { get; set; }
		public int CurrentLeadingRow { get; set; }
		public int CountOfVariables { get; set; }
		public double FreeCoefficient { get; set; }

		public SimplexTable(double[][] variables, double[] freeMemebers, int[] basis, double[] goalFunctionCoefficients, int[] fakeVariablesIndexes, double goalFunctionValue = 0, double freeCoef = 0)
		{
			Matrix = variables.CloneMatrix();
			FreeMemebers = (double[])freeMemebers.Clone();
			BasisVariablesIndexes = (int[])basis.Clone();
			GoalFunctionCoefficients = (double[])goalFunctionCoefficients.Clone();
			GoalFunctionValue = goalFunctionValue;
			CountOfVariables = FreeMemebers.Length;
			CurrentLeadingRow = 0;
			FakeVariablesIndexes = (int[])fakeVariablesIndexes.Clone();
			FreeCoefficient = freeCoef;
		}
	}
}
