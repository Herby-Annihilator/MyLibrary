using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MyLibrary.Algorithms.Methods.Simplex
{
	public class SimplexAnswer
	{
		public AnswerStatus Status { get; set; } = AnswerStatus.NoSolutions;
		public List<Solution> Solutions { get; set; } = new List<Solution>();
		public List<CommonVariableValue> CommonVariableValues { get; private set; } = new List<CommonVariableValue>();
		public SimplexAnswer(SimplexTable finalTable, AnswerStatus status = AnswerStatus.NoSolutions)
		{
			Status = status;
			Solution solution = new Solution();
			solution.BasisIndexes = (int[])finalTable.BasisVariablesIndexes.Clone();
			solution.OptimalValue = finalTable.GoalFunctionValue;
			double[] optimalCoefficients = new double[finalTable.CountOfVariables];
			List<int> freeIndexes = new List<int>();
			for (int i = 0; i < finalTable.CountOfVariables; i++)
			{
				if (finalTable.BasisVariablesIndexes.Contains(i))
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
		public SimplexAnswer()
		{

		}
	}

	public enum AnswerStatus
	{
		NoSolutions,
		OneSolution,
		SeveralSolutions,
		TargetFunctionUnlimited
	}

	public class Solution
	{
		public double OptimalValue { get; set; }
		public double[] OptimalCoefficients { get; set; }
		public int[] BasisIndexes { get; set; }
		public int[] FreeIndexes { get; set; }
	}

	public class CommonVariableValue
	{
		public string Name { get; private set; }
		private double _firstValue;
		private double _secondValue;
		public CommonVariableValue(double firstValue, double secondValue, string name)
		{
			Name = name;
			_firstValue = firstValue;
			_secondValue = secondValue;
		}
		public override string ToString()
		{
			double coefficient = _firstValue - _secondValue;
			return $"{_secondValue} {Math.Round(coefficient, 5)}α";
		}
		private string SignToString(double value)
		{
			if (value < 0)
				return "-";
			else
				return "+";
		}
	}
}
