using System;
using System.Collections.Generic;
using System.Text;

namespace MyLibrary.Algorithms.Methods.Simplex.SimplexData
{
	public class TargetFunction
	{
		public List<double> Coefficients { get; set; }
		public IOptimalityCriterion OptimalityCriterion { get; set; }
		public TargetFunction(List<double> coeffiecients, IOptimalityCriterion criterion)
		{
			Coefficients = coeffiecients;
			OptimalityCriterion = criterion;
		}
	}
}
