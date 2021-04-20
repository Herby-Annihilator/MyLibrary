using System;
using System.Collections.Generic;
using System.Text;
using MyLibrary.Extensions.ArrayExtensions;

namespace MyLibrary.Algorithms.Methods.Simplex
{
	public class SimplexMethod : IMethod<SimplexAnswer>
	{
		private SimplexTable _table;
		private IOptimalityCriterion _optimalityCriterion;

		public SimplexAnswer Solve()
		{
			int leadingColumn, leadingRow;
			do
			{
				leadingColumn = _optimalityCriterion.GetLeadingColumn(_table);
				leadingRow = _optimalityCriterion.GetLeadingRow(leadingColumn, _table);
				UpdateSimplexTable(leadingColumn, leadingRow);
			} while (!_optimalityCriterion.IsOptimal(_table));
			SimplexAnswer answer = new SimplexAnswer(_table, AnswerStatus.OneSolution);
			return answer;
		}

		private void UpdateSimplexTable(int leadingColumn, int leadingRow)
		{
			CalculateNewLeadingRow(leadingRow, leadingColumn);
			CalculateOtherElements(leadingColumn);
		}
		private void CalculateNewLeadingRow(int leadingRow, int leadingColumn) // старая строка и старый столбец
		{
			for (int i = 0; i < _table.CountOfVariables; i++)
			{
				_table.Matrix[leadingColumn][i] = _table.Matrix[leadingRow][i] /
					_table.Matrix[leadingRow][leadingColumn];
			}
			_table.FreeMemebers[leadingColumn] = _table.FreeMemebers[leadingRow] / _table.Matrix[leadingRow][leadingColumn];
			_table.BasisVariables[_table.BasisVariables.IndexOf(leadingRow,
				(first, second) =>
				{
					if (first > second) return 1;
					if (first < second) return -1;
					return 0;
				})] = leadingColumn;
			_table.CurrentLeadingRow = leadingColumn;
		}
		private void CalculateOtherElements(int leadingColumn)
		{
			// новая строка = текущая строка - ее коэф. в ведущем столбце * новая ведущая строка
			int currentBasisVariable;
			double leadingElement;
			for (int i = 0; i < _table.BasisVariables.Length; i++)
			{
				currentBasisVariable = _table.BasisVariables[i];
				if (currentBasisVariable != _table.CurrentLeadingRow)
				{
					leadingElement = _table.Matrix[currentBasisVariable][leadingColumn];
					for (int j = 0; j < _table.FreeMemebers.Length; j++)
					{
						_table.Matrix[currentBasisVariable][j] = _table.Matrix[currentBasisVariable][j] - leadingElement * _table.Matrix[_table.CurrentLeadingRow][j];
					}
					_table.FreeMemebers[currentBasisVariable] -= leadingElement * _table.FreeMemebers[_table.CurrentLeadingRow];
				}				
			}
			leadingElement = _table.GoalFunctionCoefficients[leadingColumn];
			for (int i = 0; i < _table.GoalFunctionCoefficients.Length; i++)
			{
				_table.GoalFunctionCoefficients[i] -= leadingElement * _table.Matrix[_table.CurrentLeadingRow][i];
			}
			_table.GoalFunctionValue -= leadingElement * _table.FreeMemebers[_table.CurrentLeadingRow];
		}

		public SimplexMethod(SimplexTable startTable, IOptimalityCriterion optimalityCriterion)
		{
			_table = startTable;
			_optimalityCriterion = optimalityCriterion;
		}
	}
}
