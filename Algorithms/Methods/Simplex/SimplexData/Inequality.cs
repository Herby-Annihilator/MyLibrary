using System;
using System.Collections.Generic;
using System.Text;

namespace MyLibrary.Algorithms.Methods.Simplex.SimplexData
{
	public class Inequality
	{
		public List<double> Coefficients { get; set; }
		public double RightPart { get; set; }
		public Sign Sign { get; set; }
		public Inequality(List<double> coefficients, double rightPart, Sign sign = Sign.EqualSign)
		{
			Coefficients = coefficients;
			RightPart = rightPart;
			Sign = sign;
		}
	}

	public enum Sign
	{
		EqualSign,
		MoreThanOrEqualSign,
		LessThanOrEqualSign
	}
}
