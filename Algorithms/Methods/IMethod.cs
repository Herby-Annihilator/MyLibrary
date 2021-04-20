using System;
using System.Collections.Generic;
using System.Text;

namespace MyLibrary.Algorithms.Methods
{
	public interface IMethod<T>
	{
		public T Solve();
	}
}
