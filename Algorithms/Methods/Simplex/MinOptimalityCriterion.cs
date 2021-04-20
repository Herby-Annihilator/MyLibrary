using System;
using System.Collections.Generic;
using System.Text;

namespace MyLibrary.Algorithms.Methods.Simplex
{
	public class MinOptimalityCriterion : IOptimalityCriterion
	{
		public int GetLeadingColumn(SimplexTable table)
		{
			throw new NotImplementedException();
		}

		public int GetLeadingRow(int leadingColumn, SimplexTable table)
		{
			throw new NotImplementedException();
		}

		public bool IsOptimal(SimplexTable table)
		{
			throw new NotImplementedException();
		}
		public override string ToString() => "Min";
	}
}
