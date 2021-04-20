using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MyLibrary.Algorithms.Methods.Simplex
{
	public class SimplexAnswer
	{
		public AnswerStatus Status { get; set; }
		public List<Solution> Solutions { get; set; } = new List<Solution>();
		public SimplexAnswer(SimplexTable finalTable, AnswerStatus status = AnswerStatus.NoSolutions)
		{
			Status = status;
			Solution solution = new Solution();
			solution.BasisIndexes = (int[])finalTable.BasisVariables.Clone();
			solution.OptimalValue = finalTable.GoalFunctionValue;
			double[] optimalCoefficients = new double[finalTable.CountOfVariables];
			List<double> optimalBasis = new List<double>();
			List<int> freeIndexes = new List<int>();
			for (int i = 0; i < finalTable.CountOfVariables; i++)
			{
				if (finalTable.BasisVariables.Contains(i))
				{
					optimalCoefficients[i] = finalTable.FreeMemebers[i];
				}
				else
				{
					freeIndexes.Add(i);
					optimalCoefficients[i] = finalTable.GoalFunctionCoefficients[i];
				}
			}
			solution.FreeIndexes = freeIndexes.ToArray();
			solution.OptimalCoefficients = optimalCoefficients;
			Solutions.Add(solution);
		}
	}

	public enum AnswerStatus
	{
		NoSolutions,
		OneSolution,
		SeveralSolutions
	}

	public class Solution
	{
		public double OptimalValue { get; set; }
		public double[] OptimalCoefficients { get; set; }
		public int[] BasisIndexes { get; set; }
		public int[] FreeIndexes { get; set; }
	}
}
