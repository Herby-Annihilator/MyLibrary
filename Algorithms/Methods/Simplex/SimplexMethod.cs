using System;
using System.Collections.Generic;
using System.Text;
using MyLibrary.Extensions.ArrayExtensions;
using MyLibrary.Extensions;
using MyLibrary.Algorithms.Methods.Simplex.SimplexData;
using System.Linq;
using System.Collections.ObjectModel;

namespace MyLibrary.Algorithms.Methods.Simplex
{
	public class SimplexMethod : IMethod<SimplexAnswer>
	{
		private SimplexTable _table;
		private IOptimalityCriterion _optimalityCriterion;

		public SimplexAnswer Solve()
		{
			FirstStep();
			return SecondStep();
		}

		private void FirstStep()
		{
			if (_table.FakeVariablesIndexes.Length > 0)
			{
				double[] oldTargetFunctionCoefficients = (double[])_table.GoalFunctionCoefficients.Clone();
				NormalizeFakeTargetFunction();
				MinOptimalityCriterion minOptimalityCriterion = new MinOptimalityCriterion();
				int leadingColumn, leadingRow;
				do
				{
					leadingColumn = minOptimalityCriterion.GetLeadingColumn(_table);
					leadingRow = minOptimalityCriterion.GetLeadingRow(leadingColumn, _table);
					UpdateSimplexTable(leadingColumn, leadingRow);
				} while (!minOptimalityCriterion.IsOptimal(_table));
				CheckFakeSolution();
				PrepareTableForSecondStep(oldTargetFunctionCoefficients);
			}
		}
		private void NormalizeFakeTargetFunction()
		{
			for (int i = 0; i < _table.GoalFunctionCoefficients.Length; i++)
			{
				if (!_table.FakeVariablesIndexes.Contains(i))
				{
					_table.GoalFunctionCoefficients[i] = 0;
				}
			}
			int currentFakeIndex;
			for (int i = 0; i < _table.FakeVariablesIndexes.Length; i++)
			{
				currentFakeIndex = _table.FakeVariablesIndexes[i];
				for (int j = 0; j < _table.GoalFunctionCoefficients.Length; j++)
				{
					_table.GoalFunctionCoefficients[j] += _table.Matrix[currentFakeIndex][j] * _table.Matrix[currentFakeIndex][currentFakeIndex];
				}
				_table.GoalFunctionValue += _table.FreeMemebers[currentFakeIndex] * _table.Matrix[currentFakeIndex][currentFakeIndex];
			}
		}

		private void CheckFakeSolution()
		{
			if (_table.GoalFunctionValue != 0)
				throw new Exception("Получить допустимое базисное решение не удалось. Дальнейшие вычисления прекращены.");
			for (int i = 0; i < _table.FakeVariablesIndexes.Length; i++)
			{
				if (_table.BasisVariablesIndexes.Contains(_table.FakeVariablesIndexes[i]))
					throw new Exception("Ложная переменная оказалась в базисе. Такое теоретически возможно. Однако, дальнейшие вычисления прекращены");
				else if (_table.GoalFunctionCoefficients[_table.FakeVariablesIndexes[i]] > 0)
					throw new Exception("На конечной итерации одна или несколько ложных переменных приняли положительное значение. Дальнейшие вычисления прекращены т.к задача не имеет допустимого решения.");
			}
		}

		private void PrepareTableForSecondStep(double[] oldTargetFunctionCoefficients)
		{
			int matrixSize = _table.Matrix.GetLength(0) - _table.FakeVariablesIndexes.Length;
			double[][] matrix = new double[matrixSize][];
			double[] freeMembers = new double[matrixSize];
			double[] targetFunctionCoefficients = new double[matrixSize];
			//
			// Поскольку ложные переменные добавлялись в таблицу последними, то они находятся в ее 
			// конце (как по строкам, так и по столбцам), поэтому их можно просто отсечь.
			// Изменять индексы базисных переменных тоже не потребуется, поэтому можно исползовать 
			// старый базис
			//
			for (int i = 0; i < matrixSize; i++)
			{
				matrix[i] = new double[matrixSize];
				freeMembers[i] = _table.FreeMemebers[i];
				targetFunctionCoefficients[i] = oldTargetFunctionCoefficients[i];
				for (int j = 0; j < matrixSize; j++)
				{
					matrix[i][j] = _table.Matrix[i][j];
				}
			}
			_table.Matrix = matrix;
			_table.FreeMemebers = freeMembers;
			_table.GoalFunctionCoefficients = targetFunctionCoefficients;
			_table.GoalFunctionValue = 0;
			_table.CountOfVariables = matrixSize;
			_table.FakeVariablesIndexes = new int[0];
		}
		private SimplexAnswer SecondStep()
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

