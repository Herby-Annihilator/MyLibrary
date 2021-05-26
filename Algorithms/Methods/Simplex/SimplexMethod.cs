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
				while (!minOptimalityCriterion.IsOptimal(_table))
				{
					if (minOptimalityCriterion.IsTargetFunctionUnlimited(_table))
					{
						throw new Exception("Расширенная функция не ограничена");
					}
					leadingColumn = minOptimalityCriterion.GetLeadingColumn(_table);
					leadingRow = minOptimalityCriterion.GetLeadingRow(leadingColumn, _table);
					UpdateSimplexTable(leadingColumn, leadingRow);
				}
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
				else
				{
					_table.GoalFunctionCoefficients[i] = -1;
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
			if (Math.Abs(_table.GoalFunctionValue) > Constants.EPS)
				throw new Exception("Получить допустимое базисное решение не удалось. Дальнейшие вычисления прекращены.");
			for (int i = 0; i < _table.FakeVariablesIndexes.Length; i++)
			{
				if (_table.GoalFunctionCoefficients[_table.FakeVariablesIndexes[i]] > Constants.EPS)
					throw new Exception("На конечной итерации одна или несколько ложных переменных приняли положительное значение. Дальнейшие вычисления прекращены т.к задача не имеет допустимого решения.");
			}
		}

		private void PrepareTableForSecondStep(double[] oldTargetFunctionCoefficients)
		{

			DeleteNonBasisFakeVariablesFromTheTable(oldTargetFunctionCoefficients);

			CorrectFakeBasisVariablesOrThrowException();

			CorrectTargetFunction();
		}

		private void DeleteNonBasisFakeVariablesFromTheTable(double[] oldTargetFunctionCoefficients)
		{
			List<int> toDelete = new List<int>();
			List<int> fakes = new List<int>();
			for (int i = 0; i < _table.FakeVariablesIndexes.Length; i++)
			{
				if (!_table.BasisVariablesIndexes.Contains(_table.FakeVariablesIndexes[i]))
				{
					toDelete.Add(_table.FakeVariablesIndexes[i]);
				}
				else
				{
					fakes.Add(_table.FakeVariablesIndexes[i]);
				}
			}
			int size = _table.Matrix.GetLength(0) - toDelete.Count;
			int currentRowInMatrix = 0, currentColInMatrix = 0;
			double[][] matrix = new double[size][];
			double[] freeMembers = new double[size];
			double[] targetFunctionCoefficients = new double[size];
			for (int i = 0; i < _table.Matrix.GetLength(0); i++)
			{
				if (!toDelete.Contains(i))
				{
					matrix[currentRowInMatrix] = new double[size];
					freeMembers[currentRowInMatrix] = _table.FreeMemebers[i];					
					currentColInMatrix = 0;
					for (int j = 0; j < _table.Matrix.GetLength(0); j++)
					{
						if (!toDelete.Contains(j))
						{
							matrix[currentRowInMatrix][currentColInMatrix] = _table.Matrix[i][j];
							currentColInMatrix++;
						}
					}
					currentRowInMatrix++;
				}
				else   // если удаляется строка, то те базисные строки, у которых индекс больше автоматически переезжают на уровень выше
				{
					for (int j = 0; j < _table.BasisVariablesIndexes.Length; j++)
					{
						if (_table.BasisVariablesIndexes[j] >= i)
						{
							_table.BasisVariablesIndexes[j]--;
						}
					}
					for (int j = 0; j < fakes.Count; j++)
					{
						if (fakes[j] >= i)
						{
							fakes[j]--;
						}
					}
				}
			}
			currentColInMatrix = 0;
			for (int i = 0; i < _table.Matrix.GetLength(0); i++)
			{
				if (!toDelete.Contains(i))
				{
					targetFunctionCoefficients[currentColInMatrix] = _table.GoalFunctionCoefficients[i] + oldTargetFunctionCoefficients[i];
					currentColInMatrix++;
				}
			}
			_table.Matrix = matrix;
			_table.FreeMemebers = freeMembers;
			_table.GoalFunctionCoefficients = targetFunctionCoefficients;
			_table.GoalFunctionValue = 0;
			_table.CountOfVariables = size;
			_table.FakeVariablesIndexes = fakes.ToArray();
		}
		private void CorrectFakeBasisVariablesOrThrowException()
		{
			int index;
			for (int i = 0; i < _table.BasisVariablesIndexes.Length; i++)
			{
				index = _table.BasisVariablesIndexes[i];
				if (_table.FakeVariablesIndexes.Contains(index))
				{
					if (Math.Abs(_table.FreeMemebers[index]) > Constants.EPS)
						throw new Exception("Искусственная переменная в базисе не ноль");
					else
						_table.FreeMemebers[index] = 0;
				}
			}
		}
		private void CorrectTargetFunction()
		{
			int index;
			double coefficient;
			_table.GoalFunctionValue = 0;
			for (int i = 0; i < _table.BasisVariablesIndexes.Length; i++)
			{
				index = _table.BasisVariablesIndexes[i];
				if (Math.Abs(_table.GoalFunctionCoefficients[index]) > Constants.EPS)
				{
					coefficient = _table.GoalFunctionCoefficients[index] * (-1);
					_table.GoalFunctionCoefficients.AddAnotherToThis(_table.Matrix[index], (first, second) => first + second * coefficient);
					_table.GoalFunctionValue += _table.FreeMemebers[index] * coefficient;
				}
			}
		}

		private SimplexAnswer SecondStep()
		{
			int leadingColumn, leadingRow;
			while (!_optimalityCriterion.IsOptimal(_table))
			{
				if (_optimalityCriterion.IsTargetFunctionUnlimited(_table))
				{
					return new SimplexAnswer()
					{
						Status = AnswerStatus.TargetFunctionUnlimited
					};
				}
				leadingColumn = _optimalityCriterion.GetLeadingColumn(_table);
				leadingRow = _optimalityCriterion.GetLeadingRow(leadingColumn, _table);
				UpdateSimplexTable(leadingColumn, leadingRow);
				
			}
			_table.GoalFunctionValue += _table.FreeCoefficient;  // костыль
			SimplexAnswer answer = new SimplexAnswer(_table, AnswerStatus.OneSolution);
			AddCommonSolutionIfNecessary(answer);
			return answer;
		}

		private void AddCommonSolutionIfNecessary(SimplexAnswer answer)
		{
			double[][] pairs = new double[_table.StartVariablesIndexes.Length][]; // изначальное число переменных = общее число (вместе с добавленными) - число базисных переменных
			for (int i = 0; i < _table.CountOfVariables; i++)
			{
				if (!_table.BasisVariablesIndexes.Contains(i))
				{
					if (_table.GoalFunctionCoefficients[i] == 0)
					{
						answer.Status = AnswerStatus.SeveralSolutions;
						for (int j = 0; j < _table.StartVariablesIndexes.Length; j++)  // изначальное число переменных = общее число (вместе с добавленными) - число базисных переменных
						{
							pairs[j] = new double[2];
							if (_table.BasisVariablesIndexes.Contains(j))
								pairs[j][0] = _table.FreeMemebers[j];
							else
								pairs[j][0] = _table.GoalFunctionCoefficients[j];
						}
						if (_optimalityCriterion.IsTargetFunctionUnlimited(_table))
						{
							throw new Exception("Целева функция не ограничена");
						}
						int leadingColumn = _optimalityCriterion.GetLeadingColumn(_table);
						int leadingRow = _optimalityCriterion.GetLeadingRow(leadingColumn, _table);
						UpdateSimplexTable(leadingColumn, leadingRow);

						for (int j = 0; j < _table.StartVariablesIndexes.Length; j++)  // изначальное число переменных = общее число (вместе с добавленными) - число базисных переменных
						{
							if (_table.BasisVariablesIndexes.Contains(j))
								pairs[j][1] = _table.FreeMemebers[j];
							else
								pairs[j][1] = _table.GoalFunctionCoefficients[j];
							answer.CommonVariableValues.Add(new CommonVariableValue(pairs[j][0], pairs[j][1], $"X{j + 1}"));
						}
						Solution alternativeSolution = new Solution();
						alternativeSolution.BasisIndexes = (int[])_table.BasisVariablesIndexes.Clone();
						alternativeSolution.OptimalValue = _table.GoalFunctionValue;
						double[] optimalCoefficients = new double[_table.CountOfVariables];
						List<int> freeIndexes = new List<int>();
						for (int j = 0; j < _table.CountOfVariables; j++) 
						{
							if (_table.BasisVariablesIndexes.Contains(j))
							{
								optimalCoefficients[j] = _table.FreeMemebers[j];
							}
							else
							{
								freeIndexes.Add(j);
								optimalCoefficients[j] = 0;
							}
						}
						alternativeSolution.FreeIndexes = freeIndexes.ToArray();
						alternativeSolution.OptimalCoefficients = optimalCoefficients;
						answer.Solutions.Add(alternativeSolution);
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
			List<int> basisIndexes, fakeIndexes;
			int[] startVariablesIndexes = new int[inequalities[0].Coefficients.Count];
			for (int i = 0; i < startVariablesIndexes.Length; i++)
			{
				startVariablesIndexes[i] = i;
			}
			ConfigureTableWithDifferentVariables(inequalities, targetFunction, out basisIndexes, out fakeIndexes);
			targetFunction.Coefficients.Map((element) => element *= -1);			
			int matrixSize = targetFunction.Coefficients.Count;
			int currentInequalityToAddAsBasis = 0;
			double[][] matrix = new double[matrixSize][];
			double[] freeMembers = new double[matrixSize];
			for (int i = 0; i < matrixSize; i++)
			{
				if (basisIndexes.Contains(i))
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
			SimplexTable table = new SimplexTable(matrix, freeMembers, basisIndexes.ToArray(), targetFunction.Coefficients.ToArray(), fakeIndexes.ToArray(), startVariablesIndexes, 0, targetFunction.FreeCoefficient);
			return table;
		}
		

		private static void ConfigureTableWithDifferentVariables(List<Inequality> inequalities, TargetFunction targetFunction, out List<int> basisIndexes, out List<int> fakeIndexes)
		{
			basisIndexes = new List<int>();
			fakeIndexes = new List<int>();
			for (int i = 0; i < inequalities.Count; i++)
			{
				if (inequalities[i].Sign == Sign.EqualSign)
				{
					basisIndexes.Add(inequalities[i].Coefficients.Count);
					fakeIndexes.Add(inequalities[i].Coefficients.Count);
					inequalities[i].Coefficients.Add(1);					
					targetFunction.Coefficients.Add(0);
					for (int j = 0; j < inequalities.Count; j++)
					{
						if (i != j)
						{
							inequalities[j].Coefficients.Add(0);
						}
					}
				}
				else if (inequalities[i].Sign == Sign.MoreThanOrEqualSign)
				{
					inequalities[i].Coefficients.Add(-1);
					basisIndexes.Add(inequalities[i].Coefficients.Count);
					fakeIndexes.Add(inequalities[i].Coefficients.Count);
					inequalities[i].Coefficients.Add(1); // fake
					targetFunction.Coefficients.Add(0);
					targetFunction.Coefficients.Add(0);					
					for (int j = 0; j < inequalities.Count; j++)
					{
						if (i != j)
						{
							inequalities[j].Coefficients.Add(0);
							inequalities[j].Coefficients.Add(0);
						}
					}
				}
				else
				{
					basisIndexes.Add(inequalities[i].Coefficients.Count);
					inequalities[i].Coefficients.Add(1);					
					targetFunction.Coefficients.Add(0);
					for (int j = 0; j < inequalities.Count; j++)
					{
						if (i != j)
						{
							inequalities[j].Coefficients.Add(0);
						}
					}
				}
			}
		}		
	}
}
