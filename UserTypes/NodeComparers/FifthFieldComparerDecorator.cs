using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MyLibrary.UserTypes.NodeComparers
{
	public class FifthFieldComparerDecorator : BaseComparerDecorator
	{
		public override int Compare([AllowNull] Node x, [AllowNull] Node y)
		{
			if (comparer.Compare(x, y) == 0)
			{
				return x.FifthField.CompareTo(y.FifthField);
			}
			return 0;
		}

		public FifthFieldComparerDecorator(IComparer<Node> comparer) : base(comparer)
		{

		}
	}
}
