using System;
using System.Collections.Generic;
using System.Text;

namespace MyLibrary.Algorithms.Methods.Simplex
{
	public interface IOptimalityCriterion
	{
		int GetLeadingColumn(SimplexTable table);
		int GetLeadingRow(int leadingColumn, SimplexTable table);
		bool IsOptimal(SimplexTable table);
	}
}
