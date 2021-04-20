using System;
using System.Collections.Generic;
using System.Text;

namespace MyLibrary.Algorithms.Methods.Simplex
{
	public class MaxOptimalityCriterion : IOptimalityCriterion
	{
		public int GetLeadingColumn(SimplexTable table)
		{
			int leadingColumn = 0;
			double minValue = table.GoalFunctionCoefficients[0];
			double currentValue;
			for (int i = 0; i < table.GoalFunctionCoefficients.Length; i++)
			{
				currentValue = table.GoalFunctionCoefficients[i];
				if (currentValue < minValue)
				{
					minValue = currentValue;
					leadingColumn = i;
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
			for (int i = 0; i < table.BasisVariables.Length; i++)
			{
				currentBasisIndex = table.BasisVariables[i];
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
			foreach (var coefficient in table.GoalFunctionCoefficients)
			{
				if (coefficient < 0)
					return false;
			}
			return true;
		}
		public override string ToString() => "Max";
	}
}