		private void AddCommonSolutionIfNecessary(SimplexAnswer answer)
		{
			double[,] pairs = new double[_table.BasisVariablesIndexes.Length, 2];
			for (int i = 0; i < _table.CountOfVariables; i++)
			{
				if (!_table.BasisVariablesIndexes.Contains(i))
				{
					if (_table.GoalFunctionCoefficients[i] == 0)
					{
						answer.Status = AnswerStatus.SeveralSolutions;
						for (int j = 0; j < _table.BasisVariablesIndexes.Length; j++)
						{
							if (_table.BasisVariablesIndexes.Contains(j))
								pairs[j, 0] = _table.FreeMemebers[j];
							else
								pairs[j, 0] = _table.GoalFunctionCoefficients[j];
						}
						int leadingColumn = _optimalityCriterion.GetLeadingColumn(_table);
						int leadingRow = _optimalityCriterion.GetLeadingRow(leadingColumn, _table);
						UpdateSimplexTable(leadingColumn, leadingRow);

						for (int j = 0; j < _table.BasisVariablesIndexes.Length; j++)
						{
							if (_table.BasisVariablesIndexes.Contains(j))
								pairs[j, 1] = _table.FreeMemebers[j];
							else
								pairs[j, 1] = _table.GoalFunctionCoefficients[j];
						}

						break;
					}
				}
			}
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
			_table.BasisVariablesIndexes[_table.BasisVariablesIndexes.IndexOf(leadingRow,
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
			for (int i = 0; i < _table.BasisVariablesIndexes.Length; i++)
			{
				currentBasisVariable = _table.BasisVariablesIndexes[i];
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

		public static SimplexTable PrepareFirstSimplexTable(List<Inequality> inequalities, TargetFunction targetFunction)
		{
			int countOfAddedVariables = AddResidualAndAdditionalVariablesTo(inequalities, targetFunction);
			List<int> basis = GetBasisIndexes(inequalities, countOfAddedVariables);
			int[] fakeVariablesIndexes = new int[0];
			if (countOfAddedVariables < inequalities.Count)
			{
				fakeVariablesIndexes = AddFakeVariables(inequalities, targetFunction);
				basis.AddRange(fakeVariablesIndexes);
				for (int i = 0; i < basis.Count; i++)
				{
					for (int j = 0; j < inequalities.Count; j++)
					{
						if (inequalities[j].Coefficients[basis[i]] < 0)
						{
							basis.RemoveAt(i);
							i--;
							break;
						}
					}
				}
			}
			targetFunction.Coefficients.Map((element) => element *= -1);			
			int matrixSize = targetFunction.Coefficients.Count;
			int currentInequalityToAddAsBasis = 0;
			double[][] matrix = new double[matrixSize][];
			double[] freeMembers = new double[matrixSize];
			for (int i = 0; i < matrixSize; i++)
			{
				if (basis.Contains(i))
				{
					matrix[i] = inequalities[currentInequalityToAddAsBasis].Coefficients.ToArray();
					freeMembers[i] = inequalities[currentInequalityToAddAsBasis].RightPart;
					currentInequalityToAddAsBasis++;
				}
				else
				{
					matrix[i] = new double[matrixSize];
				}
			}
			SimplexTable table = new SimplexTable(matrix, freeMembers, basis.ToArray(), targetFunction.Coefficients.ToArray(), fakeVariablesIndexes);
			return table;
		}
		private static int AddResidualAndAdditionalVariablesTo(List<Inequality> inequalities, TargetFunction targetFunction)
		{
			int countOfAddedVariables = 0;
			int countOfVariablesToAdd = inequalities.Count;
			for (int i = 0; i < countOfVariablesToAdd; i++)
			{
				if (inequalities[i].Sign != Sign.EqualSign)
				{
					countOfAddedVariables++;
					if (inequalities[i].Sign == Sign.LessThanOrEqualSign)
					{
						inequalities[i].Coefficients.Add(1);
					}
					else
					{
						inequalities[i].Coefficients.Add(-1);
					}
					for (int j = 0; j < countOfVariablesToAdd; j++)
					{
						if (i != j)
							inequalities[j].Coefficients.Add(0);
					}
					targetFunction.Coefficients.Add(0);
				}				
			}
			return countOfAddedVariables;
		}
		
		private static int[] AddFakeVariables(List<Inequality> inequalities, TargetFunction targetFunction)
		{
			List<int> fakeVariablesIndexes = new List<int>();
			for (int i = 0; i < inequalities.Count; i++)
			{
				if (inequalities[i].Sign != Sign.LessThanOrEqualSign)
				{
					fakeVariablesIndexes.Add(inequalities[i].Coefficients.Count);
					inequalities[i].Coefficients.Add(1);
					for (int j = 0; j < inequalities.Count; j++)
					{
						if (i != j)
							inequalities[i].Coefficients.Add(0);
					}
					targetFunction.Coefficients.Add(0);
				}
			}
			return fakeVariablesIndexes.ToArray();
		}
		private static List<int> GetBasisIndexes(List<Inequality> inequalities, int countOfAddedVariables)
		{
			List<int> basisIndexes = new List<int>();
			for (int i = inequalities[0].Coefficients.Count - countOfAddedVariables; i < inequalities[0].Coefficients.Count; i++)
			{
				basisIndexes.Add(i);
			}
			return basisIndexes;
		}
	}
}
