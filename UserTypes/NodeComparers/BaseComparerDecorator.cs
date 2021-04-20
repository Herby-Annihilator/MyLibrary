using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MyLibrary.UserTypes.NodeComparers
{
	public class BaseComparerDecorator : IComparer<Node>
	{
		protected IComparer<Node> comparer;
		public BaseComparerDecorator(IComparer<Node> comparer)
		{
			this.comparer = comparer;
		}
		public virtual int Compare([AllowNull] Node x, [AllowNull] Node y)
		{
			return comparer.Compare(x, y);
		}
	}
}
