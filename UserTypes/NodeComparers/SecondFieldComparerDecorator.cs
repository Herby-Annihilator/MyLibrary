using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MyLibrary.UserTypes.NodeComparers
{
	public class SecondFieldComparerDecorator : BaseComparerDecorator
	{
		public override int Compare([AllowNull] Node x, [AllowNull] Node y)
		{
			if (comparer.Compare(x, y) == 0)
			{
				return x.SecondField.CompareTo(y.SecondField);
			}
			return 0;
		}

		public SecondFieldComparerDecorator(IComparer<Node> comparer) : base(comparer)
		{

		}
	}
}
