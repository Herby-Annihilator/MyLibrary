using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MyLibrary.Algorithms.Methods.Simplex
{
	public class MinOptimalityCriterion : IOptimalityCriterion
	{
		public int GetLeadingColumn(SimplexTable table)
		{
			int leadingColumn = 0;
			double maxValue = -1;
			double currentValue;
			for (int i = 0; i < table.CountOfVariables; i++)
			{
				if (!table.BasisVariablesIndexes.Contains(i))
				{
					currentValue = table.GoalFunctionCoefficients[i];
					if (currentValue > maxValue)
					{
						maxValue = currentValue;
						leadingColumn = i;
					}
				}				
			}
			return leadingColumn;
		}

		public int GetLeadingRow(int leadingColumn, SimplexTable table)
		{
			double currentRatio, minRatio;
			int leadingRow = -1;
			minRatio = double.MaxValue;
			int currentBasisIndex;
			for (int i = 0; i < table.BasisVariablesIndexes.Length; i++)
			{
				currentBasisIndex = table.BasisVariablesIndexes[i];
				currentRatio = table.FreeMemebers[currentBasisIndex] / table.Matrix[currentBasisIndex][leadingColumn];
				if (currentRatio >= 0 && !double.IsNaN(currentRatio) && !double.IsInfinity(currentRatio))
				{
					if (currentRatio < minRatio)
					{
						minRatio = currentRatio;
						leadingRow = currentBasisIndex;
					}
				}
			}
			return leadingRow;
		}

		public bool IsOptimal(SimplexTable table)
		{
			for (int i = 0; i < table.CountOfVariables; i++)
			{
				if (!table.BasisVariablesIndexes.Contains(i))
				{
					if (table.GoalFunctionCoefficients[i] > 0)
						return false;
				}
			}
			return true;
		}

		public bool IsTargetFunctionUnlimited(SimplexTable simplexTable)
		{
			for (int i = 0; i < simplexTable.CountOfVariables; i++)
			{
				if (!simplexTable.BasisVariablesIndexes.Contains(i))
				{
					if (simplexTable.GoalFunctionCoefficients[i] > 0)
					{
						for (int j = 0; j < simplexTable.BasisVariablesIndexes.Length; j++)
						{
							if (simplexTable.Matrix[simplexTable.BasisVariablesIndexes[j]][i] > 0)
								return false;
						}
					}
				}
			}
			return true;
		}

		public override string ToString() => "Min";
	}
}
