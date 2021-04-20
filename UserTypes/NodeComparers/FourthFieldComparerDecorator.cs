using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MyLibrary.UserTypes.NodeComparers
{
	public class FourthFieldComparerDecorator : BaseComparerDecorator
	{
		public override int Compare([AllowNull] Node x, [AllowNull] Node y)
		{
			if (comparer.Compare(x, y) == 0)
			{
				return x.FourthField.CompareTo(y.FourthField);
			}
			return 0;
		}

		public FourthFieldComparerDecorator(IComparer<Node> comparer) : base(comparer)
		{

		}
	}
}
