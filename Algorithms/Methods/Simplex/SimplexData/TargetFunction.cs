using System;
using System.Collections.Generic;
using System.Text;

namespace MyLibrary.Algorithms.Methods.Simplex.SimplexData
{
	public class TargetFunction
	{
		public List<double> Coefficients { get; set; }
		public IOptimalityCriterion OptimalityCriterion { get; set; }
		public double FreeCoefficient { get; set; }
		public TargetFunction(List<double> coeffiecients, IOptimalityCriterion criterion, double freeCoefficient = default)
		{
			Coefficients = coeffiecients;
			OptimalityCriterion = criterion;
			FreeCoefficient = freeCoefficient;
		}
	}
}
