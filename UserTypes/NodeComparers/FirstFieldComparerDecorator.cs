using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MyLibrary.UserTypes.NodeComparers
{
	public class FirstFieldComparerDecorator : BaseComparerDecorator
	{
		public override int Compare([AllowNull] Node x, [AllowNull] Node y)
		{
			if (comparer.Compare(x, y) == 0)
			{
				return x.FirstField.CompareTo(y.FirstField);
			}
			return 0;
		}

		public FirstFieldComparerDecorator(IComparer<Node> comparer) : base(comparer)
		{

		}
	}
}
